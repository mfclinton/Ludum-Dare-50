using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandPlot : MonoBehaviour
{
    public Cabbage cabbage;

    public void SetCabbage(Cabbage cabbage)
    {
        this.cabbage = cabbage;
    }


    public void ClearPlot()
    {
        SetCabbage(null);
    }


    public void UpdatePlotVisual()
    {

    }
}
