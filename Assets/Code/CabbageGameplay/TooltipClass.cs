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
    private GameObject GetTooltipFromId(int _id) {
        return tooltipDict[_id];
    }
    private void SetTooltipFromId(int _id, GameObject _val) {
        //tooltipDict[_id] = _val;
        // bruh moment, idk if u can get refs
        if (_id == 1) {
            tooltipCabbage1 = _val;
        }
        if (_id == 2) {
            tooltipCabbage2 = _val;
        }
    }


    private void Start() {
        tooltipDict[1] = tooltipCabbage1;
        tooltipDict[2] = tooltipCabbage2;
    }


    private enum TooltipPopupType {
        NOTHING,
        PLOT,
        CABBAGE
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


    public void ShowCabbageTooltip(GameObject _cabbageObj) {
        //print("TODO show cabbage popup");

        if (HideCabbageIfAlreadyShown(_cabbageObj)) {
            return;
        }

        // show on second
        if (IsOnlyFirstCabbageShown()) {

            tooltipCabbage2 = _cabbageObj;
            ShowTooltipInternal(TooltipPopupType.CABBAGE, 2);
            mostRecentlySetCabbageNonOverwrite = tooltipCabbage2;
        }

        // show on first
        else if (IsOnlySecondCabbageShown() || IsNoCabbageShown()) {

            tooltipCabbage1 = _cabbageObj;
            ShowTooltipInternal(TooltipPopupType.CABBAGE, 1);
            mostRecentlySetCabbageNonOverwrite = tooltipCabbage1;
        }

        // if both full
        else {


            if (mostRecentlySetCabbageNonOverwrite == tooltipCabbage1) {
                tooltipCabbage2 = _cabbageObj;
                ShowTooltipInternal(TooltipPopupType.CABBAGE, 1);
            }

            else if (mostRecentlySetCabbageNonOverwrite == tooltipCabbage2) {
                tooltipCabbage1 = _cabbageObj;
                ShowTooltipInternal(TooltipPopupType.CABBAGE, 2);
            }
            else {
                print("why is there no valid cabbage to overwrite tooltip");
            }
        }
        // assume most recent should be replaced

    }
    public void HideTooltip(int _optTooltipID = 0) {
        ShowTooltipInternal(TooltipPopupType.NOTHING, _optTooltipID);
    }

    // TODO right click to deselect? some way to deselect by presing tooltip in easy UX manner


    private void ShowTooltipInternal(TooltipPopupType _whatToShow, int _optTooltipID = 0) { // TODO pass cabbage class not gameobject?
        if (_whatToShow == TooltipPopupType.PLOT) {
            // assume accuracy, show on first
            print("todo show plot tooltip on top tooltip");
        }

        if (_whatToShow == TooltipPopupType.CABBAGE) {
            print("todo show tooltip for cabbage on given page");
        }

        if (_whatToShow == TooltipPopupType.NOTHING) {
            print("todo replace tooltip with empty, or hide it idk");
            if (_optTooltipID != 0) {
                SetTooltipFromId(_optTooltipID, null);
            } else {
                SetTooltipFromId(1, null);
                SetTooltipFromId(2, null);
                // TODO visuals
            }
        }
    }


    public void ShowPlotTooltip() {

        print("TODO show plot popup");
        if (IsNoCabbageShown()) {
            ShowTooltipInternal(TooltipPopupType.PLOT);
        }

    }

}
