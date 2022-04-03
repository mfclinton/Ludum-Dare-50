using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipClass : MonoBehaviour
{

    [ReadOnly] public GameObject tooltipCabbage1;
    [ReadOnly] public GameObject tooltipCabbage2;

    [ReadOnly] public GameObject mostRecentlySetCabbageNonOverwrite;

    private Dictionary<int, GameObject> tooltipDict = new Dictionary<int, GameObject>() {
        //[1] = tooltipCabbage1,
        //[2] = tooltipCabbage2
    };

    // should be named cabbage...
    public GameObject GetTooltipFromId(int _id) {
        return tooltipDict[_id];
    }
    private void SetTooltipFromId(int _id, GameObject _val) {
        //tooltipDict[_id] = _val;
        // bruh moment, idk if u can get refs
        if (_id == 1) {
            tooltipCabbage1 = _val;
            tooltipDict[_id] = _val;
        }
        if (_id == 2) {
            tooltipCabbage2 = _val;
            tooltipDict[_id] = _val;
        }
    }

    public int GetTooltipShownCount() {
        if (IsNoCabbageShown()) {
            return 0;
        } else if (IsOnlyFirstCabbageShown()) {
            return 1;
        } else if (IsOnlySecondCabbageShown()) {
            return 1;
        }
        else {
            return 2;
        }
    }



    private void Start() {
        tooltipDict[1] = tooltipCabbage1;
        tooltipDict[2] = tooltipCabbage2;
    }


    private enum TooltipPopupType {
        NOTHING,
        PLOT,
        CABBAGE,
        GENETICS
    }


    private bool IsNoCabbageShown() {
        return (!tooltipCabbage1 && !tooltipCabbage2);
    }

    private bool IsOnlyFirstCabbageShown() {
        return (!tooltipCabbage2 && tooltipCabbage1);
    }

    private bool IsOnlySecondCabbageShown() {
        return (tooltipCabbage2 && !tooltipCabbage1);
    }

    private bool HideCabbageIfAlreadyShown(GameObject _cabbageObj) {

        if (IsNoCabbageShown()) {
            mostRecentlySetCabbageNonOverwrite = null;
        }

        if (tooltipCabbage1 == _cabbageObj) {
            HideTooltip(1);
            return true;
        }
        if (tooltipCabbage2 == _cabbageObj) {
            HideTooltip(2);
            return true;
        }
        return false;
    }


    //void ShowCabbageTooltipHelper(int idToUse, GameObject _cabbageObj) {
    //}

    public void ShowCabbageTooltip(GameObject _cabbageObj, int _whichTooltipId = 0, bool _shouldHighlight = true) {
        //print("TODO show cabbage popup");

        if (HideCabbageIfAlreadyShown(_cabbageObj)) {
            return;
        }

        int idToUse = _whichTooltipId;
        bool shouldShowCabbage = false;
        // show on second
        if (IsOnlyFirstCabbageShown()) {
            if (idToUse == 0) {
                idToUse = 2;
            }
            shouldShowCabbage = true;
        }

        // show on first
        else if (IsOnlySecondCabbageShown() || IsNoCabbageShown()) {
            if (idToUse == 0) {
                idToUse = 1;
            }
            shouldShowCabbage = true;
        }

        // if both full
        else {


            if (mostRecentlySetCabbageNonOverwrite == tooltipCabbage2) {
                if (idToUse == 0) {
                    idToUse = 2;
                }
                shouldShowCabbage = true;
            }

            else if (mostRecentlySetCabbageNonOverwrite == tooltipCabbage1) {
                if (idToUse == 0) {
                    idToUse = 1;
                }
                shouldShowCabbage = true;
            }
            else {
                print("why is there no valid cabbage to overwrite tooltip");
            }
        }
        // assume most recent should be replaced

        if (shouldShowCabbage) {
            if (_shouldHighlight) {
                //print("highligth!" + idToUse);
                SetTooltipFromId(idToUse, _cabbageObj);
                ShowTooltipInternal(TooltipPopupType.CABBAGE, idToUse);
                mostRecentlySetCabbageNonOverwrite = GetTooltipFromId(idToUse);
            }
            else {
                // visibly show first tooltip 
                if ( (GetTooltipFromId(1) == null 
                    && GetTooltipFromId(2) != _cabbageObj)
                    || (GetTooltipFromId(2) == null
                    && GetTooltipFromId(1) != _cabbageObj)
                ) {
                    //print("preview!" + idToUse);
                    ShowTooltipInternal(TooltipPopupType.CABBAGE, idToUse, _shouldHighlight);
                }
            }
        }

    }
    public void HideTooltip(int _optTooltipID = 0) {
        ShowTooltipInternal(TooltipPopupType.NOTHING, _optTooltipID);
    }

    // TODO right click to deselect? some way to deselect by presing tooltip in easy UX manner


    public void ShowGenetics() {
        // only use top tooltip for now
    }


    private void ShowTooltipInternal(TooltipPopupType _whatToShow, int _optTooltipID = 0, bool _shouldHighlight = true) { // TODO pass cabbage class not gameobject?
        if (_whatToShow == TooltipPopupType.PLOT) {
            // assume accuracy, show on first
            //print("todo show plot tooltip on top tooltip");
        }

        if (_whatToShow == TooltipPopupType.CABBAGE) {
            //print("todo show tooltip for cabbage on given page");
        }

        if (_whatToShow == TooltipPopupType.GENETICS) {
            //print("todo show tooltip for cabbage on given page");
        }

        if (_whatToShow == TooltipPopupType.NOTHING) {
            //print("todo replace tooltip with empty, or hide it idk");
            if (_optTooltipID != 0) {
                SetTooltipFromId(_optTooltipID, null);
            }
            else {
                SetTooltipFromId(1, null);
                SetTooltipFromId(2, null);
                // TODO visuals
            }
        }

        if (_shouldHighlight) {
            foreach (PlotClass plot in PlotManager.GetPlots()) {
                if (plot.attachedCabbage == tooltipCabbage1) {
                    plot.HighlightPlot(PlotManager.HighlightTypes.FIRST);
                }
                else if (plot.attachedCabbage == tooltipCabbage2) {
                    plot.HighlightPlot(PlotManager.HighlightTypes.SECOND);
                }
                else {
                    plot.HighlightPlot(PlotManager.HighlightTypes.NONE);
                }
            }
        }
    }

    public void ShowPlotTooltip(int _whichTooltipId) {

        print("TODO show plot popup");
        if (IsNoCabbageShown()) {
            ShowTooltipInternal(TooltipPopupType.PLOT);
        }

    }

}
