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
    public GameObject panel;
    public Button sale_button;
    public Selected_UI_Entry[] entries;
}

public class UIManager : MonoBehaviour
{
    InputManager input_mng;
    GameManager gm;
    Image selected_seed;
    public GameObject splice_button;
    public Transform seed_panel;
    public Image seed_image_prefab;

    public Selected_UI_Panel[] selected_ui_panels;
    public Cabbage_Info_Helper[] cabbage_info_entries;
    public GeneticVector.TRAIT_ID[] displayed_traits;

    public TextMeshProUGUI cash_text;

    public void Set_Sale_Triggers()
    {
        for (int i = 0; i < selected_ui_panels.Length; i++)
        {
            int temp = i;
            selected_ui_panels[i].sale_button
                .onClick.AddListener(() => input_mng.Sell(temp));
        }
    }

    /// <summary>
    /// Updates the selected panel for the given cabbage
    /// </summary>
    /// <param name="bottom">True to update the bottom panel, false to update the top panel</param>
    /// <param name="selected">The selected cabbage</param>
    public void Update_Selected_Panel(int selected_index, Cabbage selected)
    {
        selected_ui_panels[selected_index].panel.SetActive(true);
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

        if(1 < selected_ui_panels.Count(sup => sup.panel.activeSelf == true))
        {
            splice_button.SetActive(true);
        }

        TextMeshProUGUI price = selected_ui_panels[selected_index].sale_button.GetComponentInChildren<TextMeshProUGUI>();
        price.text = "$" + gm.Get_Price(selected).ToString("0.00");
    }


    public void Clear_Selected_Panel(int selected_index)
    {
        selected_ui_panels[selected_index].panel.SetActive(false);
        splice_button.SetActive(false);
    }


    public void Clear_All_Panels()
    {
        for (int i = 0; i < selected_ui_panels.Count(); i++)
        {
            Clear_Selected_Panel(i);
        }

        DeSelect_Seed();
    }


    /// <summary>
    /// Creates a new cabbage from the selected cabbages
    /// </summary>
    public void Trigger_Splice()
    {
        (GeneticVector gv, int id) = gm.Splice_Selected();
        Image seed_img = Instantiate(seed_image_prefab, seed_panel);
        seed_img.color = new Color(gv.color.r, gv.color.g, gv.color.b, 0.5f);
        seed_img.GetComponent<RectTransform>().localScale = Vector3.one * gv.size_p;

        Button button = seed_img.GetComponent<Button>();
        button.onClick.AddListener(() => input_mng.Set_Selected_Seed(id, seed_img));
    }

    public void Select_Seed(Image new_seed)
    {
        selected_seed = new_seed;
        Color c = selected_seed.color;
        selected_seed.color = new Color(c.r, c.g, c.b, 1f);
    }

    public void DeSelect_Seed()
    {
        if (selected_seed == null)
            return;

        Color c = selected_seed.color;
        selected_seed.color = new Color(c.r, c.g, c.b, 0.5f);
        selected_seed = null;
    }

    public void Destroy_Seed()
    {
        Destroy(selected_seed.gameObject);
    }

    public void Update_Cash(float cash)
    {
        cash_text.text = "$" + cash.ToString("0.00");
    }

    private void Start()
    {
        input_mng = FindObjectOfType<InputManager>();
        gm = FindObjectOfType<GameManager>();

        Set_Sale_Triggers();
    }
}
