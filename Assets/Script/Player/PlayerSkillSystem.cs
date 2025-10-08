using System.Collections.Generic;
using Unity.Collections;
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
    /// <summary>
    /// E 스킬 쿨타임이 얼마나 지났는지
    /// </summary>
    float ECoolTimeElapsed
    {
        get;
        set;
    }

    /// <summary>
    /// Q 스킬 쿨타임이 얼마나 지났는지
    /// </summary>
    float QCoolTimeElapsed
    {
        get;
        set;
    }

    /// <summary>
    /// E 스킬 지속 시간이 얼마나 지났는지
    /// </summary>
    float EDurationElapsedTime
    {
        get;
        set;
    }

    /// <summary>
    /// Q 스킬 지속 시간이 얼마나 지났는지
    float QDurationElapsedTime
    {
        get;
        set;
    }

    /// <summary>
    /// Q 전체 쿨타임
    /// </summary>
    float QCoolTime
    {
        get;
    }

    /// <summary>
    /// E 전체 쿨타임
    /// </summary>
    float ECoolTime
    {
        get;
    }


    /// <summary>
    /// Q 스킬 지속 시간
    /// </summary>
    float QDuration
    {
        get;
    }

    /// <summary>
    /// E 스킬 지속 시간
    /// </summary>
    float EDuration
    {
        get;
    }

}

public class PlayerSkillSystem : MonoBehaviour, ISkillEvent, ISkillTimeData
{
    [SerializeField] private List<GameObject>   _effectObjects;
    [SerializeField] private SkillStrategy      _skillStrategy;    

    [SerializeField, ReadOnly] private float _eCoolTimeElapsed;
    public float ECoolTimeElapsed
    {
        get { return _eCoolTimeElapsed; }
        set { _eCoolTimeElapsed = value; }
    }

    [SerializeField, ReadOnly] private float _qCoolTimeElapsed;
    public float QCoolTimeElapsed
    {
        get { return _qCoolTimeElapsed; }
        set { _qCoolTimeElapsed = value; }
    }

    [SerializeField, ReadOnly] private float _eDurationElapsedTime;
    public float EDurationElapsedTime
    {
        get { return _eDurationElapsedTime; }
        set { _eDurationElapsedTime = value; }
    }

    [SerializeField, ReadOnly]  private float _qDurationElapsedTime;
    public float QDurationElapsedTime
    {
        get { return _qDurationElapsedTime; }
        set { _qDurationElapsedTime = value; }
    }

    //Caching
    private IInputActionControll _inputActionControll;
    private ISkillIndicator _skillIndicator;
    private IStateData _stateData;
    private IBasicData _basicData;
    private IMoveData _moveData;

    private SkillData _skillData => _skillStrategy.SkillData;
    
    public float QCoolTime       => _skillStrategy.SkillData.QCoolTime;
    public float ECoolTime       =>  _skillStrategy.SkillData.ECoolTime;
    public float QDuration      => _skillData.QDuration;
    public float EDuration      => _skillData.EDuration;

    private SkillContext _skillContext;
    [SerializeField] private GameObject _meshRoot;

    void Awake()
    {
        _inputActionControll = GetComponent<IInputActionControll>();
        _basicData = GetComponent<PlayerDataManager>();
        _moveData = GetComponent<PlayerMovement>();
        _stateData = GetComponent<PlayerStateManager>();
        _skillIndicator = GetComponentInChildren<SkillIndicator>(true);

        _eCoolTimeElapsed = _skillData.ECoolTime;
        _qCoolTimeElapsed = _skillData.QCoolTime;
        _skillData.IsEContinue = false;
        _skillData.IsQContinue = false;

        _skillContext = new SkillContext()
        {
            Target = null,
            Caster = gameObject,
            MeshRoot = _meshRoot,
            BasicData = _basicData,
            MoveData = _moveData,
            StateData = _stateData,
            SkillTimeData = this,
            EffectObjects = _effectObjects,
        };

        _skillStrategy.SkillIndicator = _skillIndicator;
    }

    void OnEnable()
    {
        UISystem.Instance._skillTimeData = this;
        _inputActionControll.InputActions.Player.ESkill.performed += OnESkill;
        _inputActionControll.InputActions.Player.QSkill.performed += OnQSkill;
    }


    public void Update()
    {
        ECoolTimer();
        QCoolTimer();
        ESkillTimer();
        QSkillTimer();

        _skillData.UpdateESkill(_skillContext);
        _skillData.UpdateQSkill(_skillContext);
    }
    
    public void FixedUpdate()
    {
        _skillData.FixedUpdateESkill(_skillContext);
        _skillData.FixedUpdateQSkill(_skillContext);
    }


    //스킬 지속 타이머 (자기 자신을 기준으로 스킬이 지속되는 경우에만 실행됩니다.)
    //skillData의 IsContinue 속성이 true가 되면 Duration동안 지속됨.
    //가령 유우카 E 스킬이 쉴드 키는 거라서 쉴드 지속 시간을 체크함
    //데스 모모이처럼 풍차돌리기 하는 것도 이거 킬듯
    private void ESkillTimer()
    {
        if (_skillData.IsEContinue)
        {
            _eDurationElapsedTime += Time.deltaTime;
            if (_eDurationElapsedTime >= _skillData.EDuration)
            {
                OnFinishESkill();
            }
        }
    }

    private void QSkillTimer()
    {
        if (_skillData.IsQContinue)
        {
            _qDurationElapsedTime += Time.deltaTime;
            if (_qDurationElapsedTime >= _skillData.QDuration)
            {
                OnFinishQSkill();
            }
        }
    }

    //쿨타임 타이머.
    private void ECoolTimer()
    {
        _eCoolTimeElapsed = Mathf.Clamp(_eCoolTimeElapsed - Time.deltaTime, 0f, _skillData.ECoolTime);
    }
    private void QCoolTimer()
    {
        _qCoolTimeElapsed = Mathf.Clamp(_qCoolTimeElapsed - Time.deltaTime, 0f, _skillData.QCoolTime);
    }

    //쿨타임 중인지 체크
    private bool CheckCooldown(float coolTimeElapsed, bool isContinue)
    {
        if (coolTimeElapsed != 0f || isContinue) return false;
        else return true;
    }


    public void OnESkill(InputAction.CallbackContext inputContext)
    {
        //공통 동작
        if (!CheckCooldown(_eCoolTimeElapsed, _skillData.IsEContinue))
        {
            print("E Skill is on cooldown");
            return; // Exit if the skill is on cooldown
        }

        //스킬 마다 동작을 어떻게 할 지 정의하는 건 skill strategy에서 정의해야함.

        _skillStrategy.OnESkill(_skillContext, inputContext);
        print("OnESkill called");
        // _skillData.ExecuteESkill(_skillContext, inputContext);
        // print(_basicData.currentShieldCount);
        // print(_basicData.currentShields[0].ShieldEffectObject.Count);
    }

    public void OnQSkill(InputAction.CallbackContext inputContext)
    {
        //공통 동작
        if (!CheckCooldown(_qCoolTimeElapsed, _skillData.IsQContinue))
        {
            print("Q Skill is on cooldown");
            return; // Exit if the skill is on cooldown
        }

        _skillStrategy.OnQSkill(_skillContext, inputContext);
       // OnShowSkillRange(inputContext);
    }


    public void OnFinishESkill()
    {
        //_skillData.FinishESkill(_skillContext);
        _skillStrategy.OnFinishESkill(_skillContext);
        _skillData.IsEContinue = false;
        _eDurationElapsedTime = 0f; // Reset the elapsed time after finishing the skill
        _eCoolTimeElapsed = _skillData.ECoolTime; // Reset the cooldown after finishing the skill
    }

    public void OnFinishQSkill()
    {
        _skillData.FinishQSkill(_skillContext);
        _skillData.IsQContinue = false;
        _qDurationElapsedTime = 0f; // Reset the elapsed time after finishing the skill
        _qCoolTimeElapsed = _skillData.QCoolTime; // Reset the cooldown after finishing the skill
    }


    public void OnShowSkillRange(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;

        switch (actionName)
        {
            case "QSkill":
                _skillIndicator.OnSkillRangeIndicator(_skillData.QSkillRangeIndicator,
                 _skillData.ExecuteQSkill,
                 _skillContext, context,
                 _skillData.QAttackRange,
                 _skillData.QRange);
                break;

            case "ESkill":

                break;
            default:
                Debug.LogError("Unknown action name for skill range display: " + actionName);
                return;
        }       

    }

   


}
