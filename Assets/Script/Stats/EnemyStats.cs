using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats", order = 1)]
public class EnemyStats : ScriptableObject
{
    [System.Serializable]
    struct EnemyInfo{
        public short  EnemyCode;
        public short  EnemyHealth;
        public short  EnemyDamage;
        public EnemyMovement.EenemyMoveType EnemyMoveType;
        public float  EnemyMoveSpeed;
        public string EnemyName;
    }

    [SerializeField] private List<EnemyInfo> _enemyInfoList = new List<EnemyInfo>();

}
