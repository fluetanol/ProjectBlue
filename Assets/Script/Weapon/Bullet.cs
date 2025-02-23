using System.Collections;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] protected float _bulletDamage;
    [SerializeField] protected float _bulletLifeTime;

    public Vector3 bulletDiretion
    {
        get;
        set;
    }

    void OnEnable(){
        GetBulletComponent();
        StartCoroutine(BulletLifeTime());
    }
    
    public abstract void SetBulletStats(WeaponStats.WeaponInfo weaponInfo);
    
    protected abstract void BulletCollide();
    protected abstract IEnumerator BulletLifeTime();
    protected virtual void GetBulletComponent(){ }

}
