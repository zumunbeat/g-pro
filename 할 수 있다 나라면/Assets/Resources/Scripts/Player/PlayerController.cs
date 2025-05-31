using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string playerName = "플레이어";
    public int maxHP = 100;
    public int currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"{playerName}이(가) {amount} 데미지를 입었습니다. 남은 체력: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{playerName}이(가) 쓰러졌습니다!");
        // 게임 오버 처리 또는 전투 종료 로직 추가 가능
    }

    public void PerformAttack(Action onComplete)
    {
        Debug.Log($"{playerName}이(가) 일반 공격!");
        onComplete?.Invoke();
    }

    public void PerformSkill(Action onComplete)
    {
        Debug.Log($"{playerName}이(가) 스킬 사용!");
        onComplete?.Invoke();
    }

    public void UseItem(Action onComplete)
    {
        Debug.Log($"{playerName}이(가) 아이템 사용!");
        onComplete?.Invoke();
    }
}
