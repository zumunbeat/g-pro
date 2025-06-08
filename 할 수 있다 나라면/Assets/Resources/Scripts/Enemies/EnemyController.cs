using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public string enemyName = "�⺻ ��";
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
        Debug.Log($"{name} ���");
        animator.SetTrigger(name);
    }
    public void PerformAttack(Action onComplete)
    {
        Debug.Log($"{enemyName}�� ���� �ִϸ��̼� ����");

        onAttackComplete = onComplete;
        nextAnimation = AnimState.Idle;

        animator.SetTrigger(AnimState.Attack);
    }

    // �ִϸ��̼� �������� �̺�Ʈ�� ȣ���
    public void OnAttackAnimationComplete()
    {
        Debug.Log($"{enemyName}�� ���� �ִϸ��̼� ���� �� Idle");
        animator.SetTrigger(nextAnimation);
        onAttackComplete?.Invoke();
        onAttackComplete = null;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"{enemyName}��(��) {amount} �������� ����. ���� ü��: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{enemyName}��(��) ���������ϴ�!");
        gameObject.SetActive(false);
    }

    public void ResetEnemy()
    {
        currentHP = maxHP;
        gameObject.SetActive(true);
    }
}