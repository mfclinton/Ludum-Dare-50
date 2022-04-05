using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyoutForecaster : MonoBehaviour
{
    public int BUYOUT_EXTRAPOLATION_DAYS = 10;
    public int BUYOUT_LAST_DAYS_FOR_AVG = 10;
    public float BUYOUT_DEFAULT_PRICE = 10f;

    public float GetBuyoutAmount(Dictionary<int, List<float>> data)
    {
        //sum each list in data to get total daily income (or loss)
        List<float> daily_income = new List<float>();
        foreach (KeyValuePair<int, List<float>> entry in data)
        {
            float income = 0f;
            foreach (float price in entry.Value)
            {
                income += price;
            }
            daily_income.Add(income);
        }

        // If there are less than BUYOUT_DEFAULT_FIRST_DAYS days of data, return BUYOUT_DEFAULT_PRICE
        if (daily_income.Count < BUYOUT_LAST_DAYS_FOR_AVG)
        {
            return BUYOUT_DEFAULT_PRICE;
        }

        //Get last BUYOUT_LAST_DAYS_FOR_AVG values in daily_income
        List<float> daily_income_last = new List<float>();
        for (int i = Mathf.Max(0, daily_income.Count - 50); i < daily_income.Count; ++i)
        {
            daily_income_last.Add(daily_income[i]);
        }

        //calculate average of daily income for last BUYOUT_LAST_DAYS_FOR_AVG
        float average_income = 0f;
        foreach (float income in daily_income_last)
        {
            average_income += income;
        }
        average_income /= daily_income_last.Count;

        //Buyout formula: average sales over last 10 days * Clamp01(curr day / BUYOUT_DAYS_TO_ESTABLISH_SELF) * BUYOUT_EXTRAPOLATION_DAYS
        float buyout_price = Mathf.Max(10f, average_income * BUYOUT_EXTRAPOLATION_DAYS);
        return buyout_price;
    }
}
