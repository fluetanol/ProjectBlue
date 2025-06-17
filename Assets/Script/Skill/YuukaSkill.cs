using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName= "Yuuka", menuName = "Skill/YuukaSkill")]
public class YuukaSkill : SkillData
{
    int shieldIdx = 0;
    public override void ExecuteESkill(SkillContext context, ref InputAction.CallbackContext inputContext)
    {
        context.BasicData.currentShieldCount++;
        context.BasicData.currentShieldHeadIdx = ShieldCondition.MaxShieldIdx(context.BasicData.currentShields);
        ShieldCondition.AddShield(context.BasicData.currentShields, new ShieldCondition()
        {
            ShieldValue = EskillDataInfo.Shield,
            ShieldDuration = EskillDataInfo.Duration
        });

        foreach(var effect in context.EffectObjects)
        {
            if (effect != null)
            {
                effect.SetActive(false);
            }
        }
    }


    public override void FinishESkill(SkillContext context)
    {
        context.BasicData.currentShields[shieldIdx] = new ShieldCondition()
        {
            ShieldValue = 0,
            ShieldDuration = 0
        };
        ShieldCondition.RemoveShield(context.BasicData.currentShields, shieldIdx);

        context.BasicData.currentShieldCount--;
        context.BasicData.currentShieldHeadIdx = ShieldCondition.MaxShieldIdx(context.BasicData.currentShields);
    }

    public override void ExecuteQSkill(SkillContext context, ref InputAction.CallbackContext inputContext)
    {
        throw new System.NotImplementedException();
    }
}
