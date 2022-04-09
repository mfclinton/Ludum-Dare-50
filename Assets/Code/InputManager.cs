using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    int max_selected;
    public Camera cam;

    UIManager ui;
    GameManager gm;
    int selected_index_pointer;
    List<LandPlot> selected_plots;
    List<int> display_indexes;
    int selected_seed;

    void Start()
    {
        // cam = Camera.main;
        ui = FindObjectOfType<UIManager>();
        gm = FindObjectOfType<GameManager>();
        max_selected = ui.selected_ui_panels.Length;

        ClearSelected();
    }


    private void Update()
    {
        HandleUserInput();
    }


    void HandleUserInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetMouseButtonDown(0))
        {
            object hit_component = Raycast();

            if (typeof(LandPlot).IsInstanceOfType(hit_component))
                HandleSelectedPlot((LandPlot) hit_component);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ClearSelected();
        }
        else if (scroll != 0f)
        {
            bool go_right = 0f < scroll;
            ui.Select_Next_Seed(go_right);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ui.Trigger_Splice_If_Button_Active();
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
        if (IsPointerOverUIElement(GetEventSystemRaycastResults()) || gm.game_over)
        {
            // TODO Make this efficient, double raycasting rn;
            return null;
        }

        // Create Mask
        int layer_mask = LayerMask.GetMask("Plot", "Cabbage");
        print(layer_mask);

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, layer_mask))
        {
            print(hit.transform.name);
            GameObject object_hit = hit.transform.gameObject;

            // Check Components
            LandPlot lp = object_hit.GetComponent<LandPlot>();
            if (lp != null)
                return lp;

            Cabbage cabbage = object_hit.GetComponent<Cabbage>();
            if (cabbage != null)
                return cabbage.plot;
        }
        else
        {
            // do this on esc press instead
            //tooltipClass.HideTooltip();
            ClearSelected();
            print("Clear");
        }

        return null;
    }


    void HandleSelectedPlot(LandPlot lp)
    {
        LandPlot found_plot = selected_plots.FirstOrDefault(plot => plot == lp);
        if (found_plot != null)
            return;

        if (selected_seed != -1 && lp.cabbage == null)
        {
            gm.Plant_Cabbage(selected_seed, lp);
            ClearSelected();
        }
        else if (selected_seed != -1)
        {
            ClearSelected();
        }

        if (selected_plots.Count == max_selected)
        {
            selected_plots.RemoveAt(selected_index_pointer);
            display_indexes.RemoveAt(selected_index_pointer);
        }

        selected_plots.Insert(selected_index_pointer, lp);
        display_indexes.Insert(selected_index_pointer, selected_index_pointer);

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
        display_indexes = new List<int>();
        selected_seed = -1;
        ui.Clear_All_Panels();
    }

    public IEnumerable<Cabbage> GetSelectedCabbages()
    {
        print(selected_plots.Count);
        return selected_plots.Where(lp => lp.cabbage != null).Select(lp => lp.cabbage);
    }

    public void Sell(int index)
    {
        int selected_plot_index = display_indexes.Select((date_i, idx) => new {di = date_i, i = idx}).First(obj => obj.di == index).i;

        LandPlot plot = selected_plots[selected_plot_index];

        // Clean Up
        selected_index_pointer = selected_plot_index;
        selected_plots.RemoveAt(selected_plot_index);
        display_indexes.RemoveAt(selected_plot_index);

        ui.Clear_Selected_Panel(index);

        gm.Sell(plot);
    }

    public void Set_Selected_Seed(int id, Image img)
    {
        ui.DeSelect_Seed();
        selected_seed = id;
        ui.Select_Seed(img);
    }
}
