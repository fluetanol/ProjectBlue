using System;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 기본 데이터를 받아온 뒤, 여기에서 실시간 수정을 합니다.
public class PlayerDataManager : DataManager, IBasicData
{
    public struct WeaponCondition
    {
        public ushort WeaponCode;
        public ushort WeaponLevel;
    }
    
    [SerializeField] private PlayerStats _playerStats;
    public PlayerStats PlayerStats
    {
        get => _playerStats;
        private set { }
    }

    public Transform ShootPoint;
    public Weapon weapon;
    public WeaponStats.WeaponInfo wponInfo;

    [Header("Basic Player Stats")]
    [SerializeField] private float _currentHP;
    public float currentHP
    {
        get
        {
            return _currentHP;
        }
        set
        {
            _currentHP = value;
            // if (_currentHP < 0)
            // {
            //     _currentHP = 0;
            // }
        }
    }

    public float maxHP
    {
        get
        {
            return PlayerStats.Health;
        }
    } //플레이어 최대 체력

    public float currentDEF
    {
        get;
        set;
    }

    [SerializeField] private float _currentAtk;
    public float currentAtk
    {
        get
        {
            return _currentAtk;
        }
        set
        {
            _currentAtk = value;
            // if (_currentAtk < 0)
            // {
            //     _currentAtk = 0;
            // }
        }
    }     //플레이어 자체 공격력

    [SerializeField] private float _currentMoveSpeed;
    public float currentMoveSpeed
    {
        get
        {
            return _currentMoveSpeed;
        }
        set
        {
            _currentMoveSpeed = value;
            // if (_currentMoveSpeed < 0)
            // {
            //     _currentMoveSpeed = 0;
            // }
        }
    }
    public float currentAttackSpeed
    {
        get;
        set;
    }

    [Header("Shield Data")]
    [SerializeField] private int _currentShieldHeadIdx = 0;
    public int currentShieldHeadIdx
    {
        get
        {
            return _currentShieldHeadIdx;
        }
        set
        {
            _currentShieldHeadIdx = value;
        }
    }
    [SerializeField] private int _currentShieldCount = 0;
    public int currentShieldCount
    {
        get
        {
            return _currentShieldCount;
        }
        set
        {
            _currentShieldCount = value;
        }
    }

    [SerializeField] private List<ShieldCondition> _currentShields  = new List<ShieldCondition>(new ShieldCondition[3]);
    public List<ShieldCondition> currentShields
    {
        get
        {
            return _currentShields;
        }
        private set { }
    }

    private float damageRate = 1;


    //데미지 계산 공식
    // (공격력 * 공력력 증가율 + 무기 공격력) - 적 방어력 (일반적으론 보스 빼곤 0임)

    //받는 피해 계산 공식
    // max((적 공격력 - 내 방어력), 최소피해량) 


    void Awake()
    {
        PlayerStats = _playerStats;
        WeaponStats = _weaponStats;
        InitializeCurrentStats();
        CreateWeapon(ref wponInfo);
    }

    private void InitializeCurrentStats()
    {
        int weaponCode = PlayerStats.WeaponID;
        wponInfo = _weaponStats[weaponCode];
        wponInfo.AddMask(LayerMask.GetMask("Enemy"));

        currentHP = PlayerStats.Health;
        currentMoveSpeed = PlayerStats.MoveSpeed;
        currentAttackSpeed = wponInfo.AttackSpeed;
    }

    private void CreateWeapon(ref WeaponStats.WeaponInfo wponinfo)
    {

        GameObject createWeapon = Instantiate(wponinfo.WeaponPrefab, ShootPoint.position, Quaternion.identity, this.transform);
        weapon = createWeapon.GetComponent<Weapon>();
        weapon.SetWeaponStats(wponinfo);
    }


    public float[] GetPlayerDamage()
    {
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


    public void SetDamageRate(float rate, bool isAdd)
    {
        if (isAdd) damageRate += rate;
        else damageRate -= rate;
    }

    public void SetMoveSpeedRate(float rate, bool isAdd)
    {
        if (isAdd) currentMoveSpeed += currentMoveSpeed * rate;
        else currentMoveSpeed -= currentMoveSpeed * rate;
    }

}
