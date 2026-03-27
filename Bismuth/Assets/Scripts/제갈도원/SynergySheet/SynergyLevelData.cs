using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SynergyLevelData
{
    [Header("효과값 목록")]
    [SerializeField] private List<string> _effectValues = new();
    
    
    [Header("활성 유닛 수")]
    [SerializeField] private int _activeCount;


    public int ActiveCount => _activeCount;
    public List<string> EffectValues => _effectValues; // 정령이 2s 를 들고 있어서 string 으로 

    public void SetData(int activeCount, List<string> effectValues)
    {
        _activeCount = activeCount;
        _effectValues = effectValues;
    }


}