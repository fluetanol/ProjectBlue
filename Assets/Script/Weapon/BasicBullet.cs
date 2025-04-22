using System.Collections;
using UnityEngine;

public struct BoxCastInfo
{
    public Vector3 center;
    public Vector3 halfExtents;
    public Quaternion orientation;
    public LayerMask mask;
}


[RequireComponent(typeof(Rigidbody))]
public class BasicBullet : Bullet
{
    private int code = 0;
    [SerializeField] private bool           _isPenetration;
    [SerializeField] private float          _bulletSpeed;
    [SerializeField] private BoxCollider    _boxCollider;
                     private BoxCastInfo    _boxCastInfo;   

    private Rigidbody _rigidbody;
    private Vector3 delta;

    // Update is called once per frame
    void FixedUpdate()
    {
        delta = _bulletSpeed * Time.fixedDeltaTime * bulletDirection;
        UpdateBoxCastInfo();
        BulletCollide();
        _rigidbody.MovePosition(_rigidbody.position + delta);
    }

    protected override void GetBulletComponent()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if(_boxCollider == null) _boxCollider = GetComponent<BoxCollider>();
        MakeBoxCastInfo();
    }

    public void SetBulletPosition(Vector3 position, Quaternion rotation){
        transform.position = position;
        transform.rotation = rotation;
    }

    public override void SetBulletStats(WeaponStats.WeaponInfo weaponInfo){
        base.SetBulletStats(weaponInfo);
        _bulletSpeed = weaponInfo.GetBulletSpeed();
    }


    protected override void BulletCollide(){
        BoxCasting(delta);
        BoxOverlaping();
    }

    public override void SetBulletMask(LayerMask mask){
        base.SetBulletMask(mask);
        _boxCastInfo.mask = mask;
    }

    private void BoxOverlaping()
    {
        Collider[] colliders = Physics.OverlapBox(_boxCastInfo.center, _boxCastInfo.halfExtents,
                                _boxCastInfo.orientation, _boxCastInfo.mask);
        if (colliders.Length > 0)
        {
            if (colliders[0].TryGetComponent(out IDamageable enemy))
            {
               // print("take DMG2");
                enemy.TakeDamage((int)_bulletDamage);
            }

            BulletPoolManager.Instance.Return(_bulletCode, this);
            //Destroy(gameObject);
        }
    }

    private void BoxCasting(Vector3 delta)
    {
        //print("cast : " + _boxCastInfo.mask.value);
        if (Physics.BoxCast(_boxCastInfo.center, _boxCastInfo.halfExtents, delta,
        out RaycastHit hit, _boxCastInfo.orientation, delta.magnitude, _boxCastInfo.mask))
        {

            if (hit.collider.TryGetComponent(out IDamageable enemy))
            {
               // print("take DMG");
                enemy.TakeDamage((int)_bulletDamage);
            }
            BulletPoolManager.Instance.Return(_bulletCode, this);
        }
    }

    private void MakeBoxCastInfo()
    {
        _boxCastInfo = new BoxCastInfo(){
            center = _boxCollider.bounds.center,
            halfExtents = _boxCollider.size * 0.5f,
            orientation = _rigidbody.rotation,
            mask = _bulletMask 
        };
    }

    private void UpdateBoxCastInfo(){
        _boxCastInfo.center = _boxCollider.bounds.center;
    }

    protected override IEnumerator BulletLifeTime(){
        float _bulletLifeTimeCounter = 0;

        while(_bulletLifeTime >= _bulletLifeTimeCounter){
            _bulletLifeTimeCounter+=Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        BulletPoolManager.Instance.Return(_bulletCode, this);
    }

}
