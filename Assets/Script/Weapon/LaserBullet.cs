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
    }

    public override void SetBulletStats(WeaponStats.WeaponInfo weaponInfo)
    {
        base.SetBulletStats(weaponInfo);
        _bulletDistance = weaponInfo.GetAttackDistance();
        _bulletLifeTime = weaponInfo.GetAttackLifeTime();

    }


    protected override void BulletCollide()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, bulletDiretion, _bulletDistance, LayerMask.GetMask("Enemy"));

        hits.ToList().ForEach(hit => {
            hit.collider.GetComponent<EnemyMovement>().TakeDamage(_bulletDamage);
        });

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
