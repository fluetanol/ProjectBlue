using UnityEngine;

/// <summary>
/// 데미지를 받을 수 있는 객체에 대한 인터페이스
/// </summary>
public interface IDamageable
{
    void TakeDamage(float amage);
}

/// <summary>
/// 치료받을 수 있는 객체에 대한 인터페이스
/// </summary>
public interface IHealable
{
    void TakeHeal(float heal);
}

/// <summary>
/// 넉백을 받을 수 있는 객체에 대한 인터페이스
/// </summary>
interface IForceable
{
    void Knockback(Vector3 direction, float force);
    void Airborne(float force);
}
/// <summary>
    /// 공격 가능한 객체에 대한 인터페이스
    /// </summary>
    interface IAttackable
    {
        void Attack();
    }


public interface IPoolable { }
public interface IPoolable<T> : IPoolable where T : MonoBehaviour
{
    void Add(T obj);
    T Get();
    void Return(T obj);
}