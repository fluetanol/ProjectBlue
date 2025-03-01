using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class LaserBullet : Bullet
{
    private float _time;
    private float _bulletDistance;
    public float thickness = 0.25f;
    private RaycastHit[] _hits;
    public LineRenderer _lineRenderer = null;

    void Update()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position + transform.forward * _bulletDistance);
        bulletDiretion = transform.forward;
    }

    private void cmdray(){
        int rcount = 3;
        //job으로 스케쥴링을 사용하기 위해선 Native Array에 유니티 관련 managed 객체들을 넣어야 한다.
        NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(rcount, Allocator.TempJob);
        NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(rcount, Allocator.TempJob);
        QueryParameters queryParameters = new QueryParameters
        {
            layerMask = LayerMask.GetMask("Enemy"),
            hitTriggers = QueryTriggerInteraction.Ignore,

        };
        for(int i=0; i<rcount; i++){
            RaycastCommand cmd = new(transform.position, bulletDiretion, queryParameters, _bulletDistance);
            commands[i] = cmd;
         }

        //job을 스케쥴링하고 완료될때까지 대기한다.
        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, rcount);
        handle.Complete();

        for(int i=0; i<rcount; i++){
            if (results[i].collider != null && results[0].collider.gameObject.GetComponent<IDamageable>() != null)
                print(i + " " +results[i].collider.gameObject.name);
            else
            {
                print("danger! null");
            }
        }

        commands.Dispose();
        results.Dispose();
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
    
        //RaycastHit[] hits = Physics.RaycastAll(transform.position, bulletDiretion, _bulletDistance, LayerMask.GetMask("Enemy"));
        Vector3 size = Vector3.right * thickness + Vector3.forward * thickness;
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, size, bulletDiretion, Quaternion.identity, _bulletDistance, LayerMask.GetMask("Enemy"));
        

        if (hits.Length == 0) {
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

    protected override IEnumerator BulletLifeTime(){
        while(true){
            _time += Time.deltaTime;
            //그냥 임시방편 시각적 효과이므로 무시해도됨
            if(_time >= _bulletLifeTime/2 && _lineRenderer.startColor != Color.white){
                _lineRenderer.startColor += new Color(0, 0.05f,0.05f,-0.05f);
                _lineRenderer.endColor += new Color(0, 0.05f, 0.05f,-0.05f);
            }
            if (_time >= _bulletLifeTime){
                _lineRenderer.enabled = false;
                cmdray();

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
        if(_lineRenderer.enabled == false){
            _lineRenderer.enabled = true;
        }
        _time = 0;
        _lineRenderer.startWidth = thickness;
        _lineRenderer.endWidth = thickness;
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        BulletCollide();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 size = Vector3.right * thickness + Vector3.forward * thickness;
        Gizmos.DrawWireCube(transform.position, size);
    }


}
