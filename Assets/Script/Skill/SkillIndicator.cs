using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public interface ISkillIndicator
{
    /// <summary>
    /// Skill Range Indeicator 켜는 메서드
    /// </summary>
    /// <param name="indicator">indicator로 쓸 머터리얼 </param>
    /// <param name="activeSkill">범위를 찍을 때 실행 할 스킬 로직</param>
    /// <param name="context">스킬 context </param>
    /// <param name="inputActionContext">input context </param>
    /// <param name="range">indicator가 돌아다닐 수 있는 전체 범위</param>
    /// <param name="skillRange">indicator의 크기</param>
    public void OnSkillRangeIndicator(Material indicator,
    Action<SkillContext, InputAction.CallbackContext> activeSkill,
    SkillContext context,
    InputAction.CallbackContext inputActionContext, float range, float skillRange);


}

[RequireComponent(typeof(DecalProjector))]
public class SkillIndicator : MonoBehaviour, ISkillIndicator
{
    //스킬 적용 범위 표시용 데칼 프로젝터
    [SerializeField] private DecalProjector decalPojector;
    //indicator 이동 가능 영역 범위 표시용 데칼 프로젝터
    [SerializeField] private DecalProjector _rangeDecalProjector;
    
    [SerializeField] private float _timeScale = 0.3f;

    //for skill caching
    private IInputActionControll _inputActionControll;
    private IInputData _inputData;
    private IMoveData _moveData;
    private event Action<SkillContext, InputAction.CallbackContext> _activeSkillCallback;
    private SkillContext _skillContext;
    private InputAction.CallbackContext _inputActionContext;

    void Awake()
    {
        decalPojector = GetComponent<DecalProjector>();
        if (decalPojector == null)
        {
            Debug.LogError("DecalProjector component is missing on the GameObject.");
        }

        _inputData = GetComponentInParent<IInputData>();
        _inputActionControll = GetComponentInParent<IInputActionControll>();
        if (_inputData == null)
        {
            Debug.LogError("IInputData component is missing on the parent GameObject.");
        }


        _moveData = GetComponentInParent<IMoveData>();
        if (_moveData == null)
        {
            Debug.LogError("IMoveData component is missing on the parent GameObject.");
        }

        //gameObject.SetActive(false); // Initially disable the GameObject
    }

    void OnEnable()
    {
        Time.timeScale = _timeScale;

        EnvironmentSystem.instance.BloomIntensity = 1.25f;
        EnvironmentSystem.instance.LightIntensity = 0.5f;
        print("SkillIndicator enabled");
    }


    void OnDisable()
    {
        Time.timeScale = 1f; // Reset time scale when the indicator is disabled
        print("SkillIndicator Disbled");
        EnvironmentSystem.instance.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        IndicatorRangeManage();
        OnClickTrigger();
    }





    public void OnSkillRangeIndicator(Material indicator, Action<SkillContext, InputAction.CallbackContext> activeSkill, SkillContext context,
    InputAction.CallbackContext inputActionContext, float range, float skillRange)
    {
        _activeSkillCallback = activeSkill;
        _skillContext = context;
        _inputActionContext = inputActionContext;

        gameObject.SetActive(true); // Enable the GameObject when setting the material
        InitializeDecalProjector(indicator, new Vector3(range, range, 5f));
        InitializeRangeDecalProjector(new Vector3(skillRange, skillRange, 5f));
    }


    private void InitializeDecalProjector(Material material, Vector3 size)
    {
        decalPojector.material = material;
        decalPojector.size = size;
    }

    private void InitializeRangeDecalProjector(Vector3 size)
    {
        _rangeDecalProjector.enabled = true;
        _rangeDecalProjector.size = size;
    }

    private void IndicatorRangeManage()
    {
        float distance = Vector3.Distance(_inputData.CursorRaycastingPosition, _moveData.PlayerPosition);
        print(distance);
        if (distance < _rangeDecalProjector.size.x / 2f)
        {
            this.transform.position = _inputData.CursorRaycastingPosition;
        }
    }

    private void OnClickTrigger()
    {
        if (_inputActionControll.IsClicked)
        {
            gameObject.SetActive(false); // Disable the GameObject after invoking the skill
            _skillContext.TargetPosition = _inputData.CursorRaycastingPosition;
            _activeSkillCallback?.Invoke(_skillContext, _inputActionContext);
            _rangeDecalProjector.enabled = false; // Disable the range decal projector

            _skillContext.SkillTimeData.QElapsedTime = 0f;
            _skillContext.SkillTimeData.QCoolTimeElapsed = _skillContext.SkillTimeData.QCoolTime;
        }
    }

}
