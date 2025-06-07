using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public string enemyName = "기본 적";
    public int maxHP = 100;
    public int currentHP;

    private Animator animator;

    private Action onAttackComplete;
    private string nextAnimation;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHP = maxHP;
    }
    public void PlayAnimation(string name)
    {
        Debug.Log($"{name} 재생");
        animator.SetTrigger(name);
    }
    public void PerformAttack(Action onComplete)
    {
        Debug.Log($"{enemyName}의 공격 애니메이션 시작");

        onAttackComplete = onComplete;
        nextAnimation = AnimState.Idle;

        animator.SetTrigger(AnimState.Attack);
    }

    // 애니메이션 마지막에 이벤트로 호출됨
    public void OnAttackAnimationComplete()
    {
        Debug.Log($"{enemyName}의 공격 애니메이션 종료 → Idle");
        animator.SetTrigger(nextAnimation);
        onAttackComplete?.Invoke();
        onAttackComplete = null;
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

    private void Die()
    {
        Debug.Log($"{enemyName}이(가) 쓰러졌습니다!");
        gameObject.SetActive(false);
    }

    public void ResetEnemy()
    {
        currentHP = maxHP;
        gameObject.SetActive(true);
    }
}