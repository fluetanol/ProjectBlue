using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amage);
}

public interface IHealable
{
    void TakeHeal(int heal);
}