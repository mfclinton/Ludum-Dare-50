using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabbage : MonoBehaviour
{
    public GeneticVector chromosome;
    public float grown_p, max_size, max_weight;

    private MeshRenderer[] mrs;
    private ParticleSystem ps;

    private void Awake()
    {
        mrs = GetComponentsInChildren<MeshRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
    }


    private void Update()
    {
        UpdateAppearance();
    }


    public void Set(GeneticVector chromosome, float max_size, float max_weight)
    {
        this.chromosome = chromosome;
        this.max_size = max_size;
        this.max_weight = max_weight;

        this.grown_p = 1f; // TODO : Temp

        UpdateAppearance();
    }


    public void UpdateAppearance()
    {
        Color color = chromosome.color;
        float size = chromosome.size_p * this.max_size * this.grown_p;
        float weight = chromosome.weight_p * this.max_weight * this.grown_p;

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
}
