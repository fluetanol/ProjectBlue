using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletDamage;
    [SerializeField] private float _bulletLifeTime;
    
    public Vector3 bulletDiretion{
        get;
        set;
    }

    float _bulletLifeTimeCounter = 0f;
    private Rigidbody _rigidbody;

    void Start(){
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate(){
        _rigidbody.MovePosition(_rigidbody.position + _bulletSpeed * Time.fixedDeltaTime * bulletDiretion);
        _bulletLifeTimeCounter += Time.fixedDeltaTime;
       if(_bulletLifeTime <= _bulletLifeTimeCounter){
              Destroy(gameObject);
       }
    }

    public void SetBulletDirection(Vector3 direction){
        bulletDiretion = direction;
    }
}
