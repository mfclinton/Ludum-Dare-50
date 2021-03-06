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
        foreach (Displayed_Cabbage_Info ci in cabbage_info)
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
    int selected_seed_sib_index = 0;
    Image selected_seed;
    public GameObject splice_button, game_over_panel, buy_reg_plot_button, buy_gold_plot_button;
    public Transform seed_panel;
    public Image seed_image_prefab;

    public Selected_UI_Panel[] selected_ui_panels;
    public Cabbage_Info_Helper[] cabbage_info_entries;
    public GeneticVector.TRAIT_ID[] displayed_traits;

    public TextMeshProUGUI cash_text, day_text, event_text, n_splices_text, upkeep_text, sellout_text, highscore_text;
    public TemporaryText cash_event_text;
    public Vector2 seed_display_constraints;

    public void Set_Sale_Triggers()
    {
        for (int i = 0; i < selected_ui_panels.Length; i++)
        {
            int temp = i;
            selected_ui_panels[i].sale_button
                .onClick.AddListener(() => input_mng.Sell(temp));
        }
    }

    public void Disable_Plot_Sale(bool regular_plot)
    {
        if (regular_plot)
        {
            buy_reg_plot_button.SetActive(false);
        }
        else
        {
            buy_gold_plot_button.SetActive(false);
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

            // NOTE VALUE IS MODIFIED HERE
            if (t_id == GeneticVector.TRAIT_ID.GROWN_P || t_id == GeneticVector.TRAIT_ID.NUT_P)
                value = value * 100f;

            TextMeshProUGUI text_mesh = element.panel.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.gameObject != element.panel.gameObject);
            text_mesh.text = value.ToString("F2") + dci.text;

            Image icon = element.panel.GetComponentsInChildren<Image>().First(x => x.gameObject != element.panel.gameObject);
            icon.sprite = dci.icon;
        }

        if (1 < selected_ui_panels.Count(sup => sup.panel.activeSelf == true)
            && !input_mng.GetSelectedCabbages().Any(cabbage => cabbage.grown_p < 1f))
        {
            splice_button.SetActive(true);
        }

        TextMeshProUGUI price = selected_ui_panels[selected_index].sale_button.GetComponentInChildren<TextMeshProUGUI>();
        price.text = Format_Money_String(gm.Get_Price(selected));
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


    public void Game_Over()
    {
        game_over_panel.SetActive(true);
        Clear_All_Panels();
        Update_Sellout_Text(0f);
    }


    /// <summary>
    /// Creates a new cabbage from the selected cabbages
    /// </summary>
    public void Trigger_Splice()
    {
        (GeneticVector gv, int id) = gm.Splice_Selected();
        if (gv == null)
            return;

        Add_Seed(gv, id);
    }

    public void Trigger_Splice_If_Button_Active()
    {
        if (splice_button.activeSelf)
        {
            splice_button.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void Trigger_Random_Seed()
    {
        (GeneticVector gv, int id) = gm.GenerateRandomSeed();
        if (gv == null)
            return;

        Add_Seed(gv, id);
    }

    void Add_Seed(GeneticVector gv, int id)
    {
        Image seed_img = Instantiate(seed_image_prefab, seed_panel);
        seed_img.color = new Color(gv.color.r, gv.color.g, gv.color.b, 0.5f);
        seed_img.GetComponent<RectTransform>().localScale = Vector3.one * Mathf.Lerp(seed_display_constraints[0], seed_display_constraints[1], gv.size_p); // Clamp to prevent SO SMOL

        Button button = seed_img.GetComponent<Button>();
        button.onClick.AddListener(() => input_mng.Set_Selected_Seed(id, seed_img));
    }

    public void Select_Next_Seed(bool go_right)
    {
        if (seed_panel.childCount == 0)
            return;

        if(selected_seed != null)
        {
            if (go_right)
                selected_seed_sib_index += 1;
            else
                selected_seed_sib_index -= 1;
        }

        Clamp_Selected_Seed_Sib_Index();
        Trigger_Seed_Button(seed_panel.GetChild(selected_seed_sib_index).GetComponent<Image>());
    }

    void Clamp_Selected_Seed_Sib_Index()
    {
        selected_seed_sib_index = Mathf.Clamp(selected_seed_sib_index, 0, seed_panel.childCount - 1);
    }

    void Trigger_Seed_Button(Image seed)
    {
        seed.GetComponent<Button>().onClick.Invoke();
    }

    public void Select_Seed(Image new_seed)
    {
        selected_seed = new_seed;
        selected_seed_sib_index = new_seed.transform.GetSiblingIndex();

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
        selected_seed_sib_index -= 1;
        Clamp_Selected_Seed_Sib_Index();

        Destroy(selected_seed.gameObject);
    }

    string Format_Money_String(float cash, bool show_plus = false)
    {
        string before_str = "$";
        if (cash < 0)
            before_str = "-" + before_str;
        else if (show_plus)
            before_str = "+" + before_str;

        return before_str + Mathf.Abs(cash).ToString("F2");
    }

    public void Update_Cash(float cash)
    {
        cash_text.text = "Cash: " + Format_Money_String(cash);
    }

    public void Update_HighScore_Text(float cash)
    {
        highscore_text.text = "Previous High Score: " + Format_Money_String(cash);
    }

    public void Update_Day(float day)
    {
        day_text.text = "Day: " + day.ToString();
        Clear_All_Panels();
    }

    public void Update_Event(string text)
    {
        event_text.text = text;
    }

    public void Update_Sellout_Text(float sellout_value)
    {
        string before_str = "SELLOUT TO BIG CABBAGE\n$";
        sellout_text.text = before_str + sellout_value.ToString("0.00");
    }

    public void Update_N_Splices(int n_splices_today, int max_splices)
    {
        n_splices_text.text = "SPLICES " + n_splices_today.ToString() + "/" + max_splices.ToString();
    }

    public void Update_Upkeep(float days_cash_change, UpkeepEntry ue)
    {
        upkeep_text.text = "Upkeep: " + Format_Money_String(days_cash_change);

        if (ue != null)
        {
            cash_event_text.TriggerFade(ue.cash_change, ue.flavor_text);
        }
    }

    private void Start()
    {
        input_mng = FindObjectOfType<InputManager>();
        gm = FindObjectOfType<GameManager>();

        Set_Sale_Triggers();
        game_over_panel.SetActive(false);
    }
}
