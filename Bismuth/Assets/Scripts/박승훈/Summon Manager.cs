using System;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class SummonManager : MonoBehaviour
{
    [SerializeField] private SynergyManager _synergyManager;
    
    [SerializeField] private List<UnitSO> units = new List<UnitSO>(4);
    
    [SerializeField] private bool SummonLog = true;


    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Summon, SummonLog);
    }

    public void SummonUnit()
    {
        UnitData data = RandomUnit(RandomTier());
        
        GameObject _unitPrefab = LoadUnitPrefab(data);
        
        UnitStat stat = SetUnitStat(_unitPrefab, data);
        
        if (_unitPrefab == null)
            return;
        
        Instantiate(_unitPrefab, RandomLoc(), Quaternion.identity);
        
        _synergyManager.OnUnitCreated?.Invoke(stat);
    }

    private Vector2 RandomLoc()
    {
        Vector2 randomLoc = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
        return randomLoc;
    }

    private int RandomTier()
        => Random.Range(0, 0/*UnitDataController.MaxTier - 1*/);

    private UnitData RandomUnit(int tier)
    {
        int _randomUnit = Random.Range(0, units[tier].Units.Count - 1);
        
        DebugTool.Log($"랜덤 티어 : {tier + 1} | " +
                      $"랜덤 번호 : [{_randomUnit + 1}] | " +
                      $"유닛 이름 : {units[tier].Units[_randomUnit].UnitName}", DebugType.Summon, this);

        return units[tier].Units[_randomUnit];
    }

    private GameObject LoadUnitPrefab(UnitData data)
    {
        return (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Prefabs/박승훈/Units/{data.UnitName}.prefab", typeof(GameObject));
    }

    private UnitStat SetUnitStat(GameObject unit, UnitData unitData)
    {
        UnitStat stat = unit?.GetComponent<UnitStat>();

        if (stat == null)
        {
            DebugTool.Warnning($"{stat.Name} 을 찾을 수 없습니다.", DebugType.Summon, this);
            return null;
        }

        stat.Id = unitData.Id;
        stat.Tier = unitData.Tier;
        stat.Name = unitData.UnitName;
        stat.AttackPower = unitData.AttackPower;
        stat.AttackSpeed = unitData.AttackSpeed;
        stat.CritChance = unitData.CriticalChance;
        stat.Range = unitData.Range;
        stat.AttackArea = unitData.AttackArea;
        stat.attackTypes = unitData.AttackType;
        stat.AttackTargetCount = unitData.AttackTargetCount;
        stat.SynergIDs = unitData.SynergyIDs;
        
        PrintStat(stat);

        return stat;
    }

    // 시너지 객채 생성 매서드
    // 시너지 id와 시너지 enum과 비교

    private void PrintStat(UnitStat stat)
    {
        if (stat == null)
        {
            DebugTool.Warnning("스탯이 없습니다.", DebugType.Summon, this);
            return;
        }

        DebugTool.Log($"ID : {stat.Id}\n" +
                      $"Tier : {stat.Tier}\n" +
                      $"Name : {stat.Name}\n" +
                      $"AttackPower : {stat.AttackPower}\n" +
                      $"AttackSpeed : {stat.AttackSpeed}\n" +
                      $"CritChance : {stat.CritChance}\n" +
                      $"Range : {stat.Range}\n" +
                      $"AttackArea : {stat.AttackArea}\n" +
                      $"AttackType : {stat.attackTypes}\n" +
                      $"AttackTargetCount : {stat.AttackTargetCount}\n" +
                      $"Synerge1 : {stat.SynergIDs[0]}\n" +
                      $"Synerge2 : {stat.SynergIDs[1]}\n" +
                      $"Synerge3 : {stat.SynergIDs[2]}\n"
            , DebugType.Summon, this);
    }
}
