using UnityEngine;
using UnityEngine.UI;

public class TimingBar : MonoBehaviour
{
    public Slider progressBar;
    public RectTransform dodgeSpotUI;

    private float duration;
    private float currentTime;
    private bool isRunning = false;

    private float spotStart = 0.4f;
    private float spotEnd = 0.6f;

    public void StartBar(float _duration)
    {
        duration = _duration;
        currentTime = 0f;
        isRunning = true;
        SetDodgeSpot(spotStart, spotEnd);
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
            Debug.Log("회피 실패 (시간 초과)");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (progress >= spotStart && progress <= spotEnd)
            {
                Debug.Log("회피 성공!");
                isRunning = false;
            }
            else
            {
                Debug.Log("회피 실패 (타이밍 미스)");
                isRunning = false;
            }
        }
    }

    void SetDodgeSpot(float start, float end)
    {
        // 진행 바 위에 회피 구간 표시
        float barWidth = ((RectTransform)progressBar.fillRect).rect.width;
        float posX = barWidth * start;
        float width = barWidth * (end - start);

        dodgeSpotUI.anchoredPosition = new Vector2(posX, 0);
        dodgeSpotUI.sizeDelta = new Vector2(width, dodgeSpotUI.sizeDelta.y);
    }
}
