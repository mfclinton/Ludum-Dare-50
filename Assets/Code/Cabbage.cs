using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabbage : MonoBehaviour
{
    public GeneticVector chromosome;
    public float mut_r;
    public Vector2 size_constraint, weight_constraint;

    private MeshRenderer[] mrs;
    private ParticleSystem ps;

    private void Start()
    {
        mrs = GetComponentsInChildren<MeshRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
    }


    public void Set(GeneticVector chromosome, float mut_r, Vector2 size_constraint, Vector2 weight_constraint)
    {
        this.chromosome = chromosome;
        this.size_constraint = size_constraint;
        this.weight_constraint = weight_constraint;
        this.mut_r = mut_r;

        UpdateAppearance();
    }


    public void UpdateAppearance()
    {
        Color color = chromosome.color;
        float size = chromosome.size;
        float weight = chromosome.weight;

        // Update the particle system
        var ps_main = ps.main;
        var ps_emission = ps.emission;
        var ps_shape = ps.shape;
        ps_main.startSize = size / 4f;
        ps_main.startSpeed = 3f * Mathf.Log(3f * size);
        ps_emission.rateOverTime = weight;
        ps_shape.radius = size;

        // Updates the mesh
        transform.localScale = new Vector3(size, size, size);
        foreach (MeshRenderer mr in mrs)
        {
            mr.material.color = color;
        }
    }


    public void CrossBreed(Cabbage other)
    {
        GeneticVector new_chromosome = chromosome.CrossOver(other.chromosome, Random.value, mut_r, size_constraint, weight_constraint);

        // TODO
        float new_mut_r = 0f;
        Vector2 new_size_constraint = Vector2.zero;
        Vector2 new_weight_constraint = Vector2.zero;
        
        Cabbage new_cabbage = null;
        new_cabbage.Set(new_chromosome, new_mut_r, new_size_constraint, new_weight_constraint);
    }
}
