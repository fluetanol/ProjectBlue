using System;
using DG.Tweening;
using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private Transform _target;
    public Transform Target
    {
        get { return _target; }
        set
        {
            _target = value;
        }
    }

    [SerializeField, Range(1, 10)] private float _targetBarDistance = 2.0f;
    public float TargetBarDistance
    {
        get { return _targetBarDistance; }
        set
        {
            _targetBarDistance = value;
        }
    }


    [SerializeField] private bool _isFollow = true;
    public bool IsFollow
    {
        get { return _isFollow; }
        set
        {
            _isFollow = value;
        }
    }

    [SerializeField] private bool _isLookAt = true;
    public bool IsLookAt
    {
        get { return _isLookAt; }
        set
        {
            _isLookAt = value;
        }
    }

    public float SmoothSpeed = 0.125f;
    public Vector3 TargetBarDirection = Vector3.up;
    public Vector3 TargetBarOffset = Vector3.zero;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        FollowControl();
        LookTargetControll();
    }


    // Update is called once per frame
    void LateUpdate()
    {
        FollowControl();
        LookTargetControll();
    }


    private void FollowControl()
    {
        if (IsFollow)
        {
            transform.position = Vector3.Lerp(transform.position,
            _target.position + TargetBarDirection * _targetBarDistance + TargetBarOffset,
            SmoothSpeed * Time.deltaTime);
        }
        if (Camera.main.orthographic)
        {
            Camera.main.orthographicSize = _targetBarDistance;
        }
    }

    private void LookTargetControll()
    {
        if (_isLookAt)
        {
            transform.LookAt(_target);
        }
        else
        {
            Camera.main.transform.forward = -TargetBarDirection;
        }
    }


}
