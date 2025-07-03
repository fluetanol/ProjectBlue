using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 기본 데이터를 정의하는 인터페이스
/// </summary>
public interface IBasicData
{
    public float currentHP
    {
        get;
    }
    public float maxHP
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

