using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Monobehaviour 때세요.
public class TurnManager : MonoBehaviour, IManager
{
    public enum BattleState { PlayerTurn, EnemyTurn, Busy }
    public BattleState state;

    public TimingBar timingBar;
    public List<TimingPattern> enemyAttackPatterns;
    public PlayerController player;
    public EnemyController enemy;
    public BattleUI battleUI;
    private int currentEnemyIndex = 0;

    public void Init()
    {
        // Managers 시스템에서 자동 호출됨
    }


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
        EnemyAttackWithTiming(enemyAttackPatterns[currentEnemyIndex]);
    }

    void EnemyAttackWithTiming(TimingPattern pattern)
    {
        state = BattleState.Busy;

        StartCoroutine(EnemyAttackSequence(pattern));
    }

    IEnumerator EnemyAttackSequence(TimingPattern pattern)
    {
        Vector3 origin = enemy.transform.position;
        Vector3 target = player.transform.position + new Vector3(1.5f, 0);

        yield return MoveTo(enemy.transform, target);

        enemy.PlayAnimation("Attack");
        yield return new WaitForSeconds(0.5f);

        // 핵심: TimingBar 실행, 콜백 안에서 턴 종료
        timingBar.StartBar(pattern, (bool dodged) =>
        {
            if (!dodged)
            {
                player.TakeDamage(10);
                Debug.Log("회피 실패 → 데미지 처리");
            }
            else
            {
                Debug.Log("회피 성공");
            }

            StartCoroutine(EndEnemyTurn(origin));
        });
    }
    IEnumerator EndEnemyTurn(Vector3 origin)
    {
        yield return MoveTo(enemy.transform, origin);
        enemy.PlayAnimation("Idle");

        currentEnemyIndex = (currentEnemyIndex + 1) % enemyAttackPatterns.Count;
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