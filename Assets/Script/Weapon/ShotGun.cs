using UnityEngine;

public class ShotGun : Weapon
{
    public override void Attack()
    {
        //GameObject g = Instantiate(ShootingBulletPrefab, transform.position, transform.rotation);
        print("shotgun attack");
        Bullet shotgunBullet  = BulletPoolManager.Instance.Get(1, transform.position, transform.rotation, true);
        shotgunBullet.SetBulletStats(weaponInfo);
        shotgunBullet.SetBulletMask(attackMask);

    }

    public override void SetWeaponStats(WeaponStats.WeaponInfo weaponInfo)
    {
        if (ShootingBulletPrefab == null) ShootingBulletPrefab = weaponInfo.BulletPrefab;
        this.attackMask = weaponInfo.BasicAttackMask;
        this.weaponInfo = weaponInfo;
    }
}
