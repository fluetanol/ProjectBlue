using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
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

    public Queue<Vector3> PathQueue { get; } = new Queue<Vector3>(20);

    [SerializeField] private float _pathfindHeight = 0.25f;
    [SerializeField] private bool _pathfindMode = false;
    [SerializeField] private float _pathfindupdateTime = 0.5f;
    [SerializeField] private float _pathfindTick = 0;
    [SerializeField] private float _pathfindWidth = 0.5f;
    [SerializeField] private bool _isFindPath = false;
    private Vector3 _pathfindTargetPosition;

    void OnEnable()
    {
        gameObject.SetActive(true);
        InitializeStats();
    }

    void FixedUpdate()
    {
        if (_isDead)
        {
            return;
        }

  
        _nextPosition = enemyMove();

        // if (_pathfindMode)
        // {
        //     _pathfindTick += Time.fixedDeltaTime;
        //     if (_pathfindTick >= _pathfindupdateTime)
        //     {
        //         _pathfindTick = 0;
        //         _pathfindMode = false;
        //     }
        // }
        
        Attack();
        //_rigidbody.linearVelocity = (_nextPosition - _rigidbody.position)/Time.fixedDeltaTime;
        //Debug.Log(_nextPosition +" " + _rigidbody.position);
        _rigidbody.MovePosition(_nextPosition);
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

    private Vector3 enemyMove()
    {
        //기본 타겟은 항상 플레이어입니다.
        _targetPosition = _target != null ? _target.position : MoveData.PlayerPosition;


        if (PathQueue.Count > 0 && Vector3.Distance(_rigidbody.position, PathQueue.Peek()) < 0.1f)
        {
            PathQueue.Dequeue();
        }
        // else if(PathQueue.Count > 0)
        // {
        //     Debug.DrawLine(_rigidbody.position, PathQueue.Peek(), Color.red, 0.1f);
        // }
        _targetPosition.y = 0; //y값을 현재 rigidbody의 y값으로 고정
        Vector3 targetDiff  = _targetPosition - _rigidbody.position;
        Vector3 targetDirection = targetDiff.normalized;
        float targetMagnitude = targetDiff.magnitude;
        LayerMask ExceptionGroundMask = LayerMask.GetMask("Building", "Props", "Player");



       _isFindPath = pathFinding(_rigidbody.position, ref _targetPosition, targetDirection, targetMagnitude, _pathfindHeight, _pathfindWidth, ref ExceptionGroundMask, 0);
        

        Vector3 nextPos = PathQueue.Peek();
        nextPos.y = targetDiff.y; //y값을 현재 rigidbody의 y값으로 고정

        Debug.DrawLine(_rigidbody.position, nextPos, Color.green, 0.1f);
        transform.LookAt(PathQueue.Peek());


        if (_enemyStats[0].EnemyMoveType == EenemyMoveType.linearInterpolation)
            return Vector3.Lerp(_rigidbody.position, PathQueue.Peek(),
            Time.fixedDeltaTime * _enemyStats[0]._linearInterpolationMoveSpeed);

        else
        {
            //Vector3 direction = nextPos - _rigidbody.position;
            //direction.Normalize();
            Vector3 direction = transform.forward;
            return _rigidbody.position + direction * Time.fixedDeltaTime * _enemyStats[0]._linearMoveSpeed;
        }
    }


    public bool pathFinding(Vector3 originPosition, ref Vector3 targetPosition, Vector3 targetDirection, float magnitude, float pathfindHeight, float pathfindWidth, ref LayerMask Mask, int repeat)
    {
        bool isPathfind = false;
        Vector3 origin = originPosition;
        origin.y = 0;
        origin.y += pathfindHeight; //y값을 현재 rigidbody의 y값으로 고정
        Ray ray = new Ray(origin, targetDirection);
        if (repeat == 0 && Physics.Raycast(ray, out RaycastHit hit, magnitude + 0.5f, Mask))
        {
            if(repeat == 0 && PathQueue.Count > 0)
            {
                print("큐에 아직 갈 길이 남아있음");
                return false;
            }
            print("hit! pathfind! : " + hit.normal + " " + hit.distance);
            
            Debug.DrawRay(hit.point, hit.normal, Color.red, 2f);


            Vector3 hitnormal = hit.normal;
            hitnormal.y = 0;

            //수직
            Vector3 moveDirection = new Vector3(-hitnormal.z, 0, hitnormal.x).normalized; //수직으로 빼기
            Debug.DrawRay(hit.point, moveDirection, Color.blue, 2f);
            float hitDistance = hit.distance - pathfindWidth;
            Vector3 movePos1 = originPosition + targetDirection * hitDistance + moveDirection * (magnitude - hitDistance);
            Vector3 movePos2 = originPosition + targetDirection * hitDistance - moveDirection * (magnitude - hitDistance);


            Vector3 movePos = Vector3.Distance(movePos1, targetPosition) < Vector3.Distance(movePos2, targetPosition) ? movePos1 : movePos2;


            movePos.y = 0; //y값을 현재 rigidbody의 y값으로 고정
            Vector3 test = (movePos - origin).normalized * _enemyStats[0]._linearMoveSpeed * Time.fixedDeltaTime;
            // PathQueue.Enqueue(movePos);
            PathQueue.Enqueue(origin + test);

            Vector3 NextDirection = (targetPosition - movePos).normalized;
            float NextMagnitude = Vector3.Distance(movePos, targetPosition);
            Debug.DrawLine(originPosition, PathQueue.Peek(), Color.red, 1f);
            return isPathfind = pathFinding(movePos, ref targetPosition, NextDirection, NextMagnitude, pathfindHeight, pathfindWidth, ref Mask, repeat + 1);

        }
        else
        {
            if (repeat > 0)
            {
                isPathfind = true;
            }
            else
            {
                Debug.Log("뭣?");
                PathQueue.Clear();
            }

            PathQueue.Enqueue(origin + targetDirection * magnitude);
            
        }

        return isPathfind;
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
}
