using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public int max_selected = 2;
    public Camera cam;

    UIManager ui;
    int selected_index_pointer;
    List<LandPlot> selected_plots;

    void Start()
    {
        // cam = Camera.main;
        ui = FindObjectOfType<UIManager>();

        ClearSelected();
    }


    private void Update()
    {
        HandleUserInput();
    }


    void HandleUserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            object hit_component = Raycast();

            if (typeof(LandPlot).IsInstanceOfType(hit_component))
                HandleSelectedPlot((LandPlot) hit_component);
        }
    }


    // https://forum.unity.com/threads/how-to-detect-if-mouse-is-over-ui.1025533/
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }


    //https://forum.unity.com/threads/how-to-detect-if-mouse-is-over-ui.1025533/
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }


    public object Raycast()
    {
        if (IsPointerOverUIElement(GetEventSystemRaycastResults()))
        {
            // TODO Make this efficient, double raycasting rn;
            return null;
        }

        // Create Mask
        int layer_mask = LayerMask.GetMask("Plot");
        print(layer_mask);

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, layer_mask))
        {
            print(hit);
            GameObject object_hit = hit.transform.gameObject;

            // Check Components
            LandPlot lp = object_hit.GetComponent<LandPlot>();
            if (lp != null)
                return lp;
        }
        else
        {
            // do this on esc press instead
            //tooltipClass.HideTooltip();
            ClearSelected();
        }

        return null;
    }


    void HandleSelectedPlot(LandPlot lp)
    {
        LandPlot found_plot = selected_plots.FirstOrDefault(plot => plot == lp);
        if (found_plot != null)
            return;

        if (selected_plots.Count == max_selected)
            selected_plots.RemoveAt(selected_index_pointer);

        selected_plots.Insert(selected_index_pointer, lp);

        if (lp.cabbage != null)
        {
            ui.Update_Selected_Panel(selected_index_pointer, lp.cabbage);
        }

        selected_index_pointer = (selected_index_pointer + 1) % max_selected;
    }

    void ClearSelected()
    {
        selected_index_pointer = 0;
        selected_plots = new List<LandPlot>();
        ui.Clear_All_Panels();
    }

    public IEnumerable<Cabbage> GetSelectedCabbages()
    {
        print(selected_plots.Count);
        return selected_plots.Where(lp => lp.cabbage != null).Select(lp => lp.cabbage);
    }
}
