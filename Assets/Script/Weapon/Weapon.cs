using UnityEngine;


public class Weapon : MonoBehaviour
{
    //무기가 원거리인 경우에만 장착
   // protected GameObject ShootingBulletPrefab = null;
    protected GameObject ShootingBulletPrefab;
    private LayerMask attackMask;
    private WeaponStats.WeaponInfo weaponInfo;

    void Awake()
    {
        print("awake");
    }

    public virtual void Attack() {
        //총알 발사
         BasicBullet b = BulletPoolManager.Instance.Get<BasicBullet>(0);
        //Instantiate(ShootingBulletPrefab, transform.position, transform.rotation);

        b.transform.parent = null;
        b.SetBulletPosition(transform.position, transform.rotation);
        b.SetBulletStats(weaponInfo);
       b.bulletDirection = transform.forward;
        b.SetBulletMask(attackMask);
        b.gameObject.SetActive(true); // 여기서 마지막에 활성화
        print("bullet create " + b.transform.position + " " + transform.position);

    }

    /// <summary>
    /// 무기를 부르는 첫 시점에만 호출되는 함수입니다.
    /// </summary>
    /// <param name="weaponInfo"></param>
    public virtual void SetWeaponStats(WeaponStats.WeaponInfo weaponInfo) {
        print("set weapon");
        //if (ShootingBulletPrefab == null) ShootingBulletPrefab = weaponInfo.BulletPrefab;
        //ShootingBulletPrefab.GetComponent<BasicBullet>().SetBulletStats(weaponInfo);
        this.weaponInfo = weaponInfo;
        this.attackMask = weaponInfo.BasicAttackMask;
    }

}
