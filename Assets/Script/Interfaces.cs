using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amage);
}

public interface IHealable
{
    void TakeHeal(float heal);
}