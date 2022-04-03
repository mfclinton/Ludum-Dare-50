using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject cabbage_model_prefab;
    public int day;
    public float cash;

    public float mut_r, max_size, max_weight;

    InputManager input_mng;

    private void Start()
    {
        input_mng = FindObjectOfType<InputManager>();
    }

    // https://stackoverflow.com/questions/1952153/what-is-the-best-way-to-find-all-combinations-of-items-in-an-array
    static IEnumerable<IEnumerable<T>>
    GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
    {
        if (length == 1) return list.Select(t => new T[] { t });
        return GetKCombs(list, length - 1)
            .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                (t1, t2) => t1.Concat(new T[] { t2 }));
    }


    public void Splice_Selected()
    {
        List<GeneticVector> chromosomes = input_mng.GetSelectedCabbages()
            .Select(cabbage => cabbage.chromosome).ToList();

        print(chromosomes.Count);
        if (chromosomes.Count < 2)
            return;

        while(1 < chromosomes.Count)
        {
            IEnumerable<int> chromosome_indexes = chromosomes.Select((chromosome, i) => i);
            List<IEnumerable<int>> index_pairings = GetKCombs(chromosome_indexes, 2).ToList();

            List<GeneticVector> new_chromosomes = new List<GeneticVector>();
            foreach (IEnumerable<int> index_pair_enumer in index_pairings)
            {
                int[] index_pair = index_pair_enumer.ToArray();

                GeneticVector c0 = chromosomes[index_pair[0]];
                GeneticVector c1 = chromosomes[index_pair[1]];
                GeneticVector c2 = c0.CrossOver(c1, mut_r);

                new_chromosomes.Add(c2);
            }

            chromosomes = new_chromosomes;
        }

        GeneticVector new_chromosome = chromosomes[0];
        GameObject new_cabbage_model = Instantiate(cabbage_model_prefab);
        Cabbage new_cabbage = new_cabbage_model.AddComponent<Cabbage>();
        new_cabbage.Set(new_chromosome, max_size, max_weight);

        // TODO: do something with the new cabbage? Temp?
    }
}
