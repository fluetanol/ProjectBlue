using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface ISkillEvent
{
    void OnESkill(InputAction.CallbackContext context);
    void OnQSkill(InputAction.CallbackContext context);
    void OnFinishESkill();
    void OnFinishQSkill();
}



public class PlayerSkillSystem : MonoBehaviour, ISkillEvent
{
    private IInputActionControll inputActionControll;
    private IBasicData basicData;
    private IMoveData moveData;

    [SerializeField] private List<GameObject> effectObjects;
    [SerializeField] private SkillData skillData;

    private float eElapsedTime = 0f;
    private float qElapsedTime = 0f;


    void Awake()
    {
        inputActionControll = GetComponent<IInputActionControll>();
        basicData = GetComponent<PlayerDataManager>();
        moveData = GetComponent<IMoveData>();
    }

    private void OnEnable()
    {
        inputActionControll.InputActions.Player.ESkill.performed += OnESkill;
        inputActionControll.InputActions.Player.QSkill.performed += OnQSkill;
    }


    public void Update()
    {
        ESkillTimer();
        QSkillTimer();
    }


    private void ESkillTimer()
    {
        if (skillData.IsEContinue)
        {
            eElapsedTime += Time.deltaTime;
            if (eElapsedTime >= skillData.EDuration)
            {
                OnFinishESkill();
            }
        }
    }

    private void QSkillTimer()
    {
        if (skillData.IsQContinue)
        {
            qElapsedTime += Time.deltaTime;
            if (qElapsedTime >= skillData.QDuration)
            {
                OnFinishQSkill();
            }
        }
    }   

    public void OnFinishESkill()
    {
        SkillContext finishSkillContext = new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            BasicData = basicData,
            EffectObjects = effectObjects
        };
        skillData.FinishESkill(finishSkillContext);
        eElapsedTime = 0f; // Reset the elapsed time after finishing the skill
    }

    public void OnFinishQSkill()
    {
        SkillContext finishSkillContext = new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            BasicData = basicData,
            EffectObjects = effectObjects
        };
        skillData.FinishQSkill(finishSkillContext);
        skillData.IsEContinue = false;
        qElapsedTime = 0f; // Reset the elapsed time after finishing the skill
    }




    public void OnESkill(InputAction.CallbackContext context)
    {
        print("OnESkill called");
        skillData.ExecuteESkill(new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            BasicData = basicData,
            EffectObjects = effectObjects
        }, ref context);
    }

    public void OnQSkill(InputAction.CallbackContext context)
    {
        skillData.ExecuteQSkill(new SkillContext()
        {

        }, ref context);
    }


}
