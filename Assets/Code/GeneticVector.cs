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

        // Shares the stratification of weight and size?
        p = Random.value;
        (float new_weight_p, bool weight_mut) = FloatCrossOver(weight_p, other.weight_p, p, mut_r, new Vector2(min_p, 1f));

        p = Random.value;
        (float new_nut_p, bool nut_p_mut) = FloatCrossOver(nut_p, other.nut_p, p, mut_r, new Vector2(min_p, 1f));

        p = Random.value;
        (Color new_color, bool color_mut) = ColorCrossOver(color, other.color, p, mut_r);

        return new GeneticVector(new_size_p, new_weight_p, new_nut_p, new_color);
    }

    public static GeneticVector GenerateRandomGenetics()
    {
        return new GeneticVector(Random.value, Random.value, Random.value, new Color(Random.value, Random.value, Random.value));
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
        Vector3 self_color_vec = new Vector3(self.r, self.g, self.b) * 255;
        Vector3 other_color_vec = new Vector3(other.r, other.g, other.b) * 255;
        Vector3 full_color_vec = new Vector3(255f, 255f, 255f);

        Vector3 self_color_sqred = Vector3.Scale((full_color_vec - self_color_vec), (full_color_vec - self_color_vec));
        Vector3 other_color_sqred = Vector3.Scale((full_color_vec - other_color_vec), (full_color_vec - other_color_vec));
        Vector3 unsqrrooted_value = self_color_sqred * p + other_color_sqred * (1-p);

        Color new_color = new Color((255f - Mathf.Sqrt(unsqrrooted_value[0])) / 255f,
            (255f - Mathf.Sqrt(unsqrrooted_value[1])) / 255f,
            (255f - Mathf.Sqrt(unsqrrooted_value[2])) / 255f);

        bool mutated = false;
        if (Random.value <= mut_r)
        {
            new_color = new Color(Random.value, Random.value, Random.value);
        }

        return (new_color, mutated);
    }


    public Dictionary<TRAIT_ID, float> GetIDToObsValueDict()
    {
        Dictionary<TRAIT_ID, float> dict = new Dictionary<TRAIT_ID, float>();

        dict[TRAIT_ID.SIZE] = size_p;
        dict[TRAIT_ID.WEIGHT] = weight_p;
        dict[TRAIT_ID.NUT_P] = nut_p;

        return dict;
    }
}
