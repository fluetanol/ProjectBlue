using System.Collections;
using UnityEngine;

public interface IStateData
{
    bool IsWaitDmgTick
    {
        get;
        set;
    }

    bool CanMove
    {
        get;
        set;
    }
}



public class PlayerStateManager : MonoBehaviour, IDamageable, IHealable, IStateData
{
    private PlayerDataManager _playerDataManager;

    public float DmgTick;


    private bool isWaitDmgTick = false; //데미지 틱을 기다리는 중인지 확인
    public bool IsWaitDmgTick
    { 
        get => isWaitDmgTick;
        set => isWaitDmgTick = value;
    }

    private bool _canMove = true; // 플레이어가 이동 가능한지 여부
    public bool CanMove
    {
        get => _canMove;
        set => _canMove = value;
    }
    
    

    void Awake()
    {
        _playerDataManager = GetComponent<PlayerDataManager>();
        DmgTick = _playerDataManager.PlayerStats.DMGTick;
    }


    /// <summary>
    /// 플레이어가 데미지를 받았을 때 호출되는 메서드, 쉴드가 있는 경우 가장 최근에 생성된 쉴드에 데미지를 적용합니다.
    /// </summary>
    /// <param name="damage"></param>
    void IDamageable.TakeDamage(float damage)
    {
        if (!isWaitDmgTick)
        {
            if (_playerDataManager.currentShieldCount > 0)
            {
                TakeShieldDamage(damage);
            }
            else
            {
                _playerDataManager.currentHP -= damage;
                UISystem.Instance.HealthUpdate();
            }
            StartCoroutine(DmgTickTimer());
        }

        if (_playerDataManager.currentHP <= 0)
        {
            // Game Over
        }
    }


    void TakeShieldDamage(float damage)
    {
        int idx = _playerDataManager.currentShieldHeadIdx;
        ShieldCondition shieldCondition = _playerDataManager.currentShields[idx];
        shieldCondition.ShieldValue -= damage;

        if (shieldCondition.ShieldValue <= 0)
        {
            print("broke + " + idx);
            shieldCondition.ShieldValue = 0;

            ShieldCondition.RemoveShield(_playerDataManager.currentShields, idx);
            if (_playerDataManager.currentShieldHeadIdx < 0)
            {
                // 모든 쉴드가 파괴된 경우
                _playerDataManager.currentShieldCount = 0;
            }
        }
        else
        {
            _playerDataManager.currentShields[idx] = shieldCondition;
        }
    }

    void IHealable.TakeHeal(float heal)
    {
        _playerDataManager.currentHP += heal;
        if (_playerDataManager.currentHP > _playerDataManager.PlayerStats.Health)
        {
            _playerDataManager.currentHP = _playerDataManager.PlayerStats.Health;
        }
    }


    public IEnumerator DmgTickTimer()
    {
        isWaitDmgTick = true;
        yield return new WaitForSeconds(DmgTick);
        isWaitDmgTick = false;
    }


}
