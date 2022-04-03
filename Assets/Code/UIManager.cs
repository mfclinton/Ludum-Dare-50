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

[System.Serializable]
public class Selected_UI_Panel
{
    public Selected_UI_Entry[] entries;
}

public class UIManager : MonoBehaviour
{
    InputManager input_mng;
    GameManager gm;
    public Selected_UI_Panel[] selected_ui_panels;

    /// <summary>
    /// Updates the selected panel for the given cabbage
    /// </summary>
    /// <param name="bottom">True to update the bottom panel, false to update the top panel</param>
    /// <param name="selected">The selected cabbage</param>
    public void Update_Selected_Panel(int selected_index, Cabbage selected)
    {
        Selected_UI_Entry[] ui_elements = selected_ui_panels[selected_index].entries;

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


    public void Clear_Selected_Panel(int selected_index)
    {
        Selected_UI_Entry[] ui_elements = selected_ui_panels[selected_index].entries;
        if (selected_index == 1)
            ui_elements = selected_ui_panels[selected_index].entries;

        foreach (GeneticVector.TRAIT_ID t_id in Enum.GetValues(typeof(GeneticVector.TRAIT_ID)))
        {
            if (!ui_elements.Any(ui_elem => ui_elem.id == t_id))
                continue;

            Selected_UI_Entry element = ui_elements.First(ui_elem => ui_elem.id == t_id);
            element.panel.gameObject.SetActive(false);
        }
    }


    public void Clear_All_Panels()
    {
        for (int i = 0; i < selected_ui_panels.Count(); i++)
        {
            Clear_Selected_Panel(i);
        }
    }


    /// <summary>
    /// Creates a new cabbage from the selected cabbages
    /// </summary>
    public void Trigger_Splice()
    {
        gm.Splice_Selected();
    }


    private void Start()
    {
        input_mng = FindObjectOfType<InputManager>();
        gm = FindObjectOfType<GameManager>();

        // TODO: Nick make this work with your selection system
        // Example
        //Cabbage cabbage = FindObjectOfType<Cabbage>();
        //Update_Selected_Panel(true, cabbage);
    }
}
