using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneticVector
{
    // Traits
    public enum TRAIT_ID { SIZE, WEIGHT, NUT_P, COLOR, GROWN_P };
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
    public GeneticVector CrossOver(GeneticVector other, float mut_r, float genetic_var_p)
    {
        float p = Random.value;
        (float new_size_p, bool size_mut) = FloatCrossOver(size_p, other.size_p, p, mut_r, genetic_var_p, new Vector2(min_p, 1f));

        // Shares the stratification of weight and size?
        //p = Random.value;
        (float new_weight_p, bool weight_mut) = FloatCrossOver(weight_p, other.weight_p, p, mut_r, genetic_var_p, new Vector2(min_p, 1f));

        p = Random.value;
        (float new_nut_p, bool nut_p_mut) = FloatCrossOver(nut_p, other.nut_p, p, mut_r, genetic_var_p, new Vector2(min_p, 1f));

        p = Random.value;
        (Color new_color, bool color_mut) = ColorCrossOver(color, other.color, p, mut_r, genetic_var_p);

        return new GeneticVector(new_size_p, new_weight_p, new_nut_p, new_color);
    }

    public static GeneticVector GenerateRandomGenetics()
    {
        return new GeneticVector(Random.value, Random.value, Random.value, new Color(Random.value, Random.value, Random.value));
    }


    public (float, bool) FloatCrossOver(float self, float other, float p, float mut_r, float genetic_var_p, Vector2 constraint)
    {
        float min_value = Mathf.Clamp(Mathf.Min(self, other) - genetic_var_p, constraint[0], constraint[1]);
        float max_value = Mathf.Clamp(Mathf.Max(self, other) + genetic_var_p, constraint[0], constraint[1]);

        float new_float = p * min_value + (1 - p) * max_value;

        bool mutated = false;
        if (Random.value <= mut_r)
        {
            new_float = Random.Range(constraint[0], constraint[1]);
        }

        return (new_float, mutated);
    }


    public (Color, bool) ColorCrossOver(Color self, Color other, float p, float mut_r, float genetic_var_p)
    {
        Vector4 a = self;
        Vector4 b = other;

        Vector4 new_a = Vector4.Scale((Vector4.one - a), (Vector4.one - a));
        Vector4 new_b = Vector4.Scale((Vector4.one - b), (Vector4.one - b));

        // Note : adding offsets will impact alpha
        // Vector4 new_a_offset = (new_a - new_b).normalized * genetic_var_p;
        // Vector4 new_b_offset = (new_b - new_a).normalized * genetic_var_p;
        Vector4 new_a_offset = Random.onUnitSphere * genetic_var_p;
        Vector4 new_b_offset = Random.onUnitSphere * genetic_var_p;

        Vector4 mixed_value = (new_a + new_a_offset) * p + (new_b + new_b_offset) * (1-p);
        Color new_color = new Color();
        for (int i = 0; i < 4; i++)
        {
            new_color[i] = Mathf.Clamp01(1f - Mathf.Sqrt(Mathf.Max(0f,mixed_value[i])));
        }

        bool mutated = false;
        if (Random.value <= mut_r)
        {
            new_color = new Color(Random.value, Random.value, Random.value);
        }

        return (new_color, mutated);
    }
}
