using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorNode
{
    public Color color;
    public float mult;
}

public class Market : MonoBehaviour
{
    // Variables for changing the market
    public ColorNode[] color_nodes;
    public ColorNode[] active_color_nodes;

    public const float SIZE_MEAN = 10f;
    public const float WEIGHT_MEAN = 10f;
    public const float NUT_P_MEAN = 10f;
    public const float COLOR_MEAN = 10f;

    public const float LOG_NORMAL_SIGMA = 0.75f;

    private const int ALGO_WARMUP_TIMESTEPS = 50;
    private float EMA_ALPHA = 0.08f;

    // Pricing arrays
    public List<float> size_prices;
    public List<float> weight_prices;
    public List<float> nut_p_prices;
    public List<float> color_prices;

    private float size_mu;
    private float weight_mu;
    private float nut_p_mu;
    private float color_mu;

    private void Start()
    {
        size_prices = new List<float>();
        weight_prices = new List<float>();
        nut_p_prices = new List<float>();
        color_prices = new List<float>();
        active_color_nodes = color_nodes;

        // Compute parameters for log normal distribution based on desired mean
        size_mu = DeriveLogNormalMu(SIZE_MEAN, LOG_NORMAL_SIGMA);
        weight_mu = DeriveLogNormalMu(WEIGHT_MEAN, LOG_NORMAL_SIGMA);
        nut_p_mu = DeriveLogNormalMu(NUT_P_MEAN, LOG_NORMAL_SIGMA);
        color_mu = DeriveLogNormalMu(COLOR_MEAN, LOG_NORMAL_SIGMA);

        WarmupPrices();
    }

    public float DetermineCabbageValue(Cabbage cabbage)
    {
        GeneticVector genetic_vector = cabbage.chromosome;

        float size = cabbage.Get_Actual_Size();
        float weight = cabbage.Get_Actual_Weight();
        float nut_p = cabbage.Get_Actual_Nutrients();
        Color color = genetic_vector.color;

        float price = CabbagePrice(size, weight, nut_p, color) * cabbage.grown_p;
        return 0f <= price ? price : 0f;
    }

    public void AdvanceTimestep()
    {
        GenerateNextPrices();
    }

    public void DoMarketEvent(MarketEvent mkt_event)
    {
        size_mu = DeriveLogNormalMu(SIZE_MEAN + mkt_event.size_change, LOG_NORMAL_SIGMA + mkt_event.volatility_change);
        weight_mu = DeriveLogNormalMu(WEIGHT_MEAN + mkt_event.weight_change, LOG_NORMAL_SIGMA + mkt_event.volatility_change);
        nut_p_mu = DeriveLogNormalMu(NUT_P_MEAN + mkt_event.nut_p_change, LOG_NORMAL_SIGMA + mkt_event.volatility_change);
        color_mu = DeriveLogNormalMu(COLOR_MEAN + mkt_event.color_change, LOG_NORMAL_SIGMA + mkt_event.volatility_change);
        EMA_ALPHA += mkt_event.volatility_change;

        if (active_color_nodes.Length != mkt_event.color_multipliers.Length)
        {
            Debug.Log("Color multiplier array length does not match active color node array length");
            throw new System.Exception("Color multiplier array length does not match active color node array length");
        }

        for (int i = 0; i < active_color_nodes.Length; i++)
        {
            active_color_nodes[i].mult = mkt_event.color_multipliers[i];
        }

    }

    public void RevertMarketEvent(MarketEvent mkt_event)
    {
        size_mu = DeriveLogNormalMu(SIZE_MEAN - mkt_event.size_change, LOG_NORMAL_SIGMA - mkt_event.volatility_change);
        weight_mu = DeriveLogNormalMu(WEIGHT_MEAN - mkt_event.weight_change, LOG_NORMAL_SIGMA - mkt_event.volatility_change);
        nut_p_mu = DeriveLogNormalMu(NUT_P_MEAN - mkt_event.nut_p_change, LOG_NORMAL_SIGMA - mkt_event.volatility_change);
        color_mu = DeriveLogNormalMu(COLOR_MEAN - mkt_event.color_change, LOG_NORMAL_SIGMA - mkt_event.volatility_change);
        EMA_ALPHA -= mkt_event.volatility_change;

        for (int i = 0; i < active_color_nodes.Length; i++)
        {
            active_color_nodes[i].mult = color_nodes[i].mult;
        }

    }

    private float CabbagePrice(float size, float weight, float nut_p, Color color)
    {
        float size_component = size * size_prices[size_prices.Count - 1];
        float weight_component = weight * weight_prices[weight_prices.Count - 1];
        float nut_p_component = nut_p * nut_p_prices[nut_p_prices.Count - 1];
        float color_component = CabbageMultiplier(color) * color_prices[color_prices.Count - 1];

        return (size_component + weight_component + nut_p_component + color_component);
    }

    private void WarmupPrices()
    {
        // Warmup size prices
        for (int i = 0; i < ALGO_WARMUP_TIMESTEPS; i++)
        {
            float value = GenerateLogNormal(size_mu, LOG_NORMAL_SIGMA);
            if (i == 0)
            {
                size_prices.Add(value);
            }
            else
            {
                size_prices.Add(ExponentialMovingAverage(value, size_prices[i - 1], EMA_ALPHA));
            }
        }

        // Warmup weight prices
        for (int i = 0; i < ALGO_WARMUP_TIMESTEPS; i++)
        {
            float value = GenerateLogNormal(weight_mu, LOG_NORMAL_SIGMA);
            if (i == 0)
            {
                weight_prices.Add(value);
            }
            else
            {
                weight_prices.Add(ExponentialMovingAverage(value, weight_prices[i - 1], EMA_ALPHA));
            }
        }

        // Warmup nut_p prices
        for (int i = 0; i < ALGO_WARMUP_TIMESTEPS; i++)
        {
            float value = GenerateLogNormal(nut_p_mu, LOG_NORMAL_SIGMA);
            if (i == 0)
            {
                nut_p_prices.Add(value);
            }
            else
            {
                nut_p_prices.Add(ExponentialMovingAverage(value, nut_p_prices[i - 1], EMA_ALPHA));
            }
        }

        //  Warmup color prices
        for (int i = 0; i < ALGO_WARMUP_TIMESTEPS; i++)
        {
            float value = GenerateLogNormal(color_mu, LOG_NORMAL_SIGMA);
            if (i == 0)
            {
                color_prices.Add(value);
            }
            else
            {
                color_prices.Add(ExponentialMovingAverage(value, color_prices[i - 1], EMA_ALPHA));
            }
        }

    }

    private void GenerateNextPrices()
    {
        size_prices.Add(ExponentialMovingAverage(GenerateLogNormal(size_mu, LOG_NORMAL_SIGMA), size_prices[size_prices.Count - 1], EMA_ALPHA));
        weight_prices.Add(ExponentialMovingAverage(GenerateLogNormal(weight_mu, LOG_NORMAL_SIGMA), weight_prices[weight_prices.Count - 1], EMA_ALPHA));
        nut_p_prices.Add(ExponentialMovingAverage(GenerateLogNormal(nut_p_mu, LOG_NORMAL_SIGMA), nut_p_prices[nut_p_prices.Count - 1], EMA_ALPHA));
        color_prices.Add(ExponentialMovingAverage(GenerateLogNormal(color_mu, LOG_NORMAL_SIGMA), color_prices[color_prices.Count - 1], EMA_ALPHA));
    }

    private float CabbageMultiplier(Color color)
    {
        float total_dist = 0f;
        float multiplier = 0f;
        foreach (ColorNode node in active_color_nodes)
        {
            float dist = Vector4.Distance(node.color, color);
            multiplier += (1f / (dist + 0.01f)) * node.mult;
            total_dist += (1f / (dist + 0.01f));
        }
        multiplier /= total_dist;
        return multiplier;
    }

    private float ExponentialMovingAverage(float value, float previous_ema, float alpha)
    {
        return alpha * value + (1f - alpha) * previous_ema;
    }

    private float GenerateLogNormal(float mu, float sigma)
    {
        float U1 = Mathf.Clamp(Random.value, 0.0001f, 1f);
        float U2 = Random.value;
        float Z1 = Mathf.Sqrt(-2f * Mathf.Log(U1)) * Mathf.Cos(2f * Mathf.PI * U2);
        float scaled_normal = (sigma * Z1) + mu;
        return Mathf.Exp(scaled_normal);
    }

    private float DeriveLogNormalMu(float desired_mean, float sigma)
    {
        return Mathf.Log(desired_mean) - (sigma * sigma) / 2f;
    }


}
