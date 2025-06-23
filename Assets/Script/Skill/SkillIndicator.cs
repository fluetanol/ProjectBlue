using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public interface ISkillIndicator
{
    public void OnSkillRangeIndicator(Material indicator,
    Action<SkillContext, InputAction.CallbackContext> activeSkill,
    SkillContext context,
    InputAction.CallbackContext inputActionContext, float range, float skillRange);
}

[RequireComponent(typeof(DecalProjector))]
public class SkillIndicator : MonoBehaviour, ISkillIndicator
{
    [SerializeField] private DecalProjector decalPojector;
    [SerializeField] private DecalProjector _rangeDecalProjector;
    [SerializeField] private float _timeScale = 0.3f;
    [SerializeField] private Ease ease;
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
        print("SkillIndicator enabled");
    }


    void OnDisable()
    {
        Time.timeScale = 1f; // Reset time scale when the indicator is disabled
        print("SkillIndicator Disbled");
    }

    // Update is called once per frame
    void Update()
    {
        
        float distance = Vector3.Distance(_inputData.CursorRaycastingPosition, _moveData.PlayerPosition);
        print(distance);
        if (distance < _rangeDecalProjector.size.x /2f)
        {
            this.transform.position = _inputData.CursorRaycastingPosition;
        }

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


    public void OnSkillRangeIndicator(Material indicator, Action<SkillContext, InputAction.CallbackContext> activeSkill, SkillContext context,
    InputAction.CallbackContext inputActionContext, float range, float skillRange)
    {
        _activeSkillCallback = activeSkill;
        _skillContext = context;
        _inputActionContext = inputActionContext;

        gameObject.SetActive(true); // Enable the GameObject when setting the material
        decalPojector.material = indicator;
        decalPojector.size = new Vector3(range, range, 5f);
        _rangeDecalProjector.enabled = true;
        _rangeDecalProjector.size = new Vector3(skillRange, skillRange, 5f);


    }

    public void ExecuteSkill()
    {
        
    }

}
