using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

interface IForceable
{
    void Knockback(Vector3 direction, float force);
    
}

[RequireComponent(typeof(Rigidbody))]   
public class EnemyMovement : MonoBehaviour, IDamageable, IForceable
{
    public Transform testobj;
    public float health = 3;
    public int damage = 1;

    public enum EenemyMoveType
    {
        linear,
        linearInterpolation
    }

    public enum EenemyAttackType{
        near,
        far,
        middle
    }
    
    [SerializeField] private int EnemyCode;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Rigidbody _target;


    [Header("Setting Enemy Move Stats Scriptable Object")]
    [SerializeField] private EnemyStats _enemyStats;

    private Vector3 _targetPosition;

    void Awake(){
        if(_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
       // health =_enemyStats.SetEnemyStats(EnemyCode);
    }

    void FixedUpdate(){
        Vector3 nextPosition = enemyMove();
        _rigidbody.MovePosition(nextPosition);
    }

    public void TakeDamage(float damage){
        StartCoroutine(TEST());
        health -= damage;
        if(health <= 0){
            Destroy(gameObject);
        }
    }

    private Vector3 enemyMove(){
        _targetPosition = _target != null ? _target.position : PlayerMovement.PlayerPosition;

        if(_enemyStats[0].EnemyMoveType == EenemyMoveType.linearInterpolation)
            return Vector3.Lerp(_rigidbody.position, _targetPosition, 
            Time.fixedDeltaTime * _enemyStats[0]._linearInterpolationMoveSpeed);

        else{
            Vector3 direction = _targetPosition - _rigidbody.position;
            direction.Normalize();
            return _rigidbody.position + direction * Time.fixedDeltaTime * _enemyStats[0]._linearMoveSpeed;
        }
    }


    //현재 enemy는 dynamic 모드라서 addforce로 처리하지만
    //kinematic으로 변경해야 하는 순간이 온다면 코드 바꿔야 할 예정
    public void Knockback(Vector3 direction, float force){
        _rigidbody.AddForce(direction * force,ForceMode.Impulse);
    }

    IEnumerator TEST()
    {
        testobj.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        testobj.gameObject.SetActive(false);
    }

}
