using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
// using Unity.Collections;
using UnityEngine;


public class CreateLandPlots : MonoBehaviour
{

    public GameObject landPlotPrefab;

    [ReadOnly]
    public int plotNum = 8;

    // Start is called before the first frame update
    void Start() {

        // GameObject landPlotPrefab = gameObject;

        for (int i = 1; i < plotNum; i++) {

            GameObject newPlot = Instantiate(landPlotPrefab);
            //newPlot.transform.parent = gameObject.transform.parent.Find("StaticHolder").Find("PlotPrefab");
            newPlot.transform.parent = GameObject.Find("Plots").transform;
            //float xOfVec = i * 4;
            //if (i >= 4) {
            //    xOfVec = (i % 4) * 4;
            //}
            //print(xOfVec);
            newPlot.transform.position = landPlotPrefab.transform.position + new Vector3((i % 4) * 3, 0, (i / 4 - ((i % 4) / 4)) * -3);

        }

    }


    // Update is called once per frame
    void Update(){
        
    }
}
