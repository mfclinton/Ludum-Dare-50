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



    void SelectCabbageCheck() {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = ~LayerMask.GetMask("Plot"); // 1 << 1;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;

        Ray ray = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100, layerMask)) {
            Transform objectHit = hit.transform;

            print("hit!");
            // Do something with the object that was hit by the raycast.

            // focus cabbage or empty plot
            PlotClass plotClass = hit.transform.gameObject.GetComponent<PlotClass>();
            if (plotClass.attachedCabbage) {
                tooltipClass.ShowCabbageTooltip(plotClass.attachedCabbage);
            }
            else {
                tooltipClass.ShowPlotTooltip();
            }
        
        }
        else {
            tooltipClass.HideTooltip();
        }

    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0)) {
            SelectCabbageCheck();
        }



    }
}
