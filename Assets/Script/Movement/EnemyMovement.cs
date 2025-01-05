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
        if(_enemyMoveType == EenemyMoveType.linearInterpolation)
            return Vector3.Lerp(_rigidbody.position, PlayerMovement.PlayerPosition, Time.fixedDeltaTime * _linearInterpolationMoveSpeed);

        else{
            Vector3 direction = PlayerMovement.PlayerPosition - _rigidbody.position;
            direction.Normalize();
            return _rigidbody.position + direction * Time.fixedDeltaTime * _linearMoveSpeed;
        }
    }

}
