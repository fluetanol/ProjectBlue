using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 기본 데이터를 받아온 뒤, 여기에서 실시간 수정을 합니다.
public class PlayerDataManager : MonoBehaviour, IDamageable, IHealable
{
    public struct WeaponCondition{
        public ushort WeaponCode;
        public ushort WeaponLevel;
    }

    [SerializeField] private PlayerStats _playerStats;
    public static PlayerStats PlayerStats;

    [SerializeField] private WeaponStats _weaponStats;
    public static WeaponStats WeaponStats;

    public Transform ShootPoint;
    public static Weapon  weapon;


    public float                         currentHP;
    public static float                  currentDEF;
    public static float                  currentAtk;       //플레이어 자체 공격력
    public static float                  currentMoveSpeed;
    public static float                  currentAttackSpeed;
    public static float                  DmgTick;
    private bool                         isWaitDmgTick = false; //데미지 틱을 기다리는 중인지 확인
    
    
    private float damageRate =1;


    //데미지 계산 공식
    // (공격력 * 공력력 증가율 + 무기 공격력) - 적 방어력 (일반적으론 보스 빼곤 0임)

    //받는 피해 계산 공식
    // max((적 공격력 - 내 방어력), 최소피해량) 


    void Awake(){
        PlayerStats = _playerStats;
        WeaponStats = _weaponStats;
        InitializeCurrentStats();
    }

    private void InitializeCurrentStats(){
        int weaponCode = PlayerStats.WeaponID;
        WeaponStats.WeaponInfo wponinfo = _weaponStats[weaponCode];
        wponinfo.BasicAttackMask += LayerMask.GetMask("Enemy");
        currentHP        =  PlayerStats.Health;
        currentMoveSpeed = PlayerStats.MoveSpeed;
        currentAttackSpeed = wponinfo.AttackSpeed;
        DmgTick = PlayerStats.DMGTick;

        GameObject createWeapon = Instantiate(wponinfo.WeaponPrefab, ShootPoint.position, Quaternion.identity, this.transform);
        weapon = createWeapon.GetComponent<Weapon>();
        weapon.SetWeaponStats(wponinfo);
        
    }



    public float[] GetPlayerDamage(){
        /*
        float[] damages = new float[currentWeapon.Count];
        float myAttack = currentAtk * damageRate;

        for(int i=0; i<damages.Length; i++){
            damages[i] = myAttack + WeaponStats[currentWeapon[i].WeaponCode].Damage;
        }

        return damages;
        */
        return null;
    }


    public void SetDamageRate(float rate, bool isAdd){
        if(isAdd) damageRate +=  rate;
        else damageRate -= rate;
    }

    public void SetMoveSpeedRate(float rate, bool isAdd){
        if(isAdd) currentMoveSpeed += currentMoveSpeed * rate;
        else currentMoveSpeed -= currentMoveSpeed * rate;
    }

    void IDamageable.TakeDamage(float damage){
        if(!isWaitDmgTick){
            currentHP -= damage;
            StartCoroutine(DmgTickTimer());
        }

        if(currentHP <= 0){
            // Game Over
        }
    }

    void IHealable.TakeHeal(float heal){
        currentHP += heal;
        if(currentHP > PlayerStats.Health){
            currentHP = PlayerStats.Health;
        }
    }
    
    public IEnumerator DmgTickTimer(){
        isWaitDmgTick = true;
        yield return new WaitForSeconds(DmgTick);
        isWaitDmgTick = false;
    }
}
