using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SkillStrategy", menuName = "Scriptable Objects/SkillStrategy")]
public abstract class SkillStrategy : ScriptableObject, ISkillEvent
{
    public abstract void OnESkill(InputAction.CallbackContext context);

    public abstract void OnFinishESkill();

    public abstract void OnFinishQSkill();

    public abstract void OnQSkill(InputAction.CallbackContext context);
}
