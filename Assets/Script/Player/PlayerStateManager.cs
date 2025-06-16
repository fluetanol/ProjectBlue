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


    void IDamageable.TakeDamage(float damage)
    {
        if (!isWaitDmgTick)
        {
            _playerDataManager.currentHP -= damage;
            StartCoroutine(DmgTickTimer());
        }

        if (_playerDataManager.currentHP <= 0)
        {
            // Game Over
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
