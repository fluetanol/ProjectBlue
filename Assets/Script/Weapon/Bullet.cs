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

    void Awake() {
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
    }
    
    protected virtual void GetBulletComponent(){ 

    }

}
