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
    public List<GeneticVector> seeds;
    public List<int> seed_ids;

    public float mut_r, max_size, max_weight;
    int next_id;

    InputManager input_mng;
    UIManager uim;
    Market market;

    // Data Tracking
    Dictionary<int, List<float>> sales;

    private void Start()
    {
        input_mng = FindObjectOfType<InputManager>();
        market = FindObjectOfType<Market>();
        uim = FindObjectOfType<UIManager>();

        sales = new Dictionary<int, List<float>>();
        seeds = new List<GeneticVector>();
        seed_ids = new List<int>();
        next_id = 0;

        uim.Update_Cash(cash);
        uim.Update_Day(day);
    }

    public void NextDay()
    {
        day += 1;
        uim.Update_Day(day);
        uim.Update_Cash(cash);
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


    public (GeneticVector, int) Splice_Selected()
    {
        List<GeneticVector> chromosomes = input_mng.GetSelectedCabbages()
            .Select(cabbage => cabbage.chromosome).ToList();

        print(chromosomes.Count);
        if (chromosomes.Count < 2)
            return (null, -1);

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
        int index = seeds.Count;
        seed_ids.Insert(index, next_id);
        next_id++;
        seeds.Insert(index, new_chromosome);
        return (new_chromosome, next_id - 1);
    }

    public void Plant_Cabbage(int seed_id, LandPlot plot)
    {
        int index = seed_ids.Select((seed_id, idx) => new { id = seed_id, i = idx }).First(obj => obj.id == seed_id).i;
        GeneticVector new_chromosome = seeds[index];
        Cabbage new_cabbage = Create_Cabbage(new_chromosome);

        plot.SetCabbage(new_cabbage);
        seeds.RemoveAt(index);
        seed_ids.RemoveAt(index);

        uim.Destroy_Seed();
    }

    public Cabbage Create_Cabbage(GeneticVector new_chromosome)
    {
        GameObject new_cabbage_model = Instantiate(cabbage_model_prefab);
        Cabbage new_cabbage = new_cabbage_model.AddComponent<Cabbage>();
        new_cabbage.Set(new_chromosome, max_size, max_weight);
        return new_cabbage;
    }

    public void Sell(LandPlot plot)
    {
        Cabbage c = plot.cabbage;
        float price = Get_Price(c);
        cash += price;

        if (!sales.ContainsKey(day))
            sales[day] = new List<float>();

        sales[day].Add(price);

        plot.ClearPlot();
        Destroy(c.gameObject);

        uim.Update_Cash(cash);
    }

    public float Get_Price(Cabbage c)
    {
        float price = market.DetermineCabbageValue(c);
        return price;
    }
}
