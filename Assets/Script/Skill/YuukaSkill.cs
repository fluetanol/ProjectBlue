using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;



[CreateAssetMenu(fileName = "Yuuka", menuName = "Skill/YuukaSkill")]
public class YuukaSkill : SkillData
{
    private int shieldIdx = 0;
    private Sequence moveSequence;

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
        context.SkillTimeData.EElapsedTime = 0f;
        context.SkillTimeData.ECoolTimeElapsed = ECoolTime;
        IsEContinue = false;
    }


    private void QSkillAnimation(Transform casterTransform, Vector3 targetPosition, SkillContext context)
    {

            Debug.Log("Creating new Q Skill Movement Sequence");

            Rigidbody casterRigidbody = casterTransform.GetComponent<Rigidbody>();
            context.StateData.CanMove = false;
            Sequence moveSequence = DOTween.Sequence();
            
            moveSequence.Append(casterRigidbody.DOMoveY(casterRigidbody.position.y + 5f, 1f).SetEase(Ease.OutQuad))
                //.Join(Camera.main.DOFieldOfView(35f, 0.35f)).SetEase(Ease.InQuad)
                .Append(casterRigidbody.DOMove(targetPosition, 0.2f).SetEase(Ease.InOutFlash))
                //.Join(Camera.main.DOFieldOfView(45f, 0.1f)).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    Debug.Log("Q Skill Movement Completed");
                    context.StateData.CanMove = true;
                    Collider[] colliders = Physics.OverlapSphere(casterTransform.position, QAttackRange/2, LayerMask.GetMask("Enemy"));
                    foreach (var collider in colliders)
                    {
                        collider.GetComponent<IForceable>()?.Airborne(10f);
                        collider.GetComponent<IDamageable>()?.TakeDamage(QskillDataInfo.Damage);
                    }
                    IsQContinue = false;
                });
    }
}
