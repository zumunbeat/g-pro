using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public Button attackButton;
    public Button skillButton;
    public Button itemButton;
    public Button runButton;

    
    public void SetupButtons(TurnManager turnManager)
    {
        attackButton.onClick.AddListener(() => turnManager.OnPlayerActionChosen("Attack"));
        skillButton.onClick.AddListener(() => turnManager.OnPlayerActionChosen("Skill"));
        itemButton.onClick.AddListener(() => turnManager.OnPlayerActionChosen("Item"));
        runButton.onClick.AddListener(() => turnManager.OnPlayerActionChosen("Run"));
    }

    public void EnableActionButtons(bool enable)
    {
        attackButton.gameObject.SetActive(enable);
        skillButton.gameObject.SetActive(enable);
        itemButton.gameObject.SetActive(enable);
        runButton.gameObject.SetActive(enable);

    }
}
