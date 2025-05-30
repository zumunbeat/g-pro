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

    public TimingBar timingBar; // 타이밍바 참조
    public List<TimingPattern> enemyAttackPatterns;
    private int currentEnemyIndex = 0;

    #region 턴 진행
    private void Start()
    {
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        currentTurn = Turn.Player;
        Debug.Log("플레이어 턴 시작");

        // TODO: 플레이어가 행동할 수 있도록 UI 활성화
        EndPlayerTurn(); // 테스트용 자동 턴 종료
    }

    public void EndPlayerTurn()
    {
        Debug.Log("플레이어 턴 종료");
        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        currentTurn = Turn.Enemy;
        Debug.Log("적 턴 시작");

        EnemyAttack();
    }

    private void EndEnemyTurn(bool isDodged)
    {
        Debug.Log(isDodged ? "회피 성공" : "회피 실패");

        // 다음 패턴 순서
        currentEnemyIndex = (currentEnemyIndex + 1) % enemyAttackPatterns.Count;
        StartPlayerTurn();
    }
    #endregion

    public void EnemyAttack()
    {
        if (enemyAttackPatterns.Count == 0)
        {
            Debug.LogWarning("적 패턴 없음!");
            return;
        }

        TimingPattern pattern = enemyAttackPatterns[currentEnemyIndex];

        // 타이밍 바 시작 + 콜백으로 결과 전달 받기
        timingBar.StartBar(pattern, EndEnemyTurn);
    }
}