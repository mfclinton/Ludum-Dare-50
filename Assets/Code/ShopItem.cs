using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopItem : MonoBehaviour
{
    public float cost;
    public UnityEvent invoked_method;
    GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void Buy()
    {
        if (gm.Buy(cost))
        {
            invoked_method.Invoke();
        }
    }
}
