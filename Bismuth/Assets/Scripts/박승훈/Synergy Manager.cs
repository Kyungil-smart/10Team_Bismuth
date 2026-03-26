using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/*
 * TODO :
 * - 유닛의 소환/합성 시 전체 시너지 갱신
 *      - 소환 시 신규 시너지 추가 / 합성 시 기존 시너지 제거, 신규 시너지 추가
 * - 딕셔너리 <시너지 ID값, 리스트<시너지 객채>> 로 구성
 *      - 유닛이 생성될 때 해당 시너지의 객체 생성 하여 리스트에 추가 리스트
 *      - 시너지 ID에 해당하는 리스트의 개수로 시너지 스택 파악
 * - 소환된 유닛을 우선순위 기반으로 합성 UI에 갱신 (실시간 X)
 * - 활성화된 시너지에 따라 해당 시너지를 보유한 유닛에 효과 부여
 */
public class SynergyManager : MonoBehaviour
{
    public UnityEvent<int[]> OnUnitCreated;
    public UnityEvent<int[]> OnUnitRemoved;

    private Dictionary<int, int> synergiesDict = new();

    [SerializeField] private bool _log = true;

    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Synergy, _log);
    }

    public void ChangedSynergy(int[] synergyId)
    {
        for (int i = 0; i < synergyId.Length; i++)
        {
            if (synergyId[i] == 0)
                break;
            
            if (!synergiesDict.ContainsKey(synergyId[i]))
                synergiesDict.Add(synergyId[i], 1);
            
            else if (synergiesDict.ContainsKey(synergyId[i]))
                synergiesDict[synergyId[i]]++;
        }

        string log = "[시너지 추가]\n[보유 시너지]\n";
        foreach (var value in synergiesDict)
            log += $"시너지 : {value.Key} | 활성 개수 : {value.Value}\n";
        
        DebugTool.Log(log, DebugType.Synergy, this);
    }

    public void RemoveSynergy(int[] synergyId)
    {
        for (int i = 0; i < synergyId.Length; i++)
        {
            if (synergyId[i] == 0)
                break;
            
            if (synergiesDict.ContainsKey(synergyId[i]))
            {
                synergiesDict[synergyId[i]]--;
                
                DebugTool.Log($"{synergiesDict[synergyId[i]]}", DebugType.Synergy, this);

                if (synergiesDict[synergyId[i]] == 0)
                    synergiesDict.Remove(synergyId[i]);
                
            }
            else
                DebugTool.Log($"{synergyId[i]} 시너지가 없습니다.", DebugType.Synergy, this);
        }
        
        string log = "[시너지 삭제]\n[보유 시너지]\n";
        
        foreach (var value in synergiesDict)
            log += $"시너지 : {value.Key} | 활성 개수 : {value.Value}\n";
        
        DebugTool.Log(log, DebugType.Synergy, this);
    }
}