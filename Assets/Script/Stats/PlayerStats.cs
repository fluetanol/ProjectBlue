using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class PlayerStats : ScriptableObject
{
    public struct WeaponCondition{
        public short WeaponCode;
        public short WeaponLevel;
    }

    public int Health;
    public float MoveSpeed;
    public float AttackSpeed;
     // 시간 단위로 입력, 만약 0.5초에 한번이면 0.5f

    public List<int> HoldWeaponList;
}

public struct WeaponInfo{

}