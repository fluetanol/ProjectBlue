using System;
using System.Collections;
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

        transform.LookAt(_targetPosition);
        if (_enemyStats[0].EnemyMoveType == EenemyMoveType.linearInterpolation)
            return Vector3.Lerp(_rigidbody.position, _targetPosition,
            Time.fixedDeltaTime * _enemyStats[0]._linearInterpolationMoveSpeed);

        else
        {
            Vector3 direction = _targetPosition - _rigidbody.position;
            direction.Normalize();
            return _rigidbody.position + direction * Time.fixedDeltaTime * _enemyStats[0]._linearMoveSpeed;
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
        print("force!" + direction + " " + force);
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
}
