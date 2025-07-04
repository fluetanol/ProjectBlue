
using DG.Tweening;
using UnityEngine;

public class TestDotween : MonoBehaviour
{
    public Vector3 targetPosition;
    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();


        DOTween.To(() => rigidbody.position,
            (pos) => rigidbody.MovePosition(pos),
            targetPosition,
            2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutExpo);



        
        
    }
}
