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
    private RectTransform barRect;       // Ÿ�̹� �� ��ü RectTransform
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
        spotSuccesses = new List<bool>(new bool[currentSpots.Count]);
        barRect = spotParent;
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
        // 1. ����ģ Spot �ڵ� ����
        for (int i = 0; i < currentSpots.Count; i++)
        {
            if (spotSuccesses[i]) continue;

            float extendedEnd = currentSpots[i].end;

            if (progress > extendedEnd)
            {
                Debug.Log($"���� {i} ���� (����)");
                RemoveSpot(i);
                spotSuccesses[i] = true;
            }
        }
        // 2. Space ������ ����
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
                if (spotSuccesses[i]) continue;

                float extendedStart = currentSpots[i].start;
                float extendedEnd = currentSpots[i].end;

                extendedStart = Mathf.Max(0f, extendedStart);
                extendedEnd = Mathf.Min(1f, extendedEnd);

                if (progress >= extendedStart && progress <= extendedEnd)
                {
                    Debug.Log($"���� {i} ȸ�� ����!");
                    spotSuccesses[i] = true;
                    anySuccessThisPress = true;
                    RemoveSpot(i);
                    break;
                }
            }

            // 3. ���� �� �� ���� ����� Spot ����
            if (!anySuccessThisPress)
            {
                int closestIndex = -1;
                float closestDistance = float.MaxValue;

                for (int i = 0; i < currentSpots.Count; i++)
                {
                    if (spotSuccesses[i]) continue;

                    float dist = Mathf.Abs(progress - currentSpots[i].start);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestIndex = i;
                    }
                }

                if (closestIndex != -1)
                {
                    Debug.Log($"��Ȯ�� ȸ�� ����, ���� ����� spot {closestIndex} ����");
                    RemoveSpot(closestIndex);
                    spotSuccesses[closestIndex] = true;
                }

                Debug.Log("ȸ�� ���� (Ÿ�̹� �̽�)");
            }
        }

        // ���� ó��
        if (progress >= 1f)
        {
            isRunning = false;
            bool allSuccess = spotSuccesses.TrueForAll(success => success);
            onCompleteCallback?.Invoke(allSuccess);
        }
    }
    void RemoveSpot(int index)
    {
        if (index < spotUIObjects.Count && spotUIObjects[index] != null)
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
        float barWidth = barRect.rect.width;
        spotUIObjects.Clear();

        foreach (var spot in spots)
        {
            RectTransform spotUI = Instantiate(dodgeSpotPrefab, spotParent);
            float width = barWidth * (spot.end - spot.start);
            float posX = barWidth * spot.start;

            spotUI.anchoredPosition = new Vector2(posX, 0);
            spotUI.sizeDelta = new Vector2(width - 0.1f, spotUI.sizeDelta.y);

            //���� �� ǥ���� spot list�� �߰�
            spotUIObjects.Add(spotUI.gameObject);
        }
    }
}