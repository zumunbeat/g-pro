using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum BattleState { PlayerTurn, EnemyTurn, Busy }
    public BattleState state;

    public TimingBar timingBar;
    public List<TimingPattern> enemyAttackPatterns;
    private int currentEnemyIndex = 0;
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
                StartCoroutine(PlayerAttackSequence());
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
        StartCoroutine(EnemyAttackSequence());
    }

    IEnumerator EnemyAttackSequence()
    {
        state = BattleState.Busy;

        Vector3 origin = enemy.transform.position;
        Vector3 target = player.transform.position + new Vector3(1.5f, 0);

        yield return MoveTo(enemy.transform, target);

        enemy.PlayAnimation("Attack");
        yield return new WaitForSeconds(0.5f); // 공격 애니메이션 시간

        bool isFinished = false;

        if (enemyAttackPatterns.Count == 0)
        {
            Debug.LogWarning("적 패턴 없음!");
            isFinished = true;
        }
        else
        {
            TimingPattern pattern = enemyAttackPatterns[currentEnemyIndex];

            //TODO 회피를 실패해도 성공처리되거나 타이밍을 실패하여 눌러도 판정구간이 사라지지않음
            //스페이스바를 연타할 수 있음 <- 원래는 한 판정구간당 한번만 누를 수 있어야한다.
            timingBar.StartBar(pattern, (bool playerDodged) =>
            {
                if (!playerDodged)
                {
                    Debug.Log("회피 실패! 플레이어가 데미지 입음");

                    player.TakeDamage(10); // 데미지 수치 조정 가능
                    
                }
                else
                {
                    Debug.Log("회피 성공! 데미지 없음");
                    
                }

                currentEnemyIndex = (currentEnemyIndex + 1) % enemyAttackPatterns.Count;
                isFinished = true;
            }); 
        }

        yield return new WaitUntil(() => isFinished);

        yield return MoveTo(enemy.transform, origin);

        enemy.PlayAnimation("Idle");

        StartPlayerTurn();
    }

    IEnumerator PlayerAttackSequence()
    {
        state = BattleState.Busy;

        Vector3 origin = player.transform.position;
        Vector3 target = enemy.transform.position + new Vector3(-1.5f, 0);

        yield return MoveTo(player.transform, target);

        player.PlayAnimation("Attack");
        yield return new WaitForSeconds(0.5f); // 공격 애니메이션 길이만큼

        enemy.TakeDamage(10);
        
        yield return MoveTo(player.transform, origin);
        
        player.PlayAnimation("Idle"); // 수동으로 Idle 전환
        
        StartEnemyTurn();
    }

    IEnumerator MoveTo(Transform actor, Vector3 targetPos, float duration = 0.3f)
    {
        Vector3 start = actor.position;
        float time = 0;
        while (time < duration)
        {
            actor.position = Vector3.Lerp(start, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        actor.position = targetPos;
    }
}