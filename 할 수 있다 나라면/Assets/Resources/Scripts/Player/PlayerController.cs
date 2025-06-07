using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string playerName = "�÷��̾�";
    public int maxHP = 100;
    public int currentHP;

    private Animator animator;

    private Action onAnimationComplete; // ���� or ��ų �Ϸ� �� ȣ���� ��������Ʈ
    private string nextAnimation;       // ���� �ִϸ��̼� �̸�

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
        Debug.Log($"{name} ���");
        animator.SetTrigger(name);
    }

    // ���� �ִϸ��̼� ������ Animation Event�� ȣ���
    public void OnAttackAnimationComplete()
    {
        Debug.Log("���� �ִϸ��̼� ��");
        animator.SetTrigger(nextAnimation);
        onAnimationComplete?.Invoke();
        onAnimationComplete = null; // �ߺ� ȣ�� ����
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"{playerName}��(��) {amount} �������� �Ծ����ϴ�. ���� ü��: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{playerName}��(��) ���������ϴ�!");
        // ���� ó�� ����
    }

    public void PerformAttack(Action onComplete)
    {
        Debug.Log($"{playerName}��(��) �Ϲ� ����!");
        onAnimationComplete = onComplete;
        nextAnimation = AnimState.Idle;
        animator.SetTrigger(AnimState.Attack);
    }

    public void PerformSkill(Action onComplete)
    {
        Debug.Log($"{playerName}��(��) ��ų ���!");
        onAnimationComplete = onComplete;
        nextAnimation = AnimState.Idle; // ���߿� SkillIdle ���� �ɷ� �ٲ㵵 ��
        animator.SetTrigger(AnimState.Attack); // �ӽ÷� ���� �ִϸ��̼� ����
    }

    public void UseItem(Action onComplete)
    {
        Debug.Log($"{playerName}��(��) ������ ���!");
        // ������ �ִϸ��̼� ������ ���⼭ ó��
        onComplete?.Invoke();
    }
}