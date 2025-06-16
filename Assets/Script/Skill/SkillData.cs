using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct SkillContext
{
    public GameObject Target;
    public GameObject Caster;
    public Vector3 TargetPosition;
    public Vector3 CasterPosition;
    public IBasicData BasicData;
    public IMoveData MoveData;
}

[Serializable]
public struct SkillDataInfo
{
    public float Distance;
    public float Range;
    public int CoolTime;
    public int Cost;
    public int Damage;
    public int Heal;
    public int Shield;
    public int Duration;
    
    public List<GameObject> InstantiateObjects;
    public List<GameObject> EffectObjects;
}


public abstract class SkillData : ScriptableObject
{
    [SerializeField] protected SkillDataInfo EskillDataInfo;
    [SerializeField] protected SkillDataInfo QskillDataInfo;

    public abstract void ExecuteESkill(SkillContext context, ref InputAction.CallbackContext inputContext);
    public abstract void ExecuteQSkill(SkillContext context, ref InputAction.CallbackContext inputContext);
}
