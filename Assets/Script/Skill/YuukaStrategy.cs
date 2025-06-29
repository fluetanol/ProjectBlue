using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "YuukaSkillStrategy", menuName = "Scriptable Objects/SkillStrategy")]
public class YuukaSkillStrategy : SkillStrategy
{
    public override void OnESkill(SkillContext skillContext, InputAction.CallbackContext inputContext)
    {
        _skillData.ExecuteESkill(skillContext, inputContext);
    }

    public override void OnQSkill(SkillContext skillContext, InputAction.CallbackContext inputContext)
    {
        SkillIndicator.OnSkillRangeIndicator(_skillData.QSkillRangeIndicator,
                 _skillData.ExecuteQSkill,
                 skillContext,
                 inputContext,
                 _skillData.QAttackRange,
                 _skillData.QRange);
    }

    public override void OnFinishESkill(SkillContext skillContext)
    {
        _skillData.FinishESkill(skillContext);
    }

    public override void OnFinishQSkill(SkillContext skillContext)
    {
        throw new System.NotImplementedException();
    }


}
