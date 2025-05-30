using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    public string enemyName = "테스트 적";
    public TimingPattern attackPattern;

    [Header("무조건 연결해주세요")]
    public TurnManager turnManager;

    private void Start()
    {
        if (turnManager == null)
        {
            Debug.LogError("TurnManager를 찾을 수 없습니다.");
            return;
        }

        // 공격 패턴 등록
        turnManager.enemyAttackPatterns.Clear();
        turnManager.enemyAttackPatterns.Add(attackPattern);
    }
}