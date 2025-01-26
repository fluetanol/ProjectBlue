using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private bool  _isPenetration;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletDamage;
    [SerializeField] private float _bulletLifeTime;
    [SerializeField] private BoxCollider _boxCollider;
    
    public Vector3 bulletDiretion{
        get;
        set;
    }

    float _bulletLifeTimeCounter = 0f;
    private Rigidbody _rigidbody;

    void Start(){
         _rigidbody = GetComponent<Rigidbody>();
        if(_boxCollider == null) _boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 delta = _bulletSpeed * Time.fixedDeltaTime * bulletDiretion;

        Vector3 center, halfExtents;
        Quaternion orientation;
        LayerMask mask;
        BoxCastInfo(out center, out halfExtents, out orientation, out mask);

        if (Physics.BoxCast(center, halfExtents, delta, out RaycastHit hit, orientation, delta.magnitude, mask ))
        {
            if (hit.collider.TryGetComponent(out EnemyMovement enemy))
                enemy.TakeDamage((int)_bulletDamage);
            Destroy(gameObject);
        }

        Collider[] colliders= Physics.OverlapBox(center, halfExtents, orientation, mask);
        if(colliders.Length > 0){
            Destroy(gameObject);
        }


        _rigidbody.MovePosition(_rigidbody.position + delta);
        _bulletLifeTimeCounter += Time.fixedDeltaTime;

        if (_bulletLifeTime <= _bulletLifeTimeCounter) Destroy(gameObject);
    }

    private void BoxCastInfo(out Vector3 center, out Vector3 halfExtents, out Quaternion orientation, out LayerMask mask)
    {
        center = _boxCollider.bounds.center;
        halfExtents = _boxCollider.size * 0.5f;
        orientation = _rigidbody.rotation;
        mask = LayerMask.GetMask("Enemy", "Obstacle");
    }

    public void SetBulletDirection(Vector3 direction){
        bulletDiretion = direction;
    }

}
