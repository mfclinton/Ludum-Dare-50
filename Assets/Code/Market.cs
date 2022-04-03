using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market
{
    private float DetermineCabbageValue(Cabbage cabbage)
    {
        GeneticVector genetic_vector = cabbage.chromosome;

        float size = genetic_vector.size;
        float weight = genetic_vector.weight;
        float nut_p =  genetic_vector.nut_p; // Nutrition percentage
        Color color = genetic_vector.color;

        // TODO : THIS IS DAVE'S BLACKBOX TO OUTPUT A VALUE HERE
        float price = 0f;
        return 0f <= price ? price : 0f;
    }
}
