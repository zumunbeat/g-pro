using UnityEngine;

public class TimingBarFollow : MonoBehaviour
{
    public Transform target;//플레이어
    public Vector3 offset = new Vector3(0, 2.0f, 0); //얼마나 위에 위치시킬 것인가
    public Canvas canvas;//TimingBar가 속한 canvas

    private RectTransform rectTransform;
    private Camera mainCam;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (target == null || canvas == null) return;

        //월드 좌표 -> 화면 좌표
        Vector3 screenPos = mainCam.WorldToScreenPoint(target.position + offset);
        //화면 좌표 -> 로컬 UI 좌표
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        //변환 좌표 저장할 변수
        Vector2 uiPos;
        //Overlay 모드 -> 카메라 없어도 됨, Camera모드 -> 메인 카메라 가져오기
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCam, out uiPos))
        {
            //최종 UI의 좌표 설정
           rectTransform.anchoredPosition = uiPos;
        }
    }
}
