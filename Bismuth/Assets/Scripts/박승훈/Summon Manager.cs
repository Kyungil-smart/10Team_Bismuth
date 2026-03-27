using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SummonManager : MonoBehaviour
{
    [SerializeField] private SynergyManager _synergyManager;
    [SerializeField] private PlayerDataManager _playerDataManager;
    
    [SerializeField] private List<UnitSO> units = new List<UnitSO>(4);
    
    [SerializeField] private SummonChanceSO _summonSO;
    
    [SerializeField] private bool SummonLog = true;

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Summon, SummonLog);
    }

    // 유닛 소환 메서드
    public void SummonUnit()
    {
        UnitData data = RandomUnit(GetRandomTier());
        
        GameObject _unitPrefab = LoadUnitPrefab(data);
        
        UnitStat stat = SetUnitStat(_unitPrefab, data);
        
        if (_unitPrefab == null)
            return;
        
        Instantiate(_unitPrefab, RandomLoc(), Quaternion.identity);
        
        _synergyManager.OnUnitCreated?.Invoke(stat);
    }

    // 무작위 좌표 생성 (테스트 용)
    private Vector2 RandomLoc()
    {
        Vector2 randomLoc = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
        return randomLoc;
    }
    
    public int GetRandomTier()
    {
        SummonChanceData data = GetByEnhancementLevel(_playerDataManager.Level);

        if (data == null)
        {
            DebugTool.Warnning($"강화 단계 {_playerDataManager.Level} 데이터가 없습니다.", DebugType.Data, this);
            return -1;
        }

        float randomValue = Random.Range(0f, 100f);
        PrintChanceData(data);
        if (randomValue < data.Tier1)
            return 0;

        if (randomValue < (data.Tier1 + data.Tier2))
            return 1;

        if (randomValue < (data.Tier1 + data.Tier2 + data.Tier3))
            return 2;

        return 3;
    }

    // 플레이어 레벨 상승 시 상승 데이터가 존재하는 지 판단
    public SummonChanceData GetByEnhancementLevel(int enhancementLevel)
    {
        if (_summonSO.Rows[enhancementLevel - 1].EnhancementLevel == enhancementLevel - 1)
            return _summonSO.Rows[enhancementLevel - 1];

        return null;
    }
    private UnitData RandomUnit(int tier)
    {
        int _randomUnit = Random.Range(0, units[tier].Units.Count - 1);
        
        DebugTool.Log($"랜덤 티어 : {tier + 1} | " +
                      $"랜덤 번호 : [{_randomUnit + 1}] | " +
                      $"유닛 이름 : {units[tier].Units[_randomUnit].UnitName}", DebugType.Summon, this);
    
        return units[tier].Units[_randomUnit];
    }

    // 시너지 객채 생성 매서드
    private GameObject LoadUnitPrefab(UnitData data)
    {
        return (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Resources/Units/{data.Id}.prefab", typeof(GameObject));
    }

    // 새로 생성될 유닛의 세팅 매핑
    private UnitStat SetUnitStat(GameObject unit, UnitData unitData)
    {
        UnitStat stat = unit?.GetComponent<UnitStat>();

        if (stat == null)
        {
            DebugTool.Warnning($"유닛 스탯 컴포넌트 = {stat != null}", DebugType.Unit, this);
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

    private void Init()
    {
        _playerDataManager = GetComponent<PlayerDataManager>();
        _synergyManager = GetComponent<SynergyManager>();
    }
    
    // 유닛 스텟 디버깅 메서드
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
    
    /// <summary>
    /// 현재 강화 단계 데이터 로그 출력
    /// </summary>
    public void PrintChanceData(SummonChanceData data)
    {
        DebugTool.Log(
            $"[확률 데이터] 강화 단계: {data.EnhancementLevel} / " +
            $"1등급: {data.Tier1}, 2등급: {data.Tier2}, 3등급: {data.Tier3}, 4등급: {data.Tier4}, 합계: {data.SumPersent}",
            DebugType.Data,
            this
        );
    }
}
