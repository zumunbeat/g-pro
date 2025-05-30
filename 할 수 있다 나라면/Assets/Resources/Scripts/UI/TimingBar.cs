using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimingBar : MonoBehaviour
{
    [Header("UI")]
    public Slider progressBar;
    public RectTransform dodgeSpotPrefab; // ������ (���� �̹��� �Ǵ� ���� ���)
    public RectTransform spotParent;      // Spot���� ��� �θ� ������Ʈ

    private float duration;
    private float currentTime;
    private bool isRunning = false;

    private List<DodgeSpotData> currentSpots = new List<DodgeSpotData>();
    private Action<bool> onCompleteCallback;
    private List<bool> spotSuccesses;

    public void StartBar(TimingPattern pattern, Action<bool> onComplete)
    {
        //if (currentSpots == null || currentSpots.Count == 0)
        //{
        //    Debug.LogWarning("ȸ�� ������ ��� �ֽ��ϴ�.");
        //    return;
        //}
        duration = pattern.duration;
        spotSuccesses = new List<bool>(new bool[currentSpots.Count]);//��� false�� �ʱ�ȭ
        currentTime = 0f;
        isRunning = true;
        currentSpots = pattern.dodgeSpots;
        onCompleteCallback = onComplete;

        ClearExistingSpots();
        ShowDodgeSpots(currentSpots);
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime += Time.deltaTime;
        float progress = currentTime / duration;
        progressBar.value = progress;

        if (progress >= 1f)
        {
            isRunning = false;
            bool allSuccess = spotSuccesses.TrueForAll(success => success);
            onCompleteCallback?.Invoke(allSuccess);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentSpots == null || currentSpots.Count == 0)
            {
                Debug.LogWarning("ȸ�� ���� ����");
                return;
            }

            bool anySuccessThisPress = false;

            for (int i = 0; i < currentSpots.Count; i++)
            {
                if (!spotSuccesses[i] && progress >= currentSpots[i].start && progress <= currentSpots[i].end)
                {
                    spotSuccesses[i] = true;
                    anySuccessThisPress = true;
                    Debug.Log($"���� {i} ȸ�� ����!");
                    break; // �� ������ ���� ó��
                }
            }

            if (!anySuccessThisPress)
            {
                Debug.Log("ȸ�� ���� (Ÿ�̹� �̽�)");
            }
        }
    }

    void ClearExistingSpots()
    {
        foreach (Transform child in spotParent)
        {
            if (child.CompareTag("DodgeSpot"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    void ShowDodgeSpots(List<DodgeSpotData> spots)
    {
        float barWidth = ((RectTransform)progressBar.fillRect).rect.width;

        foreach (var spot in spots)
        {
            RectTransform spotUI = Instantiate(dodgeSpotPrefab, spotParent);
            float width = barWidth * (spot.end - spot.start);
            float posX = barWidth * spot.start;

            spotUI.anchoredPosition = new Vector2(posX, 0);
            spotUI.sizeDelta = new Vector2(width-0.1f, spotUI.sizeDelta.y);
        }
    }
}