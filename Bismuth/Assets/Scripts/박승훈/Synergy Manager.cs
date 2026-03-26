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
    public UnityEvent<UnitStat> OnUnitCreated;
    public UnityEvent<UnitStat> OnUnitRemoved;

    private Dictionary<int, List<int>> synergiesDict = new();

    [SerializeField] private int _unitCount = 0;

    [SerializeField] private bool _log = true;

    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Synergy, _log);
    }

    public void ChangedSynergy(UnitStat stat)
    {
        _unitCount++;
        for (int i = 0; i < stat.SynergIDs.Length; i++)
        {
            if (stat.SynergIDs[i] == 0)
                break;
            
            if (!synergiesDict.ContainsKey(stat.SynergIDs[i]))
            {
                List<int> list = new();
                list.Add(stat.Id);
                synergiesDict.Add(stat.SynergIDs[i], list);
            }
            
            else
            {
                if (synergiesDict[stat.SynergIDs[i]].Contains(stat.Id))
                    continue;
                else 
                    synergiesDict[stat.SynergIDs[i]].Add(stat.Id);
            }
        }

        PrintSynergy();
    }

    public void RemoveSynergy(UnitStat stat)
    {
        _unitCount--;
        for (int i = 0; i < stat.SynergIDs.Length; i++)
        {
            if (stat.SynergIDs[i] == 0)
                break;
            
            if (synergiesDict.ContainsKey(stat.SynergIDs[i]))
                synergiesDict[stat.SynergIDs[i]].Remove(stat.Id);
            
            if(synergiesDict[stat.SynergIDs[i]].Count == 0)
                synergiesDict.Remove(stat.SynergIDs[i]);
        }
        
        PrintSynergy();
    }

    private void PrintSynergy()
    {
        string log = $"[보유 유닛 수] : {_unitCount}\n[보유 시너지]\n";

        foreach (var value in synergiesDict)
        {
            switch (value.Key)
            {
                case (int)SynergyType.Warrior :
                    log += $"시너지 : 전사 | 활성 개수 : {value.Value.Count}\n";
                    break;
                case (int)SynergyType.Magician :
                    log += $"시너지 : 마법사 | 활성 개수 : {value.Value.Count}\n";
                    break;
                case (int)SynergyType.Archer :
                    log += $"시너지 : 궁수 | 활성 개수 : {value.Value.Count}\n";
                    break;
                case (int)SynergyType.Gunner :
                    log += $"시너지 : 거너 | 활성 개수 : {value.Value.Count}\n";
                    break;
                case (int)SynergyType.Fighter :
                    log += $"시너지 : 격투가 | 활성 개수 : {value.Value.Count}\n";
                    break;
                case (int)SynergyType.Human :
                    log += $"시너지 : 인간 | 활성 개수 : {value.Value.Count}\n";
                    break;
                case (int)SynergyType.Elf :
                    log += $"시너지 : 엘프 | 활성 개수 : {value.Value.Count}\n";
                    break;
                case (int)SynergyType.Orc :
                    log += $"시너지 : 오크 | 활성 개수 : {value.Value.Count}\n";
                    break;
                case (int)SynergyType.Furry :
                    log += $"시너지 : 수인 | 활성 개수 : {value.Value.Count}\n";
                    break;
                case (int)SynergyType.Spirit :
                    log += $"시너지 : 정령 | 활성 개수 : {value.Value.Count}\n";
                    break;
            }
        }
        
        DebugTool.Log(log, DebugType.Synergy, this);
    }
    
    public enum SynergyType
    {
        None = 0,
        Warrior = 50001,
        Magician = 50002,
        Archer = 50003,
        Gunner = 50004,
        Fighter = 50005,
        Human = 50006,
        Elf = 50007,
        Orc = 50008,
        Furry = 50009,
        Spirit = 50010
    }
}