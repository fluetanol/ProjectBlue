using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class PlayerStats : ScriptableObject
{
    public struct WeaponCondition{
        public short WeaponCode;
        public short WeaponLevel;
    }

    public float Health;
    public float MoveSpeed;
    public List<int> HoldWeaponList;
}

public struct WeaponInfo{

}