using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MarketEvent
{
    public float size_change;
    public float weight_change;
    public float nut_p_change;
    public float color_change;
    public float volatility_change;
    public float[] color_multipliers;
    public string flavour_text;
}

public class MarketEvents : MonoBehaviour
{
    public const int MEAN_DAYS_TIL_NEXT_EVENT = 15;
    public const int MEAN_DAYS_PER_EVENT = 6;
    public int time_until_event;
    public int active_event_days_remaining;
    public bool is_event_active;

    public MarketEvent active_event;
    public MarketEvent[] market_events;
    public Market market;

    void Start()
    {
        market = FindObjectOfType<Market>();
        is_event_active = false;
        active_event_days_remaining = 0;

        time_until_event = GenerateDiscretePoisson(MEAN_DAYS_TIL_NEXT_EVENT);

        // Don't start firing events until 5 days in.
        if (time_until_event < 5)
        {
            time_until_event = 5;
        }
    }

    public void AdvanceMarketState()
    {
        if (is_event_active)
        {
            active_event_days_remaining--;
            // if the event is over, revert the state of the market
            if (active_event_days_remaining == 0)
            {
                is_event_active = false;
                market.RevertMarketEvent(active_event);
            }
        }
        // if there is no even active and its time for the next even to fire, fire a random new event
        else if (time_until_event == 0)
        {
            active_event = market_events[Random.Range(0, market_events.Length)];
            market.DoMarketEvent(active_event);
            active_event_days_remaining = GenerateDiscretePoisson(MEAN_DAYS_PER_EVENT);
            is_event_active = true;

            // Start timer for next event (but don't start the next event until active_event_days_remaining = 0)
            time_until_event = GenerateDiscretePoisson(MEAN_DAYS_TIL_NEXT_EVENT);
            if (time_until_event <= active_event_days_remaining)
            {
                time_until_event += active_event_days_remaining - time_until_event + 2;
            }
        }

        time_until_event--;
        market.AdvanceTimestep();
    }


    private int GenerateDiscretePoisson(float lambda)
    {
        float L = Mathf.Exp(-lambda);
        float p = 1.0f;
        int k = 0;

        while (p > L)
        {
            k++;
            p *= Random.value;
        }
        return k - 1;
    }

}
