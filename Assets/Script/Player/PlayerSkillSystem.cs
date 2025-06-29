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
    [SerializeField] private SkillStrategy _skillStrategy;

    
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

    
    [SerializeField] private float _eElapsedTime;
    public float EElapsedTime
    {
        get { return _eElapsedTime; }
        set { _eElapsedTime = value; }
    }
    [SerializeField]  private float _qElapsedTime;
    public float QElapsedTime
    {
        get { return _qElapsedTime; }
        set { _qElapsedTime = value; }
    }

    //Caching
    private SkillData _skillData => _skillStrategy.SkillData;
    public float QCoolTime => _skillStrategy.SkillData.QCoolTime;
    public float ECoolTime => _skillStrategy.SkillData.ECoolTime;

    private SkillContext _skillContext;

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
            BasicData = _basicData,
            MoveData = _moveData,
            StateData = _stateData,
            SkillTimeData = this,
            EffectObjects = effectObjects,
        };

        _skillStrategy.SkillIndicator = _skillIndicator;
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
            _eElapsedTime += Time.deltaTime;
            if (_eElapsedTime >= _skillData.EDuration)
            {
                OnFinishESkill();
            }
        }
    }

    private void QSkillTimer()
    {
        if (_skillData.IsQContinue)
        {
            _qElapsedTime += Time.deltaTime;
            if (_qElapsedTime >= _skillData.QDuration)
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
        _eElapsedTime = 0f; // Reset the elapsed time after finishing the skill
        _eCoolTimeElapsed = _skillData.ECoolTime; // Reset the cooldown after finishing the skill
    }

    public void OnFinishQSkill()
    {
        _skillData.FinishQSkill(_skillContext);
        _skillData.IsQContinue = false;
        _qElapsedTime = 0f; // Reset the elapsed time after finishing the skill
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
