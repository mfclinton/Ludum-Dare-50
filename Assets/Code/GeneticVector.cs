using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneticVector
{
    public object[] genes;

    /// <summary>
    /// Creates an offspring from two genetic vectors
    /// </summary>
    /// <param name="other">Breeding partner's genes</param>
    /// <returns>A new crossbred genetic vector</returns>
    public GeneticVector CrossBreed(GeneticVector other)
    {
        // TODO
        return null;
    }

    /// <summary>
    /// Mutates the vector inplace
    /// </summary>
    /// <param name="mut_rate">Mutation rate</param>
    public void Mutate(float mut_rate)
    {
        // TODO
    }
}
