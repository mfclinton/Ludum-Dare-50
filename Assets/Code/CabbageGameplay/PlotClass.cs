using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotClass : MonoBehaviour
{


    [ReadOnly] public int nodeId;
    [ReadOnly] public GameObject attachedCabbage;

    //public enum HighlightTypes {
    //    NONE,
    //    FIRST,
    //    SECOND
    //}
    //using PlotManager.HighlightTypes as HighlightTypes;


    //private PlotManager.HighlightTypes highlightColor = PlotManager.HighlightTypes.NONE;

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

    // called from tooltip
    public void HighlightPlot(PlotManager.HighlightTypes _highlightType) {
        if (_highlightType == PlotManager.HighlightTypes.FIRST) {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        if (_highlightType == PlotManager.HighlightTypes.SECOND) {
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
        if (_highlightType == PlotManager.HighlightTypes.NONE) {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
    }
    //public void UnhighlightPlot() {
    //    gameObject.GetComponent<Renderer>().material.color = Color.green;
    //}


    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
