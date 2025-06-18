using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShieldCondition
{
    public float ShieldValue;
    public float ShieldDuration;
    public List<GameObject> ShieldEffectObject;
    public Action ShieldRemoveEvents;
    

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

    public static int AddShield(List<ShieldCondition> shields, ShieldCondition shield)
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

        return minIdx;
    }

    public static void RemoveShield(List<ShieldCondition> shields, int idx)
    {
        Debug.Log("remove shield idx: " + idx);
        if (idx >= 0)
        {
            Debug.Log("Removing shield at index: " + shields[idx].ShieldEffectObject.Count);
            for (int i = 0; i < shields[idx].ShieldEffectObject.Count; ++i)
            {
                Debug.Log("Removing shield effect object: " + shields[idx].ShieldEffectObject[i].name);
                shields[idx].ShieldEffectObject[i].SetActive(false);
            }
            shields[idx].ShieldRemoveEvents?.Invoke();
            shields[idx] = new ShieldCondition(); // Reset the shield condition
        }
        else
        {
            Debug.LogWarning("Invalid shield index to remove.");
        }
    }


}