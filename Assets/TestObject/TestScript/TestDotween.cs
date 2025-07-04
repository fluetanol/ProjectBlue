
using DG.Tweening;
using UnityEngine;

public class TestDotween : MonoBehaviour
{
    public Vector3 targetPosition;
    Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();


        DOTween.To(() => _rigidbody.position,
            (pos) => _rigidbody.MovePosition(pos),
            targetPosition,
            1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutFlash);



        
        
    }
}
