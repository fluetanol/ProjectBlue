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

public interface ISkillTimeData
{
    float ECoolTimeElapsed
    {
        get;
        set;
    }
    float QCoolTimeElapsed
    {
        get;
        set;
    }

    float EElapsedTime
    {
        get;
        set;
    }
    float QElapsedTime
    {
        get;
        set;
    }

    float QCoolTime
    {
        get;
    }

    float ECoolTime
    {
        get;
    }

}

public class PlayerSkillSystem : MonoBehaviour, ISkillEvent, ISkillTimeData
{
    private IInputActionControll _inputActionControll;
    private IStateData _stateData;
    private IBasicData _basicData;
    private IMoveData _moveData;

    private ISkillIndicator _skillIndicator;

    [SerializeField] private List<GameObject> effectObjects;
    [SerializeField] private SkillData skillData;
    [SerializeField] private SkillStrategy skillStrategy;

    
    [SerializeField] private float _eCoolTimeElapsed;
    public float ECoolTimeElapsed
    {
        get { return _eCoolTimeElapsed; }
        set { _eCoolTimeElapsed = value; }
    }

    [SerializeField] private float _qCoolTimeElapsed;
    public float QCoolTimeElapsed
    {
        get { return _qCoolTimeElapsed; }
        set { _qCoolTimeElapsed = value; }
    }

    private float _eElapsedTime;
    public float EElapsedTime
    {
        get { return _eElapsedTime; }
        set { _eElapsedTime = value; }
    }

    private float _qElapsedTime;
    public float QElapsedTime
    {
        get { return _qElapsedTime; }
        set { _qElapsedTime = value; }
    }

    public float QCoolTime => skillData.QCoolTime;
    public float ECoolTime => skillData.ECoolTime;

    void Awake()
    {
        _inputActionControll = GetComponent<IInputActionControll>();
        _basicData = GetComponent<PlayerDataManager>();
        _moveData = GetComponent<PlayerMovement>();
        _stateData = GetComponent<PlayerStateManager>();
        _skillIndicator = GetComponentInChildren<SkillIndicator>(true);

        _eCoolTimeElapsed = skillData.ECoolTime;
        _qCoolTimeElapsed = skillData.QCoolTime;
        skillData.IsEContinue = false;
        skillData.IsQContinue = false;
    }

    private void OnEnable()
    {
        _inputActionControll.InputActions.Player.ESkill.performed += OnESkill;
        _inputActionControll.InputActions.Player.QSkill.performed += OnQSkill;
    }


    public void Update()
    {
        ECoolTimer();
        QCoolTimer();
        ESkillTimer();
        QSkillTimer();

        skillData.UpdateESkill(new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            BasicData = _basicData,
            EffectObjects = effectObjects,
            SkillTimeData = this
        });
        skillData.UpdateQSkill(new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            BasicData = _basicData,
            EffectObjects = effectObjects,
            SkillTimeData = this
        });
    }
    
    public void FixedUpdate()
    {
        skillData.FixedUpdateESkill(new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            BasicData = _basicData,
            MoveData = _moveData,
            EffectObjects = effectObjects,
            StateData = _stateData,
            SkillTimeData = this
        });
        skillData.FixedUpdateQSkill(new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            BasicData = _basicData,
            MoveData = _moveData,
            StateData = _stateData,
            EffectObjects = effectObjects,
            SkillTimeData = this
        });
    }


    private void ESkillTimer()
    {
        if (skillData.IsEContinue)
        {
            _eElapsedTime += Time.deltaTime;
            if (_eElapsedTime >= skillData.EDuration)
            {
                OnFinishESkill();
                _eElapsedTime = 0f; // Reset the elapsed time after finishing the skill
                _eCoolTimeElapsed = skillData.ECoolTime; // Reset the cooldown after finishing the skill
            }

        }
    }

    private void QSkillTimer()
    {
        if (skillData.IsQContinue)
        {
            _qElapsedTime += Time.deltaTime;
            if (_qElapsedTime >= skillData.QDuration)
            {
                OnFinishQSkill();
                _qElapsedTime = 0f; // Reset the elapsed time after finishing the skill
                _qCoolTimeElapsed = skillData.QCoolTime; // Reset the cooldown after finishing the skill
            }
            // Ensure _qElapsedTime does not exceed CoolTime
        }
    }

    private void ECoolTimer()
    {
        _eCoolTimeElapsed =  Mathf.Clamp(_eCoolTimeElapsed - Time.deltaTime, 0f, skillData.ECoolTime);
    }

    private void QCoolTimer()
    {
        _qCoolTimeElapsed = Mathf.Clamp(_qCoolTimeElapsed - Time.deltaTime, 0f, skillData.QCoolTime);
    }


    public void OnESkill(InputAction.CallbackContext context)
    {
        if (_eCoolTimeElapsed > 0f || skillData.IsEContinue)
        {
            print("E Skill is on cooldown");
            return; // Exit if the skill is on cooldown
        }

        print("OnESkill called");
        skillData.ExecuteESkill(new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            BasicData = _basicData,
            EffectObjects = effectObjects,
            MoveData = _moveData,
            StateData = _stateData,
            SkillTimeData = this
        },  context);


        print(_basicData.currentShieldCount);
        print(_basicData.currentShields[0].ShieldEffectObject.Count);
    }

    public void OnQSkill(InputAction.CallbackContext context)
    {
        if (_qCoolTimeElapsed > 0f || skillData.IsQContinue)
        {
            print("Q Skill is on cooldown");
            return; // Exit if the skill is on cooldown
        }
        OnShowSkillRange(context);
    }

    public void OnFinishESkill()
    {
        SkillContext finishSkillContext = new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            BasicData = _basicData,
            EffectObjects = effectObjects,
            SkillTimeData = this
        };
        skillData.FinishESkill(finishSkillContext);
        _eElapsedTime = 0f; // Reset the elapsed time after finishing the skill
    }

    public void OnFinishQSkill()
    {
        SkillContext finishSkillContext = new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            BasicData = _basicData,
            EffectObjects = effectObjects
        };
        skillData.FinishQSkill(finishSkillContext);
        skillData.IsEContinue = false;
        _qElapsedTime = 0f; // Reset the elapsed time after finishing the skill
    }


    public void OnShowSkillRange(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;

        switch (actionName)
        {
            case "QSkill":
                _skillIndicator.OnSkillRangeIndicator(skillData.QSkillRangeIndicator,
                skillData.ExecuteQSkill,
                 new SkillContext()
                 {
                     Target = null,
                     Caster = gameObject,
                     BasicData = _basicData,
                    MoveData = _moveData,
                    StateData = _stateData,
                     EffectObjects = effectObjects,
                     SkillTimeData = this
                 }, context
                 , skillData.QAttackRange
                 , skillData.QRange);
                break;

            case "ESkill":

                break;
            default:
                Debug.LogError("Unknown action name for skill range display: " + actionName);
                return;
        }       

    }

   


}
