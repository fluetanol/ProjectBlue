using System;
using DG.Tweening;
using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    public Transform Target;
    public float SmoothSpeed = 0.125f;

    public Vector3 TargetBarDirection = Vector3.up;
    public Vector3 TargetBarOffset = Vector3.zero;
    [Range(1, 10)] public float TargetBarDistance = 2.0f;

    public bool IsFollow = true;
    public bool IsSky = true;
    public bool IsLookAt = false;


    private bool test = false;
    void Awake()
    {
        transform.position = Target.position + TargetBarDirection * TargetBarDistance + TargetBarOffset;
    }

    public void SetTarget(Transform target)
    {
        Target = target;
    }

    void Update()
    {
        if (IsFollow)
        {
            transform.position = Vector3.Lerp(transform.position,
            Target.position + TargetBarDirection * TargetBarDistance + TargetBarOffset,
            SmoothSpeed * Time.deltaTime);
        }

        if (Time.time > 5 && !test)
        {
            print("Camera Shake");
            Camera.main.DOShakePosition(1f, 2f).SetEase(Ease.InOutSine);
            test = true;
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (IsLookAt)
        {
            transform.LookAt(Target);
        }

    }


}
