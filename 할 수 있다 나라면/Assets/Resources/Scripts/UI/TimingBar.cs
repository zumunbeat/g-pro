using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TimingBar : MonoBehaviour
{
    [Header("UI")]
    public Slider progressBar;
    public RectTransform dodgeSpotPrefab; // 프리팹 (투명 이미지 또는 색상 블록)
    public RectTransform spotParent;      // Spot들을 담는 부모 오브젝트
    public RectTransform handleRect;    // 움직이는 핸들 RectTransform

    private float duration;
    private float currentTime;
    private bool isRunning = false;

    //전체 spot Object를 표현
    private List<DodgeSpotData> currentSpots = new List<DodgeSpotData>();
    //시각적으로 표현된 spot오브젝트들을 저장
    private List<GameObject> spotUIObjects = new List<GameObject>();
    private Action<bool> onCompleteCallback;
    private List<bool> spotSuccesses;

    public void StartBar(TimingPattern pattern, Action<bool> onComplete)
    {
        Debug.Log("Start Bar 생성");
        duration = pattern.duration;
        spotSuccesses = new List<bool>(new bool[currentSpots.Count]);//모두 false로 초기화
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
        // 1. 지나친 Spot 자동 제거
        for (int i = 0; i < currentSpots.Count; i++)
        {
            if (spotSuccesses[i]) continue;

            if (progress > currentSpots[i].end)
            {
                spotSuccesses[i] = true; // 먼저 true로 지정
                Debug.Log($"구간 {i} 실패 (지남)");
                RemoveSpot(i);
            }
        }
        // 2. Space 누르면 판정
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (spotSuccesses.TrueForAll(success => success))
            {
                Debug.LogWarning("이미 모든 회피 구간 처리 완료");
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
                    Debug.Log($"구간 {i} 회피 성공!");
                    spotSuccesses[i] = true;
                    RemoveSpot(i);
                    anySuccess = true;
                    break;
                }
            }

            // 실패 시 가장 가까운 Spot 제거
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
                    Debug.Log($"정확한 회피 실패, 가장 가까운 spot {closestIndex} 제거");
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