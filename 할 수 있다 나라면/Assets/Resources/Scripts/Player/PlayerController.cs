using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string playerName = "�÷��̾�";
    public int maxHP = 100;
    public int currentHP;

    private void Start()
    {
        currentHP = maxHP;
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
        // ���� ���� ó�� �Ǵ� ���� ���� ���� �߰� ����
    }

    public void PerformAttack(Action onComplete)
    {
        Debug.Log($"{playerName}��(��) �Ϲ� ����!");
        onComplete?.Invoke();
    }

    public void PerformSkill(Action onComplete)
    {
        Debug.Log($"{playerName}��(��) ��ų ���!");
        onComplete?.Invoke();
    }

    public void UseItem(Action onComplete)
    {
        Debug.Log($"{playerName}��(��) ������ ���!");
        onComplete?.Invoke();
    }
}
