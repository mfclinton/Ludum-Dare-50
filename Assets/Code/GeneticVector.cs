using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneticVector
{
    // Traits
    public enum TRAIT_ID { SIZE, WEIGHT, NUT_P, COLOR };
    public float size, weight, nut_p;
    public Color color;

    public GeneticVector(float size, float weight, float nut_p, Color color)
    {
        this.size = size;
        this.weight = weight;
        this.nut_p = nut_p;
        this.color = color;
    }


    // Cross Over Functions 
    public GeneticVector CrossOver(GeneticVector other, float p, float mut_r, Vector2 size_constraint, Vector2 weight_constraint)
    {
        (float new_size, bool size_mut) = FloatCrossOver(size, other.size, p, mut_r, size_constraint);
        (float new_weight, bool weight_mut) = FloatCrossOver(weight, other.weight, p, mut_r, weight_constraint);
        (float new_nut_p, bool nut_p_mut) = FloatCrossOver(nut_p, other.nut_p, p, mut_r, new Vector2(0f, 1f));

        (Color new_color, bool color_mut) = ColorCrossOver(color, other.color, p, mut_r);

        return new GeneticVector(new_size, new_weight, new_nut_p, new_color);
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
            if (size < 1f)
                return "SMALL";
            else if (size < 2f)
                return "MEDIUM";
            else
                return "LARGE";
        }
        else if (t_id == TRAIT_ID.WEIGHT)
        {
            if (size < 0.5f)
                return "LIGHT";
            else if (size < 1f)
                return "MEDIAL";
            else
                return "HEAVY";
        }
        else if (t_id == TRAIT_ID.NUT_P)
        {
            // TODO
            if (size < 0.5f)
                return "UNHEALTHY";
            else if (size < 0.8f)
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
