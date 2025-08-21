using System;
using System.Collections;
using System.Collections.Generic;
using Unity.InferenceEngine;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public interface IPoolCreatable
{
    void PoolingCreate();
}

public interface IEnemyData
{
    public float RestHealth { get; }
    public float MaxHealth { get; }
}

public class EnemyMovement : Enemy, IDamageable, IForceable, IAttackable
{

    [Header("Setting Enemy Move Stats Scriptable Object")]
    [SerializeField] private EnemyStats _enemyStats;
    private Vector3 _targetPosition, _nextPosition;
    private bool _isAttacking = false;

    public override float MaxHealth => _enemyStats[EnemyCode].EnemyHealth;

    [SerializeField] private float _enemyHeight = 0.25f;
    [SerializeField] private bool _pathfindMode = false;
    [SerializeField] private float _pathfindupdateTime = 0.5f;


    [SerializeField] private float _pathfindTick = 0;
    [SerializeField] private bool _isFindPath = false;

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private LayerMask _hitLayerMask;

    private NavMeshPath _path;
    private float _pathTick = 0;


    void OnEnable()
    {
        gameObject.SetActive(true);
        InitializeStats();
    }

    void Start()
    {
        // _agent = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();
    }

    void FixedUpdate()
    {
        if (_isDead)
        {
            return;
        }

        _pathTick += Time.fixedDeltaTime;

        Vector3 delta = enemyMove();
        // if (IsAbleMoveDirection(delta))
        // {
        // _agent.isStopped = true;

        _nextPosition = _rigidbody.position + delta;
        transform.LookAt(_nextPosition);
        if (Physics.Raycast(_nextPosition + transform.forward  + Vector3.up * 0.75f, Vector3.down, out RaycastHit hit, 10f, LayerMask.GetMask("Ground")))
        {
            print("hit point : " + hit.point);
            _nextPosition.y = hit.point.y;
        }
        Debug.DrawRay(_nextPosition + transform.forward + Vector3.up * 0.75f, Vector3.down * 10f, Color.green, 0.3f);
        Attack();
        _rigidbody.MovePosition(_nextPosition);
        // }
         
        //  else
        //  {
        //      print("unable to move!");

        //     _targetPosition = _target != null ? _target.position : MoveData.PlayerPosition;
        //      _agent.CalculatePath(_targetPosition, _path);
        //      _agent.SetPath(_path);

        //     Attack();
        //     Debug.DrawLine(_rigidbody.position, _path.corners[0], Color.red, 0.3f);
        //     for (int i = 1; i < _path.corners.Length; i++)
        //     {
        //         Vector3 corner = _path.corners[i];
        //         print("corner " + corner);
        //         Debug.DrawLine(_path.corners[i - 1], corner, Color.red, 0.3f);
        //     }
        //  }

    }


    protected sealed override void InitializeStats()
    {
        health = _enemyStats[EnemyCode].EnemyHealth;
        damage = _enemyStats[EnemyCode].EnemyDamage;
        dmgTick = _enemyStats[EnemyCode].EnemyDmgTick;
    }


    public void TakeDamage(float damage)
    {
        //StartCoroutine(TEST());
        health -= damage;
        DeadCheck();
    }

    private void DeadCheck()
    {
        if (health <= 0)
        {
            _isDead = true;
            _animator.SetBool("isDead", _isDead);
        }
    }

    private bool IsAbleMoveDirection(Vector3 direction)
    {
        Ray ray = new Ray(_rigidbody.position + Vector3.up * _enemyHeight, direction);
        return !Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, _hitLayerMask);
    }


    /// <summary>
    /// 적의 다음 위치를 구합니다.
    /// </summary>
    /// <returns>다음 위치를 가기 위한 delta값, 즉 currentPosition + delta = nextPosition</returns>
    private Vector3 enemyMove()
    {
        //기본 타겟은 항상 플레이어입니다.
        _targetPosition = _target != null ? _target.position : MoveData.PlayerPosition;

        if (_enemyStats[0].EnemyMoveType == EenemyMoveType.linearInterpolation)
        {
            return MoveByInterpolate();
        }

        else
        {
            return MoveByLinear();
        }
    }

    //근접 공격
    public void Attack()
    {
        if (_isAttacking)
        {
            return;
        }

        //공격 로직
        Vector3 newdirection = (_nextPosition - _rigidbody.position).normalized;
        if (Physics.Raycast(_rigidbody.position + Vector3.up * 0.75f, newdirection, out RaycastHit hit,
        Vector3.Distance(_rigidbody.position, _nextPosition) + 1f,
        LayerMask.GetMask("Player")))
        {
            StartCoroutine(AttackTimer());
            hit.collider.GetComponent<IDamageable>().TakeDamage(damage);
            _animator.SetBool("isAttack", true);
        }

        //Debug.DrawRay(_rigidbody.position, newdirection * (Vector3.Distance(_rigidbody.position, _nextPosition)+1f), Color.red,3f);
    }

    //현재 enemy는 dynamic 모드라서 addforce로 처리하지만
    //kinematic으로 변경해야 하는 순간이 온다면 코드 바꿔야 할 예정
    public void Knockback(Vector3 direction, float force)
    {
        //print("force!" + direction + " " + force);
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
    }

    private IEnumerator AttackTimer()
    {
        _isAttacking = true;
        yield return new WaitForSeconds(attackTick);
        _isAttacking = false;
        _animator.SetBool("isAttack", _isAttacking);
    }

    public void PoolingCreate()
    {
        gameObject.SetActive(true);
        InitializeStats();
    }

    public void Airborne(float force)
    {
        print("airborne force : " + force);
        _rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    public override void Dispose()
    {
        base.Dispose();

        transform.parent.gameObject.SetActive(false);
    }


    private Vector3 MoveByInterpolate()
    {
        return _rigidbody.position - Vector3.Lerp(_rigidbody.position, _targetPosition,
            Time.fixedDeltaTime * _enemyStats[0]._linearInterpolationMoveSpeed);
    }

    private Vector3 MoveByLinear()
    {
        Vector3 direction = _targetPosition - _rigidbody.position;
        direction.Normalize();
        return direction * Time.fixedDeltaTime * _enemyStats[0]._linearMoveSpeed;
    }

}
