using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

[System.Serializable]
public class Selected_UI_Entry
{
    public GeneticVector.TRAIT_ID id;
    public Image panel;
}

public class UIManager : MonoBehaviour
{
    public Selected_UI_Entry[] selected_ui_entries_top, selected_ui_entries_bottom;
    public GameObject temp_cabbage_prefab, c1_go, c2_go; // TODO: TEMP, REMOVE

    /// <summary>
    /// Updates the selected panel for the given cabbage
    /// </summary>
    /// <param name="bottom">True to update the bottom panel, false to update the top panel</param>
    /// <param name="selected">The selected cabbage</param>
    public void Update_Selected_Panel(bool bottom, Cabbage selected)
    {
        Selected_UI_Entry[] ui_elements = selected_ui_entries_top;
        if (bottom)
            ui_elements = selected_ui_entries_bottom;

        foreach (GeneticVector.TRAIT_ID t_id in Enum.GetValues(typeof(GeneticVector.TRAIT_ID)))
        {
            if (!ui_elements.Any(ui_elem => ui_elem.id == t_id))
                continue;

            Selected_UI_Entry element = ui_elements.First(ui_elem => ui_elem.id == t_id);

            string text = selected.chromosome.GetTraitClassification(t_id);
            // TODO: Grab a icon

            element.panel.gameObject.SetActive(true);

            TextMeshProUGUI text_mesh = element.panel.GetComponentInChildren<TextMeshProUGUI>();
            text_mesh.text = text;
        }
    }


    public void Clear_Selected_Panel(bool bottom)
    {
        Selected_UI_Entry[] ui_elements = selected_ui_entries_top;
        if (bottom)
            ui_elements = selected_ui_entries_bottom;

        foreach (GeneticVector.TRAIT_ID t_id in Enum.GetValues(typeof(GeneticVector.TRAIT_ID)))
        {
            if (!ui_elements.Any(ui_elem => ui_elem.id == t_id))
                continue;

            Selected_UI_Entry element = ui_elements.First(ui_elem => ui_elem.id == t_id);
            element.panel.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Creates a new cabbage from the selected cabbages
    /// </summary>
    public void Trigger_Splice()
    {
        // TODO : NICK make this work with your selection system
        Cabbage c1 = c1_go.GetComponent<Cabbage>();
        Cabbage c2 = c2_go.GetComponent<Cabbage>();

        if (c1 == null || c2 == null)
            return;

        Cabbage c3 = Instantiate(temp_cabbage_prefab).GetComponent<Cabbage>(); // TODO: Matt or Nick need to instantiate the cabbage prefab beforehand to c3
        c1.CrossBreed(c2, c3);

        return;
    }


    private void Start()
    {
        Clear_Selected_Panel(false);
        Clear_Selected_Panel(true);

        // TODO: Nick make this work with your selection system
        // Example
        //Cabbage cabbage = FindObjectOfType<Cabbage>();
        //Update_Selected_Panel(true, cabbage);
    }
}
