using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabbage : MonoBehaviour
{
    public GeneticVector chromosome;
    public float grown_p, max_size, max_weight;
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


    public void Set(GeneticVector chromosome, float max_size, float max_weight)
    {
        this.chromosome = chromosome;
        this.max_size = max_size;
        this.max_weight = max_weight;

        this.grown_p = 0f;

        UpdateAppearance();
    }


    public void UpdateAppearance()
    {
        Color color = chromosome.color;
        float size = chromosome.size_p * this.max_size * this.grown_p;
        float weight = chromosome.weight_p * this.max_weight * Mathf.Clamp(this.grown_p, 0.2f, 1f);

        // Update the particle system
        var ps_main = ps.main;
        var ps_emission = ps.emission;
        var ps_shape = ps.shape;
        ps_main.startSize = size / 4f;
        ps_main.startSpeed = 3f * Mathf.Log(3f * size);
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
            print("GROWN");
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
        Dictionary<GeneticVector.TRAIT_ID, float> dict = chromosome.GetIDToObsValueDict();
        // dict[GeneticVector.TRAIT_ID.SIZE] = (max_size * (float) dict[GeneticVector.TRAIT_ID.SIZE]);
        dict[GeneticVector.TRAIT_ID.WEIGHT] = (max_weight * (float) dict[GeneticVector.TRAIT_ID.WEIGHT] * grown_p);
        dict[GeneticVector.TRAIT_ID.GROWN_P] = grown_p;

        return dict;
    }
}
