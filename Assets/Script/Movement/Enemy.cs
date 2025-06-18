using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public abstract class Enemy : MonoBehaviour, IDisposable
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


    void Awake()
    {
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        InitializeStats();
        if (_animator == null) _animator = GetComponent<Animator>();
    }

    protected abstract void InitializeStats();

    public virtual void Dispose()
    {
        gameObject.SetActive(false);
        _animator.SetBool("isDead", false);
        _isDead = false;
    }

}
