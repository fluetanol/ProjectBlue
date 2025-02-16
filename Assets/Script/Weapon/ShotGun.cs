using UnityEngine;

public class ShotGun : Weapon
{
    public override void Attack()
    {
        GameObject g = Instantiate(ShootingBulletPrefab, transform.position, transform.rotation);
    }

    public override void SetWeaponStats(WeaponStats.WeaponInfo weaponInfo)
    {
        if (ShootingBulletPrefab == null) ShootingBulletPrefab = weaponInfo.BulletPrefab;
        ShootingBulletPrefab.GetComponent<ShotBullet>().SetBulletStats(weaponInfo);
    }
}
