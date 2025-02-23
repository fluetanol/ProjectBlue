using UnityEngine;


public class Weapon : MonoBehaviour
{
    //무기가 원거리인 경우에만 장착
    protected GameObject ShootingBulletPrefab = null;

    public virtual void Attack() {
        //총알 발사
         GameObject g = Instantiate(ShootingBulletPrefab, transform.position, transform.rotation);
         BasicBullet b = g.GetComponent<BasicBullet>();
         b.bulletDiretion = transform.forward;
    }

    public virtual void SetWeaponStats(WeaponStats.WeaponInfo weaponInfo) {
        if(ShootingBulletPrefab == null) ShootingBulletPrefab = weaponInfo.BulletPrefab;
        ShootingBulletPrefab.GetComponent<BasicBullet>().SetBulletStats(weaponInfo);
    }
}
