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
    TestAction testAction;
    
    [SerializeField] private List<UnitSO> units = new List<UnitSO>(4);
    
    [SerializeField] private bool SummonLog = false;

    private GameObject _unitPrefab;

    private void Awake()
        => Init();

    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Summon, SummonLog);
    }

    private void OnEnable()
    {
        testAction.Enable();

        testAction.Test.Random.started += OnRandom;
    }

    private void OnDisable()
    {
        testAction.Test.Random.started -= OnRandom;

        testAction.Disable();
    }

    private void OnRandom(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SummonUnit();
        }
    }

    public void SummonUnit()
    {
        _unitPrefab = SetUnitStat(RandomUnit(RandomTier()));
        if (_unitPrefab == null)
            return;

        GameObject.Instantiate(_unitPrefab, RandomLoc(), Quaternion.identity);
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
        DebugTool.Log($"랜덤 티어 : {tier + 1}" +
                      $"랜덤 번호 : [{_randomUnit + 1}] | " +
                      $"유닛 이름 : {units[tier].Units[_randomUnit].UnitName}", DebugType.Summon, this);

        return units[tier].Units[_randomUnit];
    }

    private GameObject SetUnitStat(UnitData unitData)
    {
        GameObject unit = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Test/{unitData.UnitName}.prefab", typeof(GameObject));
        if (unit == null)
        {
            DebugTool.Warnning($"[{unitData.UnitName}] 해당 유닛을 찾을 수 없습니다!", DebugType.Summon, this);
            return null;
        }

        UnitStat stat = unit.GetComponent<UnitStat>();

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
        stat.Synerge1 = unitData.Synergy1;
        stat.Synerge2 = unitData.Synergy2;
        stat.Synerge3 = unitData.Synergy3;
        
        PrintStat(stat);
        
        return unit;
    }

    private void PrintStat(UnitStat stat)
    {
        DebugTool.Log($"ID : {stat.Id}\n" +
                      $"Tier : {stat.Tier}\n" +
                      $"Name : {stat.Name}\n" +
                      $"AttackPower : {stat.AttackPower}\n" +
                      $"AttackSpeed : {stat.AttackSpeed}\n" +
                      $"CritChance : {stat.CritChance}\n" +
                      $"Range : {stat.Range}\n" +
                      $"AttackArea : {stat.AttackArea}\n" +
                      $"AttackType : {stat.attackTypes}\n" +
                      $"Synerge1 : {stat.Synerge1}\n" +
                      $"Synerge2 : {stat.Synerge2}\n" +
                      $"Synerge3 : {stat.Synerge3}", DebugType.Summon, this);
    }

    private void Init()
    {
        testAction = new TestAction();
    }
}
