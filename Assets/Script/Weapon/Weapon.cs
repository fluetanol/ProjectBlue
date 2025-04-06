using UnityEngine;


public class Weapon : MonoBehaviour
{
    //무기가 원거리인 경우에만 장착
    protected GameObject ShootingBulletPrefab = null;
    private LayerMask attackMask;

    void Awake()
    {
        print("awake");
    }

    public virtual void Attack() {
        //총알 발사
         GameObject g = Instantiate(ShootingBulletPrefab, transform.position, transform.rotation);
         BasicBullet b = g.GetComponent<BasicBullet>();
         b.bulletDirection = transform.forward;
         b.SetBulletMask(attackMask);

    }

    /// <summary>
    /// 무기를 부르는 첫 시점에만 호출되는 함수입니다.
    /// </summary>
    /// <param name="weaponInfo"></param>
    public virtual void SetWeaponStats(WeaponStats.WeaponInfo weaponInfo) {
        print("set weapon");
        if (ShootingBulletPrefab == null) ShootingBulletPrefab = weaponInfo.BulletPrefab;
        ShootingBulletPrefab.GetComponent<BasicBullet>().SetBulletStats(weaponInfo);
        attackMask = weaponInfo.BasicAttackMask;
    }

}
