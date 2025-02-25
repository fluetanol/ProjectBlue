using System.Collections;
using System.Linq;
using UnityEngine;

public class LaserBullet : Bullet
{
    private float _time;
    private float _bulletDistance;
    public LineRenderer _lineRenderer = null;


    void Update()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position + transform.forward * _bulletDistance);
        bulletDiretion = transform.forward;
    }

    public override void SetBulletStats(WeaponStats.WeaponInfo weaponInfo)
    {
        base.SetBulletStats(weaponInfo);
        bulletDiretion = transform.forward;
        _bulletDistance = weaponInfo.GetAttackDistance();
        _bulletLifeTime = weaponInfo.GetAttackLifeTime();

    }


    protected override void BulletCollide()
    {

        RaycastHit[] hits = Physics.RaycastAll(transform.position, bulletDiretion, _bulletDistance, LayerMask.GetMask("Enemy"));
       
        if(hits.Length == 0) {
            print("no hits");
            return;
        }
        print("collide!");
        foreach (RaycastHit hit in hits){
            if(hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<IDamageable>() != null) 
                 hit.collider.gameObject.GetComponent<IDamageable>().TakeDamage(_bulletDamage);
            else{
                print("danger! null");
            }
        }


    }

    protected override IEnumerator BulletLifeTime()
    {
        while(true){
            _time += Time.deltaTime;
            if(_time >= _bulletLifeTime){
                _lineRenderer.startColor = Color.white;
                _lineRenderer.endColor = Color.white;
                //gameObject.SetActive(false);
            }
            yield return null;
        }     
    }
    
    protected override void GetBulletComponent()
    {
       if(_lineRenderer == null && TryGetComponent(out LineRenderer lineRenderer)){
            _lineRenderer = lineRenderer;
        }
    }

    public void TickAttack()
    {   
        if(gameObject.activeSelf == false) {
            gameObject.SetActive(true);
        }
        _time = 0;
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        BulletCollide();
    }
}
