using System.Collections;
using UnityEngine;


public struct BoxCastInfo
{
    public Vector3 center;
    public Vector3 halfExtents;
    public Quaternion orientation;
    public LayerMask mask;
}

public static class ExtensionPhysics{
    public static bool BoxCasts(this Physics physics, ref BoxCastInfo boxCastInfo, Vector3 direction, float distance){
        return Physics.BoxCast(boxCastInfo.center, boxCastInfo.halfExtents, direction, 
        out RaycastHit hit, boxCastInfo.orientation, distance, boxCastInfo.mask);
    }
}


[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{


    [SerializeField] private bool           _isPenetration;
    [SerializeField] private float          _bulletSpeed;
    [SerializeField] private float          _bulletDamage;
    [SerializeField] private float          _bulletLifeTime;
    [SerializeField] private BoxCollider    _boxCollider;
                     private BoxCastInfo    _boxCastInfo;   

    public Vector3 bulletDiretion{
        get;
        set;
    }

    float _bulletLifeTimeCounter = 0f;
    private Rigidbody _rigidbody;

    void Start(){
         _rigidbody = GetComponent<Rigidbody>();
        if(_boxCollider == null) _boxCollider = GetComponent<BoxCollider>();
        MakeBoxCastInfo();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 delta = _bulletSpeed * Time.fixedDeltaTime * bulletDiretion;
        UpdateBoxCastInfo();
        BoxCasting(delta);
        BoxOverlaping();
        _rigidbody.MovePosition(_rigidbody.position + delta);
        LifeTimeCounter();
    }


    private void BoxOverlaping()
    {
        Collider[] colliders = Physics.OverlapBox(_boxCastInfo.center, _boxCastInfo.halfExtents,
                                _boxCastInfo.orientation, _boxCastInfo.mask);
        if (colliders.Length > 0)
        {
            if (colliders[0].TryGetComponent(out EnemyMovement enemy))
            {
               // print("take DMG2");
                enemy.TakeDamage((int)_bulletDamage);
            }

            Destroy(gameObject);
        }
    }

    private void BoxCasting(Vector3 delta)
    {
        if (Physics.BoxCast(_boxCastInfo.center, _boxCastInfo.halfExtents, delta,
        out RaycastHit hit, _boxCastInfo.orientation, delta.magnitude, _boxCastInfo.mask))
        {

            if (hit.collider.TryGetComponent(out EnemyMovement enemy))
            {
               // print("take DMG");
                enemy.TakeDamage((int)_bulletDamage);
            }
            Destroy(gameObject);
        }
    }

    private void MakeBoxCastInfo()
    {
        _boxCastInfo = new BoxCastInfo(){
            center = _boxCollider.bounds.center,
            halfExtents = _boxCollider.size * 0.5f,
            orientation = _rigidbody.rotation,
            mask = LayerMask.GetMask("Enemy", "Obstacle")
        };
    }

    private void UpdateBoxCastInfo(){
        _boxCastInfo.center = _boxCollider.bounds.center;
    }

    private void LifeTimeCounter(){
        _bulletLifeTimeCounter += Time.fixedDeltaTime;
        if (_bulletLifeTime <= _bulletLifeTimeCounter) Destroy(gameObject);
    }

}
