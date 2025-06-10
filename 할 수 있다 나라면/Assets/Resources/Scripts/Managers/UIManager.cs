using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void Init() { }

    public void ShowMessage(string msg)
    {
        Debug.Log($"[UI] {msg}");
    }
}
