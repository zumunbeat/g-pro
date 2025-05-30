using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    public string enemyName = "�׽�Ʈ ��";
    public TimingPattern attackPattern;

    [Header("������ �������ּ���")]
    public TurnManager turnManager;

    private void Start()
    {
        if (turnManager == null)
        {
            Debug.LogError("TurnManager�� ã�� �� �����ϴ�.");
            return;
        }

        // ���� ���� ���
        turnManager.enemyAttackPatterns.Clear();
        turnManager.enemyAttackPatterns.Add(attackPattern);
    }
}