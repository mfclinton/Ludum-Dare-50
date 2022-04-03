using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotClass : MonoBehaviour
{


    [ReadOnly] public int nodeId;
    [ReadOnly] public GameObject attachedCabbage;

    // TODO attach UI element and do logic here, although it should be in a tooltip class later
    //[ReadOnly] public GameObject tooltip1;
    //[ReadOnly] public GameObject tooltip2;


    // better than start
    public void InitPlot(int _nodeId) {
        nodeId = _nodeId;
    }

    public void AddCabbage(GameObject _cabbage) {
        attachedCabbage = _cabbage;
    }



    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
