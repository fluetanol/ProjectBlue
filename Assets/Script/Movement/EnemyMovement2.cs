using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(Rigidbody))]   
public class EnemyMovement2 : MonoBehaviour, IDamageable, IForceable, IAttackable
{
    [Header("Enemy Basic Stats")]
    public Transform testobj;
    public float health = 3;
    public int   damage = 1;
    public float attackTick;
    public float dmgTick;

    [Header("Enemy Additional Stats")]
    public float _moveSpeed;
    public float range; //공격 범위 반지름
    public int _weaponCode; //무기 코드
    public Transform ShotPoint; //무기 발사 위치



    [Header("Enemy Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private int EnemyCode;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Rigidbody _target;


    [Header("Setting Enemy Move Stats Scriptable Object")]
    [SerializeField] private EnemyStats _enemyStats;

    private Vector3 _targetPosition, _nextPosition;
    private Weapon _weapon;
    private bool _isAttacking = false;  
    private bool _isDead = false;
    private bool _moveLock = false;

    void Awake(){
        if(_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        InitializeStats();
        if(_animator == null) _animator =  GetComponent<Animator>();
       // health =_enemyStats.SetEnemyStats(EnemyCode);
    }

    void Start(){
         WeaponStats.WeaponInfo weaponStats = PlayerDataManager.WeaponStats[_weaponCode];
         weaponStats.BasicAttackMask += LayerMask.GetMask("Player");
         GameObject createWeapon = Instantiate(weaponStats.WeaponPrefab, ShotPoint.position, Quaternion.identity, this.transform);
         _weapon = createWeapon.GetComponent<Weapon>();
         _weapon.SetWeaponStats(weaponStats);
    
    }

    void FixedUpdate(){
        if(_isDead) {
            return;
        }
        _nextPosition = enemyMove();
        Attack();

        if(_moveLock) return;
        _rigidbody.MovePosition(_nextPosition);
    }


    private void InitializeStats(){
        health = _enemyStats[EnemyCode].EnemyHealth;
        damage = _enemyStats[EnemyCode].EnemyDamage;
        dmgTick = _enemyStats[EnemyCode].EnemyDmgTick;
    }


    public void TakeDamage(float damage){
        //StartCoroutine(TEST());
        health -= damage;
        if(health <= 0){
            _isDead = true;
            _animator.SetBool("isDead", _isDead);
        }
    }

    private Vector3 enemyMove(){
        //기본 타겟은 항상 플레이어입니다.
        _targetPosition = _target != null ? _target.position : PlayerMovement.PlayerPosition;

        transform.LookAt(_targetPosition);
        if(_enemyStats[0].EnemyMoveType == EenemyMoveType.linearInterpolation)
            return Vector3.Lerp(_rigidbody.position, _targetPosition, 
            Time.fixedDeltaTime * _enemyStats[0]._linearInterpolationMoveSpeed);

        else{
            Vector3 direction = _targetPosition - _rigidbody.position;
            direction.Normalize();
            return _rigidbody.position + direction * Time.fixedDeltaTime * _enemyStats[0]._linearMoveSpeed;
        }
    }

    //근접 공격
     public void Attack(){
        if(_isAttacking){
            return;
        } 

        //공격 로직
        Vector3 substraction = PlayerMovement.PlayerPosition - _rigidbody.position;
        float distance = substraction.magnitude;

        print("distance : " + distance);
        
        if (distance <= range){
            _weapon.Attack();
            StartCoroutine(AttackTimer());
            _animator.SetBool("isAttack", true);
        }
        else{
            _moveLock = false;
        }
        
        //Debug.DrawRay(_rigidbody.position, newdirection * (Vector3.Distance(_rigidbody.position, _nextPosition)+1f), Color.red,3f);
    }


    //현재 enemy는 dynamic 모드라서 addforce로 처리하지만
    //kinematic으로 변경해야 하는 순간이 온다면 코드 바꿔야 할 예정
    public void Knockback(Vector3 direction, float force){
        print("force!" + direction + " " + force);
        
        _rigidbody.AddForce(direction * force,ForceMode.Impulse);
    }

    private IEnumerator AttackTimer(){
        _isAttacking = true;
        _moveLock = true;
        yield return new WaitForSeconds(attackTick);
        _isAttacking = false;
        _moveLock = false;
        _animator.SetBool("isAttack", _isAttacking);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
