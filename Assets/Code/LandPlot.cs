using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandPlot : MonoBehaviour
{
    public Cabbage cabbage;
    public float growth_gain; // Days To Grow = 1/growth_gain

    public void SetCabbage(Cabbage cabbage)
    {
        this.cabbage = cabbage;

        if (cabbage != null)
        {
            cabbage.gameObject.transform.position = transform.position + Vector3.up * 0.5f;
            cabbage.plot = GetComponent<LandPlot>();
        }
            
    }


    public void ClearPlot()
    {
        SetCabbage(null);
    }


    public void UpdatePlotVisual()
    {

    }
}
