using System.Collections.Generic;
using UnityEngine;

public struct ShieldCondition
{
    public float ShieldValue;
    public float ShieldDuration;
    public List<GameObject> ShieldEffectObject;

    public static int MaxShieldIdx(List<ShieldCondition> shields)
    {
        float maxValue = 0;
        int maxIdx = -1;
        for (int i = 0; i < 3; i++)
        {
            if (maxValue < shields[i].ShieldValue)
            {
                maxValue = shields[i].ShieldValue;
                maxIdx = i;
            }
        }
        return maxIdx;
    }

    public static void AddShield(List<ShieldCondition> shields, ShieldCondition shield)
    {
        float minValue = float.PositiveInfinity;
        int minIdx = -1;
        for (int i = 0; i < shields.Count; i++)
        {
            if (shields[i].ShieldValue < minValue)
            {
                minValue = shields[i].ShieldValue;
                minIdx = i;
            }
        }
        if (minIdx != -1)
        {
            shields[minIdx] = shield;
        }
        else
        {
            Debug.LogWarning("No available shield slot to add the new shield.");
        }
    }

    public static void RemoveShield(List<ShieldCondition> shields, int idx)
    {
        if (idx >= 0 && idx < shields.Count)
        {
            for(int i=0; i< shields[idx].ShieldEffectObject.Count; i++)
            {
                if (shields[idx].ShieldEffectObject[i] != null)
                {
                    shields[idx].ShieldEffectObject[i].SetActive(false);
                }
            }
            shields[idx] = new ShieldCondition(); // Reset the shield condition
        }
        else
        {
            Debug.LogWarning("Invalid shield index to remove.");
        }
    }

    
}