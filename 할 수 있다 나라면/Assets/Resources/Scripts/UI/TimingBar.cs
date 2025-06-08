using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TimingBar : MonoBehaviour
{
    [Header("UI")]
    public Slider progressBar;
    public RectTransform dodgeSpotPrefab; // ������ (���� �̹��� �Ǵ� ���� ���)
    public RectTransform spotParent;      // Spot���� ��� �θ� ������Ʈ
    public RectTransform handleRect;    // �����̴� �ڵ� RectTransform

    private float duration;
    private float currentTime;
    private bool isRunning = false;

    //��ü spot Object�� ǥ��
    private List<DodgeSpotData> currentSpots = new List<DodgeSpotData>();
    //�ð������� ǥ���� spot������Ʈ���� ����
    private List<GameObject> spotUIObjects = new List<GameObject>();
    private Action<bool> onCompleteCallback;
    private List<bool> spotSuccesses;

    public void StartBar(TimingPattern pattern, Action<bool> onComplete)
    {
        Debug.Log("Start Bar ����");
        duration = pattern.duration;
        spotSuccesses = new List<bool>(new bool[currentSpots.Count]);//��� false�� �ʱ�ȭ
        currentTime = 0f;
        isRunning = true;
        currentSpots = pattern.dodgeSpots;
        onCompleteCallback = onComplete;
        spotSuccesses = new List<bool>(new bool[currentSpots.Count]);
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
            return;
        }
        // 1. ����ģ Spot �ڵ� ����
        for (int i = 0; i < currentSpots.Count; i++)
        {
            if (spotSuccesses[i]) continue;

            if (progress > currentSpots[i].end)
            {
                spotSuccesses[i] = true; // ���� true�� ����
                Debug.Log($"���� {i} ���� (����)");
                RemoveSpot(i);
            }
        }
        // 2. Space ������ ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (spotSuccesses.TrueForAll(success => success))
            {
                Debug.LogWarning("�̹� ��� ȸ�� ���� ó�� �Ϸ�");
                return;
            }

            bool anySuccess = false;

            for (int i = 0; i < currentSpots.Count; i++)
            {
                if (spotSuccesses[i]) continue;

                float start = Mathf.Max(0f, currentSpots[i].start);
                float end = Mathf.Min(1f, currentSpots[i].end);

                if (progress >= start && progress <= end)
                {
                    Debug.Log($"���� {i} ȸ�� ����!");
                    spotSuccesses[i] = true;
                    RemoveSpot(i);
                    anySuccess = true;
                    break;
                }
            }

            // ���� �� ���� ����� Spot ����
            if (!anySuccess)
            {
                int closestIndex = -1;
                float closestDist = float.MaxValue;

                for (int i = 0; i < currentSpots.Count; i++)
                {
                    if (spotSuccesses[i]) continue;
                    float dist = Mathf.Abs(progress - currentSpots[i].start);

                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestIndex = i;
                    }
                }

                if (closestIndex != -1)
                {
                    Debug.Log($"��Ȯ�� ȸ�� ����, ���� ����� spot {closestIndex} ����");
                    spotSuccesses[closestIndex] = false;
                    RemoveSpot(closestIndex);
                }
            }
        }
    }
    void RemoveSpot(int index)
    {
        if (index >= 0 && index < spotUIObjects.Count && spotUIObjects[index] != null)
        {
            Destroy(spotUIObjects[index]);
            spotUIObjects[index] = null;
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
        float barWidth = spotParent.rect.width;
        spotUIObjects.Clear();

        foreach (var spot in spots)
        {
            RectTransform spotUI = Instantiate(dodgeSpotPrefab, spotParent);
            float width = barWidth * (spot.end - spot.start);
            float posX = barWidth * spot.start;

            spotUI.anchoredPosition = new Vector2(posX, 0);
            spotUI.sizeDelta = new Vector2(width - 0.1f, spotUI.sizeDelta.y);
            spotUIObjects.Add(spotUI.gameObject);
        }
    }
}