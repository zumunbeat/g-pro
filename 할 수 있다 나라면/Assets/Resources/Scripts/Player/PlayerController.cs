using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string playerName = "플레이어";
    public int maxHP = 100;
    public int currentHP;

    private Animator animator;

    private Action onAnimationComplete; // 공격 or 스킬 완료 시 호출할 델리게이트
    private string nextAnimation;       // 다음 애니메이션 이름

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        currentHP = maxHP;
    }

    public void PlayAnimation(string name)
    {
        Debug.Log($"{name} 재생");
        animator.SetTrigger(name);
    }

    // 공격 애니메이션 끝에서 Animation Event로 호출됨
    public void OnAttackAnimationComplete()
    {
        Debug.Log("공격 애니메이션 끝");
        animator.SetTrigger(nextAnimation);
        onAnimationComplete?.Invoke();
        onAnimationComplete = null; // 중복 호출 방지
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
        // 죽음 처리 로직
    }

    public void PerformAttack(Action onComplete)
    {
        Debug.Log($"{playerName}이(가) 일반 공격!");
        onAnimationComplete = onComplete;
        nextAnimation = AnimState.Idle;
        animator.SetTrigger(AnimState.Attack);
    }

    public void PerformSkill(Action onComplete)
    {
        Debug.Log($"{playerName}이(가) 스킬 사용!");
        onAnimationComplete = onComplete;
        nextAnimation = AnimState.Idle; // 나중에 SkillIdle 같은 걸로 바꿔도 됨
        animator.SetTrigger(AnimState.Attack); // 임시로 공격 애니메이션 재사용
    }

    public void UseItem(Action onComplete)
    {
        Debug.Log($"{playerName}이(가) 아이템 사용!");
        // 아이템 애니메이션 있으면 여기서 처리
        onComplete?.Invoke();
    }
}