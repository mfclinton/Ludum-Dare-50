using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabbage : MonoBehaviour
{
    public GeneticVector chromosome;
    public float grown_p;
    public Vector2 size_constraints, weight_constraints, nutrition_constraints;
    public LandPlot plot;

    float size_addon = 0.25f;
    private MeshRenderer[] mrs;
    private ParticleSystem ps;
    private Animator anim;

    private void Awake()
    {
        mrs = GetComponentsInChildren<MeshRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
    }


    //private void Update()
    //{
    //    UpdateAppearance();
    //}


    public void Set(GeneticVector chromosome, Vector2 size_constraints, Vector2 weight_constraints)
    {
        this.chromosome = chromosome;
        this.size_constraints = new Vector2(size_constraints[0], size_constraints[1]);
        this.weight_constraints = new Vector2(weight_constraints[0], weight_constraints[1]); ;

        this.grown_p = 0f;

        UpdateAppearance();
    }


    public float Get_Actual_Attr_Value(float p, Vector2 constraints)
    {
        return Mathf.Lerp(constraints[0], constraints[1], p) * this.grown_p;
    }

    public float Get_Actual_Weight()
    {
        print(weight_constraints[0]);
        print(weight_constraints[1]);
        print(1111111);
        return Get_Actual_Attr_Value(chromosome.weight_p, weight_constraints);
    }

    public float Get_Actual_Size()
    {
        return Get_Actual_Attr_Value(chromosome.size_p, size_constraints);
    }

    public float Get_Actual_Nutrients()
    {
        return chromosome.nut_p;
    }

    public void UpdateAppearance()
    {
        Color color = chromosome.color;
        float size = Get_Actual_Size();
        float weight = Get_Actual_Weight();
        //TODO FIX

        // Update the particle system
        var ps_main = ps.main;
        var ps_emission = ps.emission;
        var ps_shape = ps.shape;
        ps_main.startSize = size / 4f;
        ps_main.startSpeed = 3f * Mathf.Log(Mathf.Clamp(3f * size, 0.0001f, 1f));
        ps_emission.rateOverTime = weight;
        ps_shape.radius = size;

        // Updates the mesh
        transform.localScale = new Vector3(size, size, size) + Vector3.one * size_addon;
        foreach (MeshRenderer mr in mrs)
        {
            mr.material.color = color;
        }

        if(grown_p == 1)
        {
            anim.speed = 1f;
            anim.SetTrigger("fully_grown");
        }
        else
        {
            anim.speed = 0f;
            anim.Play("cabbage_bloom", -1, grown_p);
        }
    }

    public Dictionary<GeneticVector.TRAIT_ID, float> GetObservableGeneticDict()
    {
        Dictionary<GeneticVector.TRAIT_ID, float> dict = new Dictionary<GeneticVector.TRAIT_ID, float>();

        dict[GeneticVector.TRAIT_ID.SIZE] = Get_Actual_Size();
        dict[GeneticVector.TRAIT_ID.WEIGHT] = Get_Actual_Weight();
        dict[GeneticVector.TRAIT_ID.NUT_P] = Get_Actual_Nutrients();
        dict[GeneticVector.TRAIT_ID.GROWN_P] = grown_p;

        return dict;
    }
}
