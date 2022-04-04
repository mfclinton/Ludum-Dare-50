using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ShopItem : MonoBehaviour
{
    public float cost;
    public TextMeshProUGUI cost_text;
    public UnityEvent invoked_method;

    GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();

        string pre_text = "";
        if (cost < 0f)
            pre_text = "-";

        cost_text.text = pre_text + "$" + Mathf.Abs(cost).ToString("0.0");
    }

    public void Buy()
    {
        if (gm.Buy(cost))
        {
            invoked_method.Invoke();
        }
    }
}
