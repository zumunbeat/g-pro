using UnityEngine;

public class EnemyTurnControlloer : MonoBehaviour
{
    public TimingBar timingBar;

    public void StartEnemyTurn()
    {
        Debug.Log("적 턴 시작");
        timingBar.gameObject.SetActive(true);
        timingBar.StartBar(2.0f); // 2초 동안 진행
    }
}
