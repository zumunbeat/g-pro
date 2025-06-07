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
        StartCoroutine(EnemyAttackSequence());
    }

    IEnumerator EnemyAttackSequence()
    {
        state = BattleState.Busy;

        Vector3 origin = enemy.transform.position;
        Vector3 target = player.transform.position + new Vector3(1.5f, 0);

        yield return MoveTo(enemy.transform, target);

        enemy.PlayAnimation("Attack");
        yield return new WaitForSeconds(0.5f); // ���� �ִϸ��̼� �ð�

        bool isFinished = false;

        if (enemyAttackPatterns.Count == 0)
        {
            Debug.LogWarning("�� ���� ����!");
            isFinished = true;
        }
        else
        {
            TimingPattern pattern = enemyAttackPatterns[currentEnemyIndex];

            //TODO ȸ�Ǹ� �����ص� ����ó���ǰų� Ÿ�̹��� �����Ͽ� ������ ���������� �����������
            //�����̽��ٸ� ��Ÿ�� �� ���� <- ������ �� ���������� �ѹ��� ���� �� �־���Ѵ�.
            timingBar.StartBar(pattern, (bool playerDodged) =>
            {
                if (!playerDodged)
                {
                    Debug.Log("ȸ�� ����! �÷��̾ ������ ����");

                    player.TakeDamage(10); // ������ ��ġ ���� ����
                    
                }
                else
                {
                    Debug.Log("ȸ�� ����! ������ ����");
                    
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
        yield return new WaitForSeconds(0.5f); // ���� �ִϸ��̼� ���̸�ŭ

        enemy.TakeDamage(10);
        
        yield return MoveTo(player.transform, origin);
        
        player.PlayAnimation("Idle"); // �������� Idle ��ȯ
        
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