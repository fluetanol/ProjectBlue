using UnityEngine;

[RequireComponent(typeof(Rigidbody))]   
public class EnemyMovement : MonoBehaviour
{

    public enum EenemyMoveType
    {
        linear,
        linearInterpolation
    }

    private Rigidbody _rigidbody;

    [SerializeField] private Rigidbody _target;
    private Vector3 _targetPosition;

    [SerializeField] private EenemyMoveType _enemyMoveType = EenemyMoveType.linearInterpolation;

    [Header("Setting Enemy Move Speed")]
    [SerializeField, ConditionalField(nameof(_enemyMoveType), (int)EenemyMoveType.linear), Range(1, 10)]
    private float _linearMoveSpeed = 5f;

    [SerializeField, ConditionalField(nameof(_enemyMoveType), (int)EenemyMoveType.linearInterpolation), Range(0.1f, 2f)] 
    private float _linearInterpolationMoveSpeed = 1f; 

    

    void Awake(){
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate(){
        Vector3 nextPosition = enemyMove();
        _rigidbody.MovePosition(nextPosition);
    }

    private Vector3 enemyMove(){
        _targetPosition = _target != null ? _target.position : PlayerMovement.PlayerPosition;

        if(_enemyMoveType == EenemyMoveType.linearInterpolation)
            return Vector3.Lerp(_rigidbody.position, _targetPosition, Time.fixedDeltaTime * _linearInterpolationMoveSpeed);

        else{
            Vector3 direction = _targetPosition - _rigidbody.position;
            direction.Normalize();
            return _rigidbody.position + direction * Time.fixedDeltaTime * _linearMoveSpeed;
        }
    }

}
