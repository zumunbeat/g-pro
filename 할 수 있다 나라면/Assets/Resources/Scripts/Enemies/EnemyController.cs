using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    public string enemyName = "�⺻ ��";
    public int maxHP = 100;
    public int currentHP;

    [Header("���� ����")]
    public TimingPattern attackPattern;

    [Header("���� �ʼ�")]
    public TurnManager turnManager;
    public TimingBar timingBar; // Ÿ�̹� �� ������Ʈ ����

    private void Start()
    {
        currentHP = maxHP;

        if (turnManager == null)
        {
            Debug.LogError("TurnManager�� �����ϴ�.");
            return;
        }

        if (timingBar == null)
        {
            Debug.LogError("TimingBar�� ������� �ʾҽ��ϴ�.");
        }
    }

    public void PerformAttack(Action onComplete)
    {
        Debug.Log($"{enemyName}�� ���� ����!");

        if (timingBar != null)
        {
            timingBar.StartBar(attackPattern, (bool playerDodged) =>
            {
                if (!playerDodged)
                {
                    Debug.Log("�÷��̾ ȸ�ǿ� �����߽��ϴ�. ������ �Խ��ϴ�.");
                    turnManager.player.TakeDamage(10); // ���� ������
                }
                else
                {
                    Debug.Log("�÷��̾ ȸ�ǿ� �����߽��ϴ�!");
                }

                onComplete?.Invoke(); // �� �ѱ��
            });
        }
        else
        {
            Debug.LogWarning("Ÿ�̹� �� ���� - ���� ������");
            onComplete?.Invoke();
        }
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

    void Die()
    {
        Debug.Log($"{enemyName}��(��) ���������ϴ�!");
        gameObject.SetActive(false);
        // ���� ���� ���³� ���� ���� ó�� �߰� ����
    }

    public void ResetEnemy()
    {
        currentHP = maxHP;
        gameObject.SetActive(true);
    }
}