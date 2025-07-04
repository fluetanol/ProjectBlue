using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;



[CreateAssetMenu(fileName = "Yuuka", menuName = "Skill/YuukaSkill")]
public class YuukaSkill : SkillData
{
    private int shieldIdx = 0;

    [Header("For Q Skill Animation")]
    [SerializeField] private float QAnimDelay;
    [SerializeField] private float QCameraDelay;
    [SerializeField] private float QCameraFarDistance;
    [SerializeField] private float QCameraNearDistance;

    public override void ExecuteESkill(SkillContext context, InputAction.CallbackContext inputContext)
    {
        shieldIdx = ShieldCondition.AddShield(context.BasicData.currentShields, new ShieldCondition()
        {
            ShieldValue = EskillDataInfo.Shield,
            ShieldDuration = EskillDataInfo.Duration,
            ShieldEffectObject = context.EffectObjects,
            ShieldRemoveEvents = () =>
            {
                ShieldRemove(context);
            }
        });

        context.BasicData.currentShieldCount++;
        context.BasicData.currentShieldHeadIdx = ShieldCondition.MaxShieldIdx(context.BasicData.currentShields);

        Debug.Log(context.BasicData.currentShields[shieldIdx].ShieldEffectObject.Count);

        foreach (var effect in context.EffectObjects)
        {
            if (effect != null)
            {
                effect.SetActive(true);
            }
        }
        IsEContinue = true;
    }


    public override void FinishESkill(SkillContext context)
    {
        ShieldCondition.RemoveShield(context.BasicData.currentShields, shieldIdx);
    }

    public override void ExecuteQSkill(SkillContext context, InputAction.CallbackContext inputContext)
    {
        //throw new System.NotImplementedException();
        Debug.Log("Q Skill Executed : " + context.TargetPosition);

        Transform casterTransform = context.Caster.transform;
        Vector3 targetPosition = context.TargetPosition;
        QSkillAnimation(casterTransform, targetPosition, context);
    }


    private void ShieldRemove(SkillContext context)
    {
        context.BasicData.currentShieldCount--;
        context.BasicData.currentShieldHeadIdx = ShieldCondition.MaxShieldIdx(context.BasicData.currentShields);
        context.SkillTimeData.EDurationElapsedTime = 0f;
        context.SkillTimeData.ECoolTimeElapsed = ECoolTime;
        IsEContinue = false;
    }


    private void QSkillAnimation(Transform casterTransform, Vector3 targetPosition, SkillContext context)
    {
        Debug.Log("Creating new Q Skill Movement Sequence");

        Rigidbody casterRigidbody = casterTransform.GetComponent<Rigidbody>();
        context.StateData.CanMove = false;

        float originalCameraDistance = CameraController.Instance.TargetBarDistance;
        Sequence moveSequence = DOTween.Sequence();

        float originalSmoothSpeed = CameraController.Instance.SmoothSpeed;
        CameraController.Instance.SmoothSpeed = 6f; 

        moveSequence
            //선 딜레이
            .Append(casterRigidbody.DOMoveY(casterRigidbody.position.y + 5f, 0.3f).SetEase(Ease.OutFlash))
            .Join(DOTween.To(() => CameraController.Instance.TargetBarDistance, x => CameraController.Instance.TargetBarDistance = x, QCameraFarDistance, 0.5f)).SetEase(Ease.InFlash)
            .AppendInterval(0.15f)
            //돌진
            .Append(casterRigidbody.DOMove(targetPosition, 1f).SetEase(Ease.InOutExpo))
            .Join(DOTween.To(() => CameraController.Instance.TargetBarDistance, x => CameraController.Instance.TargetBarDistance = x, QCameraNearDistance, 0.5f)).SetEase(Ease.InFlash)
            
            .Append(Camera.main.DOShakePosition(0.3f, 0.8f, 30))
            //공격 판정
            .OnComplete(() =>
            {
                Debug.Log("Q Skill Movement Completed");
                context.StateData.CanMove = true;
                Collider[] colliders = Physics.OverlapSphere(casterTransform.position, QAttackRange / 2, LayerMask.GetMask("Enemy"));
                foreach (var collider in colliders)
                {
                    collider.GetComponent<IForceable>()?.Airborne(9f);
                    collider.GetComponent<IDamageable>()?.TakeDamage(QskillDataInfo.Damage);
                }
                //casterTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
                DOTween.To(
                    () => CameraController.Instance.TargetBarDistance, x => CameraController.Instance.TargetBarDistance = x, originalCameraDistance, 0.25f)
                .SetEase(Ease.InExpo);

                CameraController.Instance.SmoothSpeed = originalSmoothSpeed;
            });
    }
}
