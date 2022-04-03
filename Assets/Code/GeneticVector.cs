using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneticVector
{
    // Traits
    public enum TRAIT_ID { SIZE, WEIGHT, NUT_P, COLOR };
    public float size_p, weight_p, nut_p;
    public Color color;

    float min_p = 0.01f;

    public GeneticVector(float size, float weight, float nut_p, Color color)
    {
        this.size_p = size;
        this.weight_p = weight;
        this.nut_p = nut_p;
        this.color = color;
    }


    // Cross Over Functions 
    public GeneticVector CrossOver(GeneticVector other, float mut_r)
    {
        float p = Random.value;
        (float new_size_p, bool size_mut) = FloatCrossOver(size_p, other.size_p, p, mut_r, new Vector2(min_p, 1f));

        // Shares the stratification of weight and size
        // p = Random.value;
        (float new_weight_p, bool weight_mut) = FloatCrossOver(weight_p, other.weight_p, p, mut_r, new Vector2(min_p, 1f));

        p = Random.value;
        (float new_nut_p, bool nut_p_mut) = FloatCrossOver(nut_p, other.nut_p, p, mut_r, new Vector2(min_p, 1f));

        p = Random.value;
        (Color new_color, bool color_mut) = ColorCrossOver(color, other.color, p, mut_r);

        return new GeneticVector(new_size_p, new_weight_p, new_nut_p, new_color);
    }


    public (float, bool) FloatCrossOver(float self, float other, float p, float mut_r, Vector2 constraint)
    {
        float new_float = p * self + (1 - p) * other;

        bool mutated = false;
        if (Random.value <= mut_r)
        {
            new_float = Random.Range(constraint[0], constraint[1]);
        }

        return (new_float, mutated);
    }


    public (Color, bool) ColorCrossOver(Color self, Color other, float p, float mut_r)
    {
        Color new_color = p * self + (1 - p) * other;

        bool mutated = false;
        if(Random.value <= mut_r)
        {
            new_color = new Color(Random.value, Random.value, Random.value);
        }

        return (new_color, mutated);
    }


    public string GetTraitClassification(TRAIT_ID t_id)
    {
        if(t_id == TRAIT_ID.SIZE)
        {
            if (size_p < 1f)
                return "SMALL";
            else if (size_p < 2f)
                return "MEDIUM";
            else
                return "LARGE";
        }
        else if (t_id == TRAIT_ID.WEIGHT)
        {
            if (weight_p < 0.5f)
                return "LIGHT";
            else if (weight_p < 1f)
                return "MEDIAL";
            else
                return "HEAVY";
        }
        else if (t_id == TRAIT_ID.NUT_P)
        {
            // TODO
            if (nut_p < 0.5f)
                return "UNHEALTHY";
            else if (nut_p < 0.8f)
                return "DECENT";
            else
                return "NUTRITIOUS";
        }
        else if (t_id == TRAIT_ID.COLOR)
        {
            return "Color is unsupported for logging";
        }

        return "NaN";
    }
}
