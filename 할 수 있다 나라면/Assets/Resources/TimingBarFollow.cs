using UnityEngine;

public class TimingBarFollow : MonoBehaviour
{
    public Transform target;//�÷��̾�
    public Vector3 offset = new Vector3(0, 2.0f, 0); //�󸶳� ���� ��ġ��ų ���ΰ�
    public Canvas canvas;//TimingBar�� ���� canvas

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

        //���� ��ǥ -> ȭ�� ��ǥ
        Vector3 screenPos = mainCam.WorldToScreenPoint(target.position + offset);
        //ȭ�� ��ǥ -> ���� UI ��ǥ
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        //��ȯ ��ǥ ������ ����
        Vector2 uiPos;
        //Overlay ��� -> ī�޶� ��� ��, Camera��� -> ���� ī�޶� ��������
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCam, out uiPos))
        {
            //���� UI�� ��ǥ ����
           rectTransform.anchoredPosition = uiPos;
        }
    }
}
