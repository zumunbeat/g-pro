using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private enum Turn
    {
        Player,
        Enemy
    }

    private Turn currentTurn;

    public TimingBar timingBar; // Ÿ�ֹ̹� ����
    public List<TimingPattern> enemyAttackPatterns;
    private int currentEnemyIndex = 0;

    #region �� ����
    private void Start()
    {
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        currentTurn = Turn.Player;
        Debug.Log("�÷��̾� �� ����");

        // TODO: �÷��̾ �ൿ�� �� �ֵ��� UI Ȱ��ȭ
        EndPlayerTurn(); // �׽�Ʈ�� �ڵ� �� ����
    }

    public void EndPlayerTurn()
    {
        Debug.Log("�÷��̾� �� ����");
        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        currentTurn = Turn.Enemy;
        Debug.Log("�� �� ����");

        EnemyAttack();
    }

    private void EndEnemyTurn(bool isDodged)
    {
        Debug.Log(isDodged ? "ȸ�� ����" : "ȸ�� ����");

        // ���� ���� ����
        currentEnemyIndex = (currentEnemyIndex + 1) % enemyAttackPatterns.Count;
        StartPlayerTurn();
    }
    #endregion

    public void EnemyAttack()
    {
        if (enemyAttackPatterns.Count == 0)
        {
            Debug.LogWarning("�� ���� ����!");
            return;
        }

        TimingPattern pattern = enemyAttackPatterns[currentEnemyIndex];

        // Ÿ�̹� �� ���� + �ݹ����� ��� ���� �ޱ�
        timingBar.StartBar(pattern, EndEnemyTurn);
    }
}