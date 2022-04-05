using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TemporaryText : MonoBehaviour
{
    public float fade_time_length = 15f;
    float total_time_elapsed;
    TextMeshProUGUI text;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        total_time_elapsed = fade_time_length;

        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, 0);
    }

    public void TriggerFade(float cash_modifier, string msg)
    {
        string before_str = "\n$";
        if (cash_modifier < 0)
            before_str = "-" + before_str;
        string cash_str = before_str + Mathf.Abs(cash_modifier).ToString("0.00");

        text.text = msg + cash_str;
        total_time_elapsed = 0f;
    }

    public void HandleFade()
    {
        float p = 1f - Mathf.Clamp01(total_time_elapsed / fade_time_length);
        if (0f < p)
        {
            total_time_elapsed += Time.deltaTime;

            Color c = text.color;
            text.color = new Color(c.r, c.g, c.b, p);
        }
    }

    private void Update()
    {
        HandleFade();
    }
}
