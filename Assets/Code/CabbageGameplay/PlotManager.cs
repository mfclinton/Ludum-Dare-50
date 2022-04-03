using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlotManager
{

    private static List<PlotClass> plotClasses = new List<PlotClass>();


    public enum HighlightTypes {
        NONE,
        FIRST,
        SECOND
    }

    public static List<PlotClass> GetPlots() {
        //foreach(PlotClass plot in plots){
        //    plot.HighlightPlot(plot.);
        //}
        return plotClasses;
    }

    public static void AddPlot(PlotClass _plotClass) {
        plotClasses.Add(_plotClass);
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
