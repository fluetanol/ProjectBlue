using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillSystem : MonoBehaviour
{
    private IInputActionControll inputActionControll;
    [SerializeField] private SkillData skillData;

    void Awake()
    {
        inputActionControll = GetComponent<IInputActionControll>();
    }

    private void OnEnable()
    {
        inputActionControll.InputActions.Player.ESkill.performed += OnESkill;
        inputActionControll.InputActions.Player.QSkill.performed += OnQSkill;
    }


    public void OnESkill(InputAction.CallbackContext context)
    {
        skillData.ExecuteESkill(new SkillContext()
        {
            
        }  ,ref context);   
    }

    public void OnQSkill(InputAction.CallbackContext context)
    {
       skillData.ExecuteQSkill(new SkillContext()
        {
            
        }, ref context);
    }
}
