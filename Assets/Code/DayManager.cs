using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DayManager : MonoBehaviour
{
    public Light sun;
    public float growth_anim_speed = 1f;
    public float sun_speed = 1f;
    public Color night;

    bool sun_setting = false;
    bool growing = false;
    bool sun_rising = false;
    EventAudio event_audio;

    Color day;
    List<Cabbage> cabbages;
    List<float> end_grown_p;
    GameManager gm;

    public void TriggerNight()
    {
        if (gm.game_over)
            return;

        if(!sun_setting && !growing && !sun_rising)
        {
            sun_setting = true;
            event_audio.PlayNextDaySound();
        }
    }

    void HandleNight()
    {
        if (sun_setting)
        {
            print("Sun Setting");
            sun_setting = InterpolateSun(night);
            if (!sun_setting)
                TriggerGrowth();
        }
        else if (growing)
        {
            print("Growing");
            growing = UpdateCabbageGrowth(cabbages, end_grown_p);
            if (!growing)
                sun_rising = true;
        }
        else if (sun_rising)
        {
            print("Sun Rising");
            sun_rising = InterpolateSun(day);
            if (!sun_rising)
            {
                gm.NextDay();
            }
        }
    }

    void TriggerGrowth()
    {
        growing = true;
        (cabbages, end_grown_p) = GetCabbageAndEndGrowths();
    }

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        event_audio = FindObjectOfType<EventAudio>();
        day = sun.color;
    }

    void Update()
    {
        HandleNight();
    }

    (List<Cabbage>, List<float>) GetCabbageAndEndGrowths()
    {
        LandPlot[] cabbaged_plots = FindObjectsOfType<LandPlot>().Where(lp => lp.cabbage != null).ToArray();
        
        List<float> end_grown_p = new List<float>();
        List<Cabbage> cabbages = new List<Cabbage>();

        foreach (LandPlot lp in cabbaged_plots)
        {
            Cabbage cabbage = lp.cabbage;
            cabbages.Add(cabbage);
            end_grown_p.Add(Mathf.Clamp01(lp.cabbage.grown_p + lp.growth_gain));
        }

        return (cabbages, end_grown_p);
    }

    // Returns true if all animations not done
    bool UpdateCabbageGrowth(List<Cabbage> cabbages, List<float> end_grown_p)
    {
        bool growing = false;
        for (int i = 0; i < cabbages.Count; i++)
        {
            Cabbage c = cabbages[i];
            float end_g_p = end_grown_p[i];

            c.grown_p = Mathf.Lerp(c.grown_p, end_g_p, Time.deltaTime * growth_anim_speed);
            c.UpdateAppearance();

            if (Mathf.Abs(c.grown_p - end_g_p) < 0.02f)
            {
                c.grown_p = end_g_p;
                c.UpdateAppearance();
            }
            else
                growing = true;
        }

        return growing;
    }

    // Returns true is sun is changing
    bool InterpolateSun(Color goal)
    {
        sun.color = Color.Lerp(sun.color, goal, Time.deltaTime * sun_speed);
        if(Vector4.Distance(sun.color, goal) < 0.1f)
        {
            sun.color = goal;
            return false;
        }

        return true;
    }
}
