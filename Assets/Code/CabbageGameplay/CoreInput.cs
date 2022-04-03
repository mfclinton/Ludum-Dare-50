using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreInput : MonoBehaviour
{

    private TooltipClass tooltipClass;

    // Start is called before the first frame update
    void Start()
    {
        tooltipClass = gameObject.GetComponent<TooltipClass>();
    }

    void FixedUpdate() {


    }



    void SelectCabbageCheck(int _whichTooltip) {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = ~LayerMask.GetMask("Plot"); // 1 << 1;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;

        Ray ray = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100, layerMask)) {
            Transform objectHit = hit.transform;

            //print("hit!");
            // Do something with the object that was hit by the raycast.

            // focus cabbage or empty plot
            GameObject plotObj = hit.transform.gameObject;
            PlotClass plotClass = plotObj.GetComponent<PlotClass>();
            //plotClass.HighlightPlot();
            //Color startColor = plotObj.GetComponent<Renderer>().material.color;

            if (plotClass.attachedCabbage) {
                tooltipClass.ShowCabbageTooltip(plotClass.attachedCabbage, _whichTooltip);
            }
            else {
                tooltipClass.ShowPlotTooltip(_whichTooltip);
            }
        
        }
        else {
            // do this on esc press instead
            //tooltipClass.HideTooltip();
        }

    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0)) {
            SelectCabbageCheck(1);
        }
        if (Input.GetMouseButtonDown(1)) {
            SelectCabbageCheck(2);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            //tooltipClass.HideTooltip(); // TODO deselect func
            // press ESC to hide the other (make esc key button popup)

            //print(tooltipClass.tooltipCabbage1);
            //print(tooltipClass.tooltipCabbage2);
            //print(tooltipClass.mostRecentlySetCabbageNonOverwrite);

            //print(tooltipClass.GetTooltipFromId(1).name == tooltipClass.mostRecentlySetCabbageNonOverwrite.name);
            //print(tooltipClass.GetTooltipFromId(2).name == tooltipClass.mostRecentlySetCabbageNonOverwrite.name);
            //print(tooltipClass.GetTooltipFromId(1).name, tooltipClass.GetTooltipFromId(2).name, tooltipClass.mostRecentlySetCabbageNonOverwrite.name);

            //if (tooltipClass.GetTooltipShownCount() == 1) {
            tooltipClass.HideTooltip();
            //}
            //else if (tooltipClass.GetTooltipFromId(1) != null && tooltipClass.GetTooltipFromId(1) == tooltipClass.mostRecentlySetCabbageNonOverwrite) {
            //    tooltipClass.HideTooltip(2);
            //    print("hide 2");
            //}
            //else if (tooltipClass.GetTooltipFromId(2) != null && tooltipClass.GetTooltipFromId(2) == tooltipClass.mostRecentlySetCabbageNonOverwrite) {
            //    tooltipClass.HideTooltip(1);
            //    print("hide 1");
            //}
            //else {
            //    // nothing to hide
            //}


        }


    }

    private void print(string name1, string name2, string name3) {
        print(name1 + " " + name2 + " " + name3);
    }
}
