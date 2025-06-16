using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName= "Yuuka", menuName = "Skill/YuukaSkill")]
public class YuukaSkill : SkillData
{
    public override void ExecuteESkill(SkillContext context, ref InputAction.CallbackContext inputContext)
    {
        EskillDataInfo.InstantiateObjects.ForEach(obj => 
        {
            GameObject instance = Instantiate(obj, context.TargetPosition, Quaternion.identity);
            // 추가적인 로직이 필요하다면 여기에 작성
        });
    }

    public override void ExecuteQSkill(SkillContext context, ref InputAction.CallbackContext inputContext)
    {
        throw new System.NotImplementedException();
    }
}
