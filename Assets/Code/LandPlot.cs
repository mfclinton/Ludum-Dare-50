using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandPlot : MonoBehaviour
{
    public Cabbage cabbage;

    public void SetCabbage(Cabbage cabbage)
    {
        this.cabbage = cabbage;

        if (cabbage != null)
            cabbage.gameObject.transform.position = transform.position + Vector3.up * 0.5f;
    }


    public void ClearPlot()
    {
        SetCabbage(null);
    }


    public void UpdatePlotVisual()
    {

    }
}
