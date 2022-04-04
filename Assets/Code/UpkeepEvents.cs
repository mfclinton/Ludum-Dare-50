using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class UpkeepEntry
{
    public float cash_change;
    public string flavor_text;
}

public class UpkeepEvents : MonoBehaviour
{
    public float daily_cash_change;
    public UpkeepEntry[] upkeep_events;
    [Range(0f, 1f)]
    public float daily_event_chance;
    [Range(0f, 1f)]
    public float good_event_chance;

    public (float, UpkeepEntry) GetTodaysCashChange(bool ignore_events)
    {
        UpkeepEntry e = null;
        if(!ignore_events)
            e = RollEvent();

        if (e != null)
        {
            float new_cash_change = e.cash_change + daily_cash_change;

            if (new_cash_change < 0f)
                daily_cash_change = e.cash_change + daily_cash_change;
            else
                e = null;
        }

        return (daily_cash_change, e);
    }

    public UpkeepEntry RollEvent()
    {
        UpkeepEntry e = null;
        if (Random.value < daily_event_chance)
        {
            // EVENT FIRED
            bool is_good = Random.value < good_event_chance;
            UpkeepEntry[] events;

            if (is_good)
                events = upkeep_events.Where(ue => 0f <= ue.cash_change).ToArray();
            else
                events = upkeep_events.Where(ue => ue.cash_change < 0f).ToArray();

            e = events[Random.Range(0, events.Length)];
        }

        return e;
    }
}
