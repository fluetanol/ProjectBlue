using System.Collections.Generic;
using UnityEngine;


//Weapon에 대한 기본 스탯 정보를 가지고 있음

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/WeaponStats", order = 1)]
public class WeaponStats : ScriptableObject
{
    [System.Serializable]
    public enum EWeaponType{
        near,
        far,
    }

    [System.Serializable]   
    public struct WeaponInfo{
        [Header("Basic Weapon Info")]
        public ushort  WeaponCode;
        public string  WeaponName;
        public ushort  Damage;
        public float   AttackSpeed;     //공격 속도입니다. 0.5초에 한번이면 0.5f

        public EWeaponType WeaponType;
        public GameObject  WeaponPrefab;
        public LayerMask   BasicAttackMask;

        [SerializeField, Header("Bullet Info"), ConditionalField(nameof(WeaponType), (int)EWeaponType.far)]
        private ushort  BulletSpeed;    
        [SerializeField, ConditionalField(nameof(WeaponType), (int)EWeaponType.far)]
        private float   BulletLifeTime;
        [ConditionalField(nameof(WeaponType), (int)EWeaponType.far)]
        public GameObject BulletPrefab;

        [SerializeField, ConditionalField(nameof(WeaponType), (int)EWeaponType.near)]
        private float AttackLifeTime;
        [SerializeField, ConditionalField(nameof(WeaponType), (int)EWeaponType.near)]
        private float AttackAngle;
        [SerializeField, ConditionalField(nameof(WeaponType), (int)EWeaponType.near)]
        private float AttackDistance;

        public float GetAttackLifeTime(){
            if(WeaponType == EWeaponType.far) return 0;
            return AttackLifeTime;
        }

        public ushort GetBulletSpeed(){
            if(WeaponType == EWeaponType.near) return 0;
            return BulletSpeed;
        }

        public float GetBulletLifeTime(){
            if(WeaponType == EWeaponType.near) return 0;
            return BulletLifeTime;
        }

        public float GetAttackAngle(){
            if(WeaponType == EWeaponType.far) return 0;
            return AttackAngle;
        }

        public float GetAttackDistance(){
            if(WeaponType == EWeaponType.far) return 0;
            return AttackDistance;
        }

    }
    
    [SerializeField] private List<WeaponInfo> WeaponList;

    public WeaponInfo this[int idx]{ // class indexer
        get{
            return WeaponList[idx];
        }
    }
}
