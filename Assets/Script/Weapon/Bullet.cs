using System;
using System.Collections;
using UnityEngine;

public abstract class Bullet : MonoBehaviour, IDisposable
{
    [SerializeField] protected int _bulletCode;
    [SerializeField] protected float _bulletDamage;
    [SerializeField] protected float _bulletLifeTime;
    [SerializeField] protected LayerMask _bulletMask;

    protected IMoveData moveData;

    public Vector3 bulletDirection
    {
        get;
        set;
    }

    protected virtual void Awake()
    {
        print("Awake !!");
        GetBulletComponent();
    }

    protected virtual void OnEnable(){
        StartCoroutine(BulletLifeTime());
    }

    protected abstract void BulletCollide();
    protected abstract IEnumerator BulletLifeTime();

    public virtual void SetBulletStats(WeaponStats.WeaponInfo weaponInfo)
    {
        _bulletCode = weaponInfo.WeaponCode;
        _bulletLifeTime = weaponInfo.GetBulletLifeTime();
        _bulletDamage = weaponInfo.Damage;
    }
    
    public void SetMoveData(IMoveData moveData)
    {
        print("SetMoveData !!");
        this.moveData = moveData;
    }

    protected virtual void GetBulletComponent() { }

    public virtual void SetBulletMask(LayerMask mask){
        _bulletMask = mask;
    }

    public void Dispose()
    {
       // BulletPoolManager.Instance.Return(_bulletNum, this);
    }
}
