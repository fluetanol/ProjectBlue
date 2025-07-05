using System;
using UnityEngine;


public abstract class Enemy : MonoBehaviour, IDisposable, IEnemyData
{
    public Transform Testobj;

    [Header("Enemy Basic Stats")]
    public int EnemyCode;
    public float health = 3;
    public int damage = 1;
    public float attackTick;
    public float dmgTick;

    [Header("Enemy Basic Components")]
    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected Rigidbody _target;

    [Header("DI")]
    public IMoveData MoveData;

    protected bool _isDead = false;

    public float RestHealth => health;
    public virtual float MaxHealth => 0;

    void Awake()
    {
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        if (_animator == null) _animator = GetComponent<Animator>();
       // InitializeStats();
    }

    protected abstract void InitializeStats();

    public virtual void Dispose()
    {
        gameObject.SetActive(false);
        _animator.SetBool("isDead", false);
        _isDead = false;
    }

}
