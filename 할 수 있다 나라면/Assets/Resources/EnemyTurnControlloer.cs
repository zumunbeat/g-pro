using UnityEngine;

public class EnemyTurnControlloer : MonoBehaviour
{
    public TimingBar timingBar;

    public void StartEnemyTurn()
    {
        Debug.Log("�� �� ����");
        timingBar.gameObject.SetActive(true);
        timingBar.StartBar(2.0f); // 2�� ���� ����
    }
}
