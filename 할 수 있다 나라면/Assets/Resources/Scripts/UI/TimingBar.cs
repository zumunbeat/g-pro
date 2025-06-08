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
    private List<bool> spotSuccesses = new List<bool>();
    private List<bool> spotProcessed = new List<bool>();


    public void StartBar(TimingPattern pattern, Action<bool> onComplete)
    {
        Debug.Log("Start Bar 생성");

        duration = pattern.duration;
        currentTime = 0f;
        isRunning = true;
        onCompleteCallback = onComplete;
        spotProcessed = new List<bool>(new bool[currentSpots.Count]);  // 모두 false로 초기화

        currentSpots = pattern.dodgeSpots;

        // spotSuccesses 초기화 (길이 맞춰서 재활용 또는 새로 생성)
        if (spotSuccesses == null || spotSuccesses.Count != currentSpots.Count)
        {
            spotSuccesses = new List<bool>(new bool[currentSpots.Count]);
        }
        else
        {
            for (int i = 0; i < spotSuccesses.Count; i++)
                spotSuccesses[i] = false;
        }

        // spotProcessed 초기화도 동일하게 적용 가능
        if (spotProcessed == null || spotProcessed.Count != currentSpots.Count)
        {
            spotProcessed = new List<bool>(new bool[currentSpots.Count]);
        }
        else
        {
            for (int i = 0; i < spotProcessed.Count; i++)
                spotProcessed[i] = false;
        }

        // UI 오브젝트 풀링
        for (int i = 0; i < currentSpots.Count; i++)
        {
            GameObject spotObj;

            // 재사용 가능한 오브젝트가 있다면 사용
            if (i < spotUIObjects.Count && spotUIObjects[i] != null)
            {
                spotObj = spotUIObjects[i];
                spotObj.SetActive(true);
            }
            else
            {
                // 없으면 새로 생성
                spotObj = Instantiate(dodgeSpotPrefab, spotParent).gameObject;

                if (i < spotUIObjects.Count)
                    spotUIObjects[i] = spotObj;
                else
                    spotUIObjects.Add(spotObj);
            }

            // 위치 및 사이즈 조정
            var rect = spotObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(currentSpots[i].start, 0f);
            rect.anchorMax = new Vector2(currentSpots[i].end, 1f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        // 남은 spot 오브젝트 비활성화
        for (int i = currentSpots.Count; i < spotUIObjects.Count; i++)
        {
            if (spotUIObjects[i] != null)
                spotUIObjects[i].SetActive(false);
        }
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
            if (spotSuccesses[i] || spotProcessed[i]) continue;

            if (progress > currentSpots[i].end)
            {
                spotProcessed[i] = true;  // 이 구간은 실패 처리 완료
                spotSuccesses[i] = false; // 실패 상태 명확히 표시
                Debug.Log($"구간 {i} 실패 (지남)");
                RemoveSpot(i);
                break; // 또는 return;
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
                    spotProcessed[i] = true;  // 성공도 처리 완료 표시
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

                if (closestIndex != -1 && !spotProcessed[closestIndex])
                {
                    spotProcessed[closestIndex] = true;
                    spotSuccesses[closestIndex] = false;
                    Debug.Log($"정확한 회피 실패, 가장 가까운 spot {closestIndex} 제거");
                    RemoveSpot(closestIndex);
                }
            }
        }
    }
    public void RemoveSpot(int index)
    {
        if (index < 0 || index >= spotUIObjects.Count) return;

        if (spotUIObjects[index] != null)
            spotUIObjects[index].SetActive(false);
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