using UnityEngine;

public class LaserGun : Weapon
{
    public static GameObject LaserBullet = null;
    private LaserBullet laserBullet;

    public override void Attack(){
        laserBullet.TickAttack();   
    }

    public override void SetWeaponStats(WeaponStats.WeaponInfo weaponInfo){
        if (ShootingBulletPrefab == null) ShootingBulletPrefab = weaponInfo.BulletPrefab;
        if (LaserBullet == null)
        {
            LaserBullet = Instantiate(ShootingBulletPrefab, transform.position, transform.rotation);
            LaserBullet.transform.parent = this.transform;
            laserBullet = LaserBullet.GetComponent<LaserBullet>();
            laserBullet.SetBulletStats(weaponInfo);
        }
    }
}
