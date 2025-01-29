using UnityEngine;

interface IAttackable
{
    void Attack();
}

public class Weapon : MonoBehaviour, IAttackable
{
    //무기가 원거리인 경우에만 장착
    private GameObject ShootingBulletPrefab;

    public void Attack() {
        //총알 발사
         GameObject g = Instantiate(ShootingBulletPrefab, transform.position, transform.rotation);
         Bullet b = g.GetComponent<Bullet>();
         b.bulletDiretion = transform.forward;
    }

    public void SetWeaponStats(WeaponStats.WeaponInfo weaponInfo) {
        ShootingBulletPrefab = weaponInfo.BulletPrefab;
        ShootingBulletPrefab.GetComponent<Bullet>().SetBulletStats(weaponInfo);
    }

}

