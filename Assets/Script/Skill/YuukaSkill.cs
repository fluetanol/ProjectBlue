using UnityEngine;
using UnityEngine.InputSystem;



[CreateAssetMenu(fileName = "Yuuka", menuName = "Skill/YuukaSkill")]
public class YuukaSkill : SkillData
{
    int shieldIdx = 0;
    public override void ExecuteESkill(SkillContext context, InputAction.CallbackContext inputContext)
    {
        shieldIdx = ShieldCondition.AddShield(context.BasicData.currentShields, new ShieldCondition()
        {
            ShieldValue = EskillDataInfo.Shield,
            ShieldDuration = EskillDataInfo.Duration,
            ShieldEffectObject = context.EffectObjects,
            ShieldRemoveEvents = () =>
            {
                ShieldRemove(context);
            }
        });

        context.BasicData.currentShieldCount++;
        context.BasicData.currentShieldHeadIdx = ShieldCondition.MaxShieldIdx(context.BasicData.currentShields);

        Debug.Log(context.BasicData.currentShields[shieldIdx].ShieldEffectObject.Count);

        foreach (var effect in context.EffectObjects)
        {
            if (effect != null)
            {
                effect.SetActive(true);
            }
        }
        IsEContinue = true;
    }


    public override void FinishESkill(SkillContext context)
    {
        ShieldCondition.RemoveShield(context.BasicData.currentShields, shieldIdx);
    }

    public override void ExecuteQSkill(SkillContext context, InputAction.CallbackContext inputContext)
    {
        //throw new System.NotImplementedException();
        Debug.Log("Q Skill Executed : " + context.TargetPosition);
    }

    private void ShieldRemove(SkillContext context)
    {
        context.BasicData.currentShieldCount--;
        context.BasicData.currentShieldHeadIdx = ShieldCondition.MaxShieldIdx(context.BasicData.currentShields);
        context.SkillTimeData.EElapsedTime = 0f;
        context.SkillTimeData.ECoolTimeElapsed = ECoolTime;
        IsEContinue = false;
    }
}
