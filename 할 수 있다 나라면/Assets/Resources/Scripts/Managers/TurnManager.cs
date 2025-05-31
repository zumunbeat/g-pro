using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum BattleState { PlayerTurn, EnemyTurn, Busy }
    public BattleState state;

    public PlayerController player;
    public EnemyController enemy;
    public BattleUI battleUI;

    void Start()
    {
        battleUI.SetupButtons(this);
        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        state = BattleState.PlayerTurn;
        //UIȣ��
        battleUI.EnableActionButtons(true);
    }

    public void OnPlayerActionChosen(string action)
    {
        Debug.Log($"�ൿ����: {action}");
        battleUI.EnableActionButtons(false);
        state = BattleState.Busy;

        switch (action)
        {
            //�ൿ�Ŀ� EnemyTurn���� �Ѿ���� Lamda�Լ� �ѱ��
            case "Attack":
                player.PerformAttack(() => StartEnemyTurn());
                break;
            case "Skill":
                player.PerformSkill(() => StartEnemyTurn());
                break;
            case "Item":
                player.UseItem(() => StartEnemyTurn());
                break;
            case "Run":
                TryRunAway();
                break;
        }
    }

    void TryRunAway()
    {
        bool success = Random.value > 0.5f;
        if (success)
        {
            Debug.Log("���� ����!");
            // ���� ���� ó��
        }
        else
        {
            Debug.Log("���� ����...");
            StartEnemyTurn();
        }
    }

    void StartEnemyTurn()
    {
        state = BattleState.EnemyTurn;
        enemy.PerformAttack(() => StartPlayerTurn());
    }
}