using System.Collections.Generic;
using UnityEngine;

public interface IBasicData
{
    public float currentHP
    {
        get;
    }
    public float currentDEF
    {
        get;
    }
    public float currentAtk   //플레이어 자체 공격력
    {
        get;
    }
    public float currentMoveSpeed
    {
        get;
    }
    public float currentAttackSpeed
    {
        get;
    }

    public List<ShieldCondition> currentShields
    {
        get;
    }

    public int currentShieldHeadIdx
    {
        get;
        set;
    }

    public int currentShieldCount
    {
        get;
        set;
    }
}

