using System;
using System.Collections;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] protected float _bulletDamage;
    [SerializeField] protected float _bulletLifeTime;
    [SerializeField] protected LayerMask _bulletMask;

    public Vector3 bulletDirection
    {
        get;
        set;
    }

    void Awake() {
        print("Awake !!");
        GetBulletComponent();
    }

    void OnEnable(){
        StartCoroutine(BulletLifeTime());
    }


    
    protected abstract void BulletCollide();
    protected abstract IEnumerator BulletLifeTime();


    public virtual void SetBulletStats(WeaponStats.WeaponInfo weaponInfo){
        _bulletLifeTime = weaponInfo.GetBulletLifeTime();
        _bulletDamage = weaponInfo.Damage;
        _bulletMask = weaponInfo.BasicAttackMask;
    }
    

    protected virtual void GetBulletComponent(){ 

    }


}
