using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

//여기 데이터들은 변하는 값들이 아닙니다. 변하는 데이터들은 PlayerCurrentDataManager에서 확인.
[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class PlayerStats : ScriptableObject
{
    public int   Health;         //전체 체력
    public int   Defense;        //방어력
    public float MoveSpeed;      //이동 속도
    public int   WeaponID;         //갖고 있는 무기의 id (주무기)

    //보조무기도 추가 될 예정입니다


}

/*
    추후에 ScriptableObject는 파이어베이스나 외부 데이터베이스에서 데이터를 받아오는 
    캐시 정도로 쓸 예정
*/

/*
    주무기 : 마우스로 직접 클릭하여 공격하는 무기입니다.
    보조 무기 : 옆에서 알아서 공격해주는 무기, 설치기 또는 버프 장치입니다.




*/