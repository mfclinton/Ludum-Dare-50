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
public class Displayed_Cabbage_Info
{
    public float thresh;
    public string text;
    public Sprite icon;
}

[System.Serializable]
public class Cabbage_Info_Helper
{
    public GeneticVector.TRAIT_ID id;
    public Displayed_Cabbage_Info[] cabbage_info;

    public void Initialize()
    {
        cabbage_info = cabbage_info.OrderBy(ci => ci.thresh).ToArray();
    }

    public Displayed_Cabbage_Info Get_Cabbage_Info(float value)
    {
        if (cabbage_info.Length == 0)
            return null;

        int index = 0;
        foreach(Displayed_Cabbage_Info ci in cabbage_info)
        {
            if (value < cabbage_info[index].thresh)
                break;
            index++;
        }

        if (index == cabbage_info.Length)
            index = cabbage_info.Length - 1;

        return cabbage_info[index];
    }
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
    public Cabbage_Info_Helper[] cabbage_info_entries;
    public GeneticVector.TRAIT_ID[] displayed_traits;

    /// <summary>
    /// Updates the selected panel for the given cabbage
    /// </summary>
    /// <param name="bottom">True to update the bottom panel, false to update the top panel</param>
    /// <param name="selected">The selected cabbage</param>
    public void Update_Selected_Panel(int selected_index, Cabbage selected)
    {
        Dictionary<GeneticVector.TRAIT_ID, float> dict = selected.GetObservableGeneticDict();
        Selected_UI_Entry[] ui_elements = selected_ui_panels[selected_index].entries;

        foreach (GeneticVector.TRAIT_ID t_id in displayed_traits)
        {
            Selected_UI_Entry element = ui_elements.FirstOrDefault(ui_elem => ui_elem.id == t_id);
            Cabbage_Info_Helper cih = cabbage_info_entries.FirstOrDefault(cih => cih.id == t_id);

            if (element == null || cih == null)
                continue;

            float value = dict[t_id];
            Displayed_Cabbage_Info dci = cih.Get_Cabbage_Info(value);

            element.panel.gameObject.SetActive(true);
            
            TextMeshProUGUI text_mesh = element.panel.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.gameObject != element.panel.gameObject);
            text_mesh.text = dci.text;

            Image icon = element.panel.GetComponentsInChildren<Image>().First(x => x.gameObject != element.panel.gameObject);
            icon.sprite = dci.icon;
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
