using System.Collections;
using UnityEngine;

public class LaserBullet : Bullet
{
    void Start()
    {
        
    }

    public override void SetBulletStats(WeaponStats.WeaponInfo weaponInfo)
    {
        throw new System.NotImplementedException();
    }

    protected override void BulletCollide()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator BulletLifeTime()
    {
        throw new System.NotImplementedException();
    }
}
