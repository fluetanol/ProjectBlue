using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ShotBullet : Bullet
{
    [SerializeField] private bool _isPenetration;
    [SerializeField] private float _radius;
    [SerializeField] private float _angle;
    

    override protected void OnEnable()
    {
        base.OnEnable();
        BulletCollide();
    }


    protected override void BulletCollide(){
        Collider[] enimies = Physics.OverlapSphere(transform.position, _radius, LayerMask.GetMask("Enemy"));
        
        for(int i=0; i<enimies.Length; i++){
            bool inRange = false;
            if(enimies[i].TryGetComponent(out IDamageable Idmg)){
                inRange  = CheckRange(enimies[i].transform, Idmg);
            }

            //안 밀리는 녀석들도 있을 테니 나눠서 처리하려고 이렇게 해놓음
            if(inRange && enimies[i].TryGetComponent(out IForceable Iforce)){
                bulletDirection = enimies[i].transform.position - moveData.PlayerPosition;
                Iforce.Knockback(bulletDirection, _bulletDamage / 2);
            }
        }
    }

    private bool CheckRange(Transform target, IDamageable Idmg)
    {
        Vector3 lookDirection = moveData.LookDirection;
        Vector3 targetPosition = target.position;
        Vector3 originPosition = moveData.PlayerPosition;
        Vector3 targetDirection = (targetPosition - originPosition).normalized;


        Debug.DrawLine(targetPosition, originPosition, Color.cyan, 5f);
        Debug.DrawRay(transform.position, lookDirection, Color.blue, 5f);

        //dot = |a||b|cos(theta)
        //cos(theta) = dot / |a||b|
        //theta = acos(dot / |a||b|)
        float dot = Vector3.Dot(targetDirection, lookDirection);
        float theta = Mathf.Acos(dot);
        float degree = Mathf.Rad2Deg * theta;

       // print("degree: " + degree + " _angle: " + _angle);

       //angle보다 작은 범위내에 있다는 건 angle안에 들어있다는 뜻입니다.
        if(degree <= _angle / 2){
           // print("dmg!");
            Idmg.TakeDamage((int)_bulletDamage);
             PlayerMovement.Debugcolor = Color.green;
             return true;
        }else{
            return false;
        }
    }



    protected override IEnumerator BulletLifeTime(){
        yield return new WaitForSeconds(_bulletLifeTime);
        BulletPoolManager.Instance.Return(_bulletCode, this);
    }

}
