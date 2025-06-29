using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillContext
{
    public GameObject Target;
    public GameObject Caster;
    public Vector3 TargetPosition;
    public IBasicData BasicData;
    public IMoveData MoveData;
    public IStateData StateData;
    public ISkillTimeData SkillTimeData;
    public List<GameObject> EffectObjects;

    
}

//스킬 데이터와 스킬 실행 시의 동작을 정의합니다.
// DTO 라고 생각하면 됨.
[Serializable]
public struct SkillDataInfo
{
    public string SkillName;
    public string Description;
    public float Distance;
    public float AttackRange;
    public float SkillRange;
    public int CoolTime;
    public int Cost;
    public int Damage;
    public int Heal;
    public int Shield;
    public float Duration;

    public Material indicatorMaterial;
    public List<GameObject> InstantiateObjects;
    public List<GameObject> EffectObjects;
}

public abstract class SkillData : ScriptableObject
{
    [SerializeField] protected SkillDataInfo EskillDataInfo;
    [SerializeField] protected SkillDataInfo QskillDataInfo;
    

    public abstract void ExecuteESkill(SkillContext context, InputAction.CallbackContext inputContext);
    public abstract void ExecuteQSkill(SkillContext context, InputAction.CallbackContext inputContext);

    public virtual void FinishESkill(SkillContext context)
    {
        // Default implementation can be empty or overridden in derived classes
    }
    public virtual void FinishQSkill(SkillContext context)
    {
        // Default implementation can be empty or overridden in derived classes
    }

    public virtual void UpdateESkill(SkillContext context){}
    public virtual void UpdateQSkill(SkillContext context){}

    public virtual void FixedUpdateESkill(SkillContext context){ }
    public virtual void FixedUpdateQSkill(SkillContext context){ }

    public Material ESkillRangeIndicator => EskillDataInfo.indicatorMaterial;
    public Material QSkillRangeIndicator => QskillDataInfo.indicatorMaterial;
    public float EDuration => EskillDataInfo.Duration;
    public float ECoolTime => EskillDataInfo.CoolTime;
    public float QDuration => QskillDataInfo.Duration;
    public float QCoolTime => QskillDataInfo.CoolTime;
    public float QRange => QskillDataInfo.SkillRange;
    public float ERange => EskillDataInfo.SkillRange;
    public float QAttackRange => QskillDataInfo.AttackRange;
    public float EAttackRange => EskillDataInfo.AttackRange;

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
