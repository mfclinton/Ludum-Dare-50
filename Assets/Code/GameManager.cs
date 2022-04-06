using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject cabbage_model_prefab;
    public int day, max_splices;
    public float cash;
    public List<GeneticVector> seeds;
    public List<int> seed_ids;

    public List<LandPlot> shop_regular_plots;
    public List<LandPlot> shop_gold_plots;

    public float mut_r, next_upkeep;
    public Vector2 size_constraints, weight_constraints, nutrition_constraints;
    int next_id, n_splices_today;

    public bool game_over;

    InputManager input_mng;
    UIManager uim;
    Market market;
    MarketEvents market_events;
    UpkeepEvents upkeep_events;
    EventAudio event_audio;
    BuyoutForecaster bf;
    HighScoreTracker hst;

    // Data Tracking
    Dictionary<int, List<float>> sales;

    private void Start()
    {
        input_mng = FindObjectOfType<InputManager>();
        market = FindObjectOfType<Market>();
        market_events = FindObjectOfType<MarketEvents>();
        uim = FindObjectOfType<UIManager>();
        upkeep_events = FindObjectOfType<UpkeepEvents>();
        event_audio = FindObjectOfType<EventAudio>();
        bf = FindObjectOfType<BuyoutForecaster>();
        hst = FindObjectOfType<HighScoreTracker>();

        sales = new Dictionary<int, List<float>>();
        seeds = new List<GeneticVector>();
        seed_ids = new List<int>();
        next_id = 0;

        GetNextUpkeep();

        n_splices_today = 0;

        uim.Update_N_Splices(n_splices_today, max_splices);
        uim.Update_Sellout_Text(bf.GetBuyoutAmount(sales));
        uim.Update_Cash(cash);
        uim.Update_Day(day);
        Disable_Shop_Stuff();
    }

    public void Disable_Shop_Stuff()
    {
        foreach (LandPlot lp in shop_regular_plots)
            lp.gameObject.SetActive(false);

        foreach (LandPlot lp in shop_gold_plots)
            lp.gameObject.SetActive(false);
    }

    public void Buy_Gold_Plot()
    {
        Buy_Plot(false);
    }

    public void Buy_Regular_Plot()
    {
        Buy_Plot(true);
    }

    public void Buy_One_Splice()
    {
        max_splices++;
        uim.Update_N_Splices(n_splices_today, max_splices);
    }

    public void Buy_Increase_Max_Weight()
    {
        weight_constraints[1] = weight_constraints[1] * 1.20f;
    }

    public void Buy_Plot(bool regular_plot)
    {
        List<LandPlot> plots;
        if (regular_plot)
            plots = shop_regular_plots;
        else
            plots = shop_gold_plots;

        LandPlot lp = plots[0];
        plots.RemoveAt(0);

        if (plots.Count == 0)
            uim.Disable_Plot_Sale(regular_plot);

        lp.gameObject.SetActive(true);
    }

    public void GetNextUpkeep()
    {
        UpkeepEntry upkeep_entry;
        (next_upkeep, upkeep_entry) = upkeep_events.GetTodaysCashChange(day == 0);
        uim.Update_Upkeep(next_upkeep, upkeep_entry);
    }

    public void ProcessUpkeep()
    {
        Update_Cash(next_upkeep);
    }

    public void NextDay()
    {
        if (game_over)
            return;

        ProcessUpkeep(); // CAN LOSE HERE

        day += 1;
        bool event_active_before = market_events.is_event_active;
        market_events.AdvanceMarketState();
        //if(event_active_before == false && market_events.is_event_active)
        //{
        //    // EVENT FIRED
        //}
        //else if (event_active_before == true && !market_events.is_event_active)
        //{
        //    // EVENT Endeded
        //    // TODO: FIND OUT IF EVENTS CAN BE BACK TO BACK OR IF THERE IS ALWAYS A TRANSITION
        //}

        if(market_events.is_event_active)
        {
            MarketEvent m_event = market_events.active_event;
            uim.Update_Event(m_event.flavour_text);
        }
        else
        {
            uim.Update_Event("NO NEWS TODAY");
        }

        GetNextUpkeep();

        n_splices_today = 0;
        uim.Update_N_Splices(n_splices_today, max_splices);
        uim.Update_Sellout_Text(bf.GetBuyoutAmount(sales));
        uim.Update_Day(day);
        uim.Update_Cash(cash);
        uim.Clear_All_Panels();
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
        if (max_splices <= n_splices_today)
            return (null, -1);

        IEnumerable<Cabbage> cabbages = input_mng.GetSelectedCabbages();
        List<GeneticVector> chromosomes = cabbages.Select(cabbage => cabbage.chromosome).ToList();

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
        
        n_splices_today++;
        uim.Update_N_Splices(n_splices_today, max_splices);

        //REMOVES THE CABBAGES AFTER BREEDING
        //foreach (Cabbage c in cabbages)
        //    ClearPlot(c.plot);
        // uim.Clear_All_Panels();

        event_audio.PlaySplicingSound();

        return AddNewSeed(new_chromosome);
    }

    public (GeneticVector, int) GenerateRandomSeed()
    {
        GeneticVector new_chromosome = GeneticVector.GenerateRandomGenetics();
        return AddNewSeed(new_chromosome);
    }

    public (GeneticVector, int) AddNewSeed(GeneticVector new_chromosome)
    {
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
        new_cabbage.Set(new_chromosome, size_constraints, weight_constraints);
        event_audio.PlayCabbagePlaceSound();
        return new_cabbage;
    }

    public void Update_Cash(float cash_change)
    {
        if (!sales.ContainsKey(day))
            sales[day] = new List<float>();

        cash += cash_change;
        sales[day].Add(cash_change);
        uim.Update_Cash(cash);

        if(cash < 0)
        {
            // GAME OVER
            game_over = true;
            event_audio.PlayLoseSound();
            uim.Game_Over();
            hst.UpdateHighScore(cash);
            uim.Update_HighScore_Text(hst.GetHighScore());
        }
    }

    public bool Buy(float cost)
    {
        if(0f <= cash + cost)
        {
            Update_Cash(cost);
            return true;
        }

        return false;
    }

    public void Sell(LandPlot plot)
    {
        Cabbage c = plot.cabbage;
        float price = Get_Price(c);
        Update_Cash(price);

        ClearPlot(plot);
        event_audio.PlaySellSound();
    }

    public void ClearPlot(LandPlot plot)
    {
        Cabbage c = plot.cabbage;
        plot.ClearPlot();
        Destroy(c.gameObject);
    }

    // TODO : BUY

    public float Get_Price(Cabbage c)
    {
        float price = market.DetermineCabbageValue(c);
        return price;
    }

    public void Sellout()
    {
        if (game_over)
            return;

        game_over = true;
        cash += bf.GetBuyoutAmount(sales);
        uim.Update_Cash(cash);
        uim.Game_Over();
        event_audio.PlaySelloutSound();
        hst.UpdateHighScore(cash);
        uim.Update_HighScore_Text(hst.GetHighScore());
    }
}
