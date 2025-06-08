using UnityEngine;

[System.Serializable]
public class DodgeSpotData
{ 
     //Range해두면 inspector에서 슬라이더로 값 조정 가능
     [Range(0f, 1f)] public float start;//시작 위치(0~1)
     [Range(0f, 1f)] public float end;//끝 위치(0~1)
    
}
