using System.Collections;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour, IDamageable, IHealable
{
    private PlayerDataManager _playerDataManager;

    public float DmgTick;
    private bool isWaitDmgTick = false; //데미지 틱을 기다리는 중인지 확인

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
            _playerDataManager.currentHP -= damage;
            if (_playerDataManager.currentShieldCount > 0)
            {
                TakeShieldDamage(damage);
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
            shieldCondition.ShieldValue = 0;
            
            for (int i = 0; i < shieldCondition.ShieldEffectObject.Count; i++)
            {
                shieldCondition.ShieldEffectObject[i].SetActive(false);
            }
            
            shieldCondition.ShieldEffectObject.ForEach(effect => effect.SetActive(false));
            _playerDataManager.currentShieldHeadIdx = ShieldCondition.MaxShieldIdx(_playerDataManager.currentShields);
            _playerDataManager.currentShieldCount--;
        }

        _playerDataManager.currentShields[idx] = shieldCondition;
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
