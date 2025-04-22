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
        public float  EnemyDmgTick;
        public float  EnemyAttackTick;


        [Header("Setting Enemy Move Stats")]
        public EenemyMoveType EnemyMoveType;
        [SerializeField, ConditionalField(nameof(EnemyMoveType), (int)EenemyMoveType.linear), Range(1, 10)]
        public float _linearMoveSpeed;
        
        [SerializeField, ConditionalField(nameof(EnemyMoveType), (int)EenemyMoveType.linearInterpolation), Range(0.1f, 2f)]
        public float _linearInterpolationMoveSpeed;

        [Header("Setting Enemy Attack Stats")]
        public EenemyAttackType EnemyAttackType;

        public GameObject EnemyPrefab;

    }

    [SerializeField] private List<EnemyInfo> _enemyInfoList = new List<EnemyInfo>();

    public EnemyInfo this[int idx]{ // class indexer
        get{
            return _enemyInfoList[idx];
        }
    }

    public int getSize(){
        return _enemyInfoList.Count;
    }
}
