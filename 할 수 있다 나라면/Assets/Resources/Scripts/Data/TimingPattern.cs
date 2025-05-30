using System.Collections.Generic;
using UnityEngine;

//Default = 2f
[System.Serializable]
public class TimingPattern 
{
    public float duration = 2f; //총 시간
    public List<DodgeSpotData> dodgeSpots = new List<DodgeSpotData>(); // 회피 구간들
    
}
