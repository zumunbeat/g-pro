using System.Collections.Generic;
using UnityEngine;

//Default = 2f
[System.Serializable]
public class TimingPattern 
{
    public float duration = 2f; //�� �ð�
    public List<DodgeSpotData> dodgeSpots = new List<DodgeSpotData>(); // ȸ�� ������
    
}
