using UnityEngine;
using UnityEngine.InputSystem;


// 구체적으로 e, q 스킬을 어떻게 동작시킬 지 정의합니다
// 쿨타임 관리, 지속시간 관리는 SkillSystem에서 별도로 함.
// 서비스 로직이라고 생각하면 편함
public abstract class SkillStrategy : ScriptableObject
{
    [SerializeField] protected SkillData _skillData;
    public SkillData SkillData
    {
        get => _skillData;
        private set
        {
            _skillData = value;
        }
    }


    public ISkillIndicator SkillIndicator
    {
        get;
        set;
    }

    //없으면 자식 클래스에서 구현할 필요 없음
    public abstract void OnESkill(SkillContext skillContext, InputAction.CallbackContext inputContext);
    public abstract void OnQSkill(SkillContext skillContext, InputAction.CallbackContext inputContext);
    public abstract void OnFinishESkill(SkillContext skillContext);
    public abstract void OnFinishQSkill(SkillContext skillContext);
}
