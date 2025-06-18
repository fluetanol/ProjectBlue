using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct SkillContext
{
    public GameObject Target;
    public GameObject Caster;
    public IBasicData BasicData;
    public IMoveData MoveData;
    public ISkillTimeData SkillTimeData;
    public List<GameObject> EffectObjects;
}

[Serializable]
public struct SkillDataInfo
{
    public string SkillName;
    public string Description;
    public float Distance;
    public float Range;
    public int CoolTime;
    public int Cost;
    public int Damage;
    public int Heal;
    public int Shield;
    public float Duration;
    
    public List<GameObject> InstantiateObjects;
    public List<GameObject> EffectObjects;
}

public abstract class SkillData : ScriptableObject
{

    [SerializeField] protected SkillDataInfo EskillDataInfo;
    [SerializeField] protected SkillDataInfo QskillDataInfo;

    public abstract void ExecuteESkill(SkillContext context, ref InputAction.CallbackContext inputContext);
    public abstract void ExecuteQSkill(SkillContext context, ref InputAction.CallbackContext inputContext);

    public virtual void FinishESkill(SkillContext context)
    {
        // Default implementation can be empty or overridden in derived classes
    }
    public virtual void FinishQSkill(SkillContext context)
    {
        // Default implementation can be empty or overridden in derived classes
    }

    public float EDuration => EskillDataInfo.Duration;
    public float ECoolTime => EskillDataInfo.CoolTime;
    public float QDuration => QskillDataInfo.Duration;
    public float QCoolTime => QskillDataInfo.CoolTime;

    public bool IsEContinue
    {
        get;
        set;
    } = false;
    public bool IsQContinue
    {
        get;
        set;
    } = false;

}
