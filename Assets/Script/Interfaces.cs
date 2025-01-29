using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);
}

public interface IHealable
{
    void TakeHeal(int heal);
}