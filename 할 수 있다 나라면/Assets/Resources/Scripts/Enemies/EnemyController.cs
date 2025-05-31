using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    public string enemyName = "기본 적";
    public int maxHP = 100;
    public int currentHP;

    [Header("공격 패턴")]
    public TimingPattern attackPattern;

    [Header("연결 필수")]
    public TurnManager turnManager;
    public TimingBar timingBar; // 타이밍 바 컴포넌트 연결

    private void Start()
    {
        currentHP = maxHP;

        if (turnManager == null)
        {
            Debug.LogError("TurnManager가 없습니다.");
            return;
        }

        if (timingBar == null)
        {
            Debug.LogError("TimingBar가 연결되지 않았습니다.");
        }
    }

    public void PerformAttack(Action onComplete)
    {
        Debug.Log($"{enemyName}의 공격 시작!");

        if (timingBar != null)
        {
            timingBar.StartBar(attackPattern, (bool playerDodged) =>
            {
                if (!playerDodged)
                {
                    Debug.Log("플레이어가 회피에 실패했습니다. 데미지 입습니다.");
                    turnManager.player.TakeDamage(10); // 예시 데미지
                }
                else
                {
                    Debug.Log("플레이어가 회피에 성공했습니다!");
                }

                onComplete?.Invoke(); // 턴 넘기기
            });
        }
        else
        {
            Debug.LogWarning("타이밍 바 없음 - 공격 생략됨");
            onComplete?.Invoke();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"{enemyName}이(가) {amount} 데미지를 입음. 남은 체력: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{enemyName}이(가) 쓰러졌습니다!");
        gameObject.SetActive(false);
        // 이후 게임 상태나 전투 종료 처리 추가 가능
    }

    public void ResetEnemy()
    {
        currentHP = maxHP;
        gameObject.SetActive(true);
    }
}