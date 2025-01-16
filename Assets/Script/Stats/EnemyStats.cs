using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats", order = 1)]
public class EnemyStats : ScriptableObject
{
    [System.Serializable]
    public struct EnemyInfo{
        [Header("Setting Enemy Basic Stats")]
        public short  EnemyCode;
        public short  EnemyHealth;
        public short  EnemyDamage;

        [Header("Setting Enemy Move Stats")]
        public EnemyMovement.EenemyMoveType EnemyMoveType;
        [SerializeField, ConditionalField(nameof(EnemyMoveType), (int)EnemyMovement.EenemyMoveType.linear), Range(1, 10)]
        public float _linearMoveSpeed;
        
        [SerializeField, ConditionalField(nameof(EnemyMoveType), (int)EnemyMovement.EenemyMoveType.linearInterpolation), Range(0.1f, 2f)]
        public float _linearInterpolationMoveSpeed;

        [Header("Setting Enemy Attack Stats")]
        public EnemyMovement.EenemyAttackType EnemyAttackType;


    }

    [SerializeField] private List<EnemyInfo> _enemyInfoList = new List<EnemyInfo>();

    public EnemyInfo this[int idx]{ // class indexer
        get{
            return _enemyInfoList[idx];
        }
    }
}
