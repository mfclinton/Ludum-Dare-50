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


    private void Start()
    {
        Clear_Selected_Panel(false);
        Clear_Selected_Panel(true);

        //Example
        //Cabbage cabbage = FindObjectOfType<Cabbage>();
        //Update_Selected_Panel(true, cabbage);
    }
}
