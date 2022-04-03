using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabbage : MonoBehaviour
{
    public GeneticVector chromosome;
    public float p_grown; // Percent Grown

    private MeshRenderer[] mrs;
    private ParticleSystem ps;

    private void Start()
    {
        mrs = GetComponentsInChildren<MeshRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
    }


    private void Update()
    {
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
}
