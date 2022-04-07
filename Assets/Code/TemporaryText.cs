using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TemporaryText : MonoBehaviour
{
    public float fade_time_length = 2f;
    public float fade_time_start = 5f;
    float total_time_elapsed;
    TextMeshProUGUI text;
    private bool fade_triggered;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        total_time_elapsed = 0f;
        fade_triggered = false;

        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, 0);

        total_time_elapsed = 0f;
    }

    public void TriggerFade(float cash_modifier, string msg)
    {
        string before_str = "$";
        if (cash_modifier < 0)
            before_str = "-" + before_str;
        else
            before_str = "+" + before_str;
        before_str = "Upkeep " + before_str;
        string cash_str = "\n" + before_str + Mathf.Abs(cash_modifier).ToString("0.00");

        text.text = msg + cash_str;
        fade_triggered = true;
        total_time_elapsed = 0f;
    }

    public void HandleFade()
    {
        float p = 1 - Mathf.Clamp01((total_time_elapsed - fade_time_start) / fade_time_length);
        // update if fade was triggered
        if (fade_triggered)
        {
            total_time_elapsed += Time.deltaTime;

            Color c = text.color;
            text.color = new Color(c.r, c.g, c.b, p);
        }
        // reset if fade is done
        if (p <= 0f)
        {
            fade_triggered = false;
            total_time_elapsed = 0f;
        }
        Debug.Log("p: " + p.ToString() + " time: " + total_time_elapsed.ToString());
    }

    public void HideUpkeepText()
    {
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, 0f);
        fade_triggered = false;
        total_time_elapsed = 0f;
    }

    private void Update()
    {
        HandleFade();
    }
}
