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
        //UI호출
        battleUI.EnableActionButtons(true);
    }

    public void OnPlayerActionChosen(string action)
    {
        Debug.Log($"행동선택: {action}");
        battleUI.EnableActionButtons(false);
        state = BattleState.Busy;

        switch (action)
        {
            //행동후에 EnemyTurn으로 넘어가도록 Lamda함수 넘기기
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
            Debug.Log("도망 성공!");
            // 전투 종료 처리
        }
        else
        {
            Debug.Log("도망 실패...");
            StartEnemyTurn();
        }
    }

    void StartEnemyTurn()
    {
        state = BattleState.EnemyTurn;
        enemy.PerformAttack(() => StartPlayerTurn());
    }
}