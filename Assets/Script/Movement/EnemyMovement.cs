using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]   
public class EnemyMovement : MonoBehaviour, IDamageable
{
    public Transform testobj;
    public int health = 3;
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

    public void TakeDamage(int damage){
        StartCoroutine(TEST());
        health -= damage;
        if(health <= 0){
            Destroy(gameObject);
        }
    }

    IEnumerator TEST(){
        testobj.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        testobj.gameObject.SetActive(false);

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

}
