using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


interface ICloneable<T>{
    T Clone();
}

//Weapon에 대한 기본 스탯 정보를 가지고 있음

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/WeaponStats", order = 1)]
public class WeaponStats : ScriptableObject
{
    public int testCount = 0;
    [System.Serializable]
    public enum EWeaponType{
        near,
        far,
    }

    [System.Serializable]   
    public struct WeaponInfo{
        public uint test{
            get{
                return test;
            }
            set{
                test += value;
            }
        }

        public void setTest(uint value){
            test = value;
        }

        
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

        public void AddMask(LayerMask mask){
            BasicAttackMask = BasicAttackMask | mask;
        }

        public void RemoveMask(LayerMask mask){
            //mask에 역을 취해서 &연산하면 되는 것, 관용문처럼 외워두자
            BasicAttackMask = BasicAttackMask & ~mask;
        }

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

    public int Count{
        get{
            return WeaponList.Count;
        }
    }


}
