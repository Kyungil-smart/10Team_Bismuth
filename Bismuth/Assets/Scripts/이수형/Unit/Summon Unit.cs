using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SummonUnit : MonoBehaviour
{
    [System.Serializable]
    public class UnitPrefabEntry
    {
        [Tooltip("비워두면 prefab.name을 사용")]
        public string unitName;
        public GameObject prefab;
    }

    [System.Serializable]
    public class SummonedTowerRecord
    {
        public int summonIndex;
        public string unitName;
        public int tier;
        public UnitData unitData;
        public TowerUnit towerUnit;
        public UnitStat unitStat;

        public PlacementSlot CurrentSlot
        {
            get
            {
                if (towerUnit == null)
                    return null;

                return towerUnit.CurrentSlot;
            }
        }
    }

    [Header("Scene References")]
    [SerializeField] private BoardSystem boardSystem;
    [SerializeField] private SynergyManager synergyManager;
    [SerializeField] private PlayerDataManager playerDataManager;

    [Header("Data")]
    [SerializeField] private List<UnitSO> units = new List<UnitSO>(4);
    [SerializeField] private SummonChanceSO summonSO;

    [Header("Prefab Table")]
    [SerializeField] private List<UnitPrefabEntry> unitPrefabTable = new List<UnitPrefabEntry>();

    [Header("Owned Towers")]
    [SerializeField] private List<SummonedTowerRecord> ownedTowers = new List<SummonedTowerRecord>();

    [SerializeField] private bool summonLog = true;

    private readonly Dictionary<string, GameObject> unitPrefabMap = new Dictionary<string, GameObject>();
    private int summonSequence = 0;

    public IReadOnlyList<SummonedTowerRecord> OwnedTowers => ownedTowers;

    private void Awake()
    {
        if (boardSystem == null)
            boardSystem = BoardSystem.Instance != null ? BoardSystem.Instance : FindAnyObjectByType<BoardSystem>();

        if (synergyManager == null)
            synergyManager = GetComponent<SynergyManager>();

        if (playerDataManager == null)
            playerDataManager = GetComponent<PlayerDataManager>();

        BuildPrefabMap();
    }

    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Summon, summonLog);
        CleanupNullOwnedTowers();
    }

    public bool TrySummonAndPlace()
    {
        int tierIndex = GetRandomTierIndex();
        if (tierIndex < 0)
            return false;

        UnitData data = GetRandomUnit(tierIndex);
        if (data == null)
            return false;

        return TrySummonAndPlace(data);
    }

    public bool TrySummonAndPlace(UnitData data)
    {
        if (data == null)
        {
            DebugTool.Warnning("소환할 UnitData가 없습니다.", DebugType.Summon, this);
            return false;
        }

        if (boardSystem == null)
        {
            DebugTool.Error("BoardSystem 참조가 없습니다.", DebugType.Board, this);
            return false;
        }

        if (!boardSystem.TryGetFirstEmptySlot(out BoardSystem.SlotData emptySlot))
        {
            DebugTool.Warnning("배치 가능한 빈 슬롯이 없습니다.", DebugType.Board, this);
            return false;
        }

        GameObject prefab = GetUnitPrefab(data);
        if (prefab == null)
        {
            DebugTool.Warnning(
                $"유닛 프리팹을 찾지 못했습니다. unitName={data.UnitName}",
                DebugType.Summon,
                this
            );
            return false;
        }

        if (!boardSystem.PlaceNewTower(prefab, emptySlot, out TowerUnit createdTower))
        {
            DebugTool.Warnning("보드에 유닛 배치에 실패했습니다.", DebugType.Board, this);
            return false;
        }

        PrepareSpawnedTower(createdTower, data);

        UnitStat stat = ApplyUnitStat(createdTower.gameObject, data);

        synergyManager?.OnUnitCreated?.Invoke(stat);

        RegisterOwnedTower(data, createdTower, stat);

        DebugTool.Log(
            $"소환 성공 - {data.UnitName} / 티어 {data.Tier} / 슬롯 {emptySlot.slot.name}",
            DebugType.Summon,
            this
        );

        return true;
    }

    public bool RemoveOwnedTower(TowerUnit tower)
    {
        if (tower == null)
            return false;

        int index = ownedTowers.FindIndex(record => record != null && record.towerUnit == tower);
        if (index < 0)
            return false;

        ownedTowers.RemoveAt(index);
        return true;
    }

    public List<UnitData> GetOwnedUnitDataSnapshot()
    {
        CleanupNullOwnedTowers();

        List<UnitData> result = new List<UnitData>();

        foreach (SummonedTowerRecord record in ownedTowers)
        {
            if (record == null || record.unitData == null)
                continue;

            result.Add(record.unitData);
        }

        return result;
    }

    private int GetRandomTierIndex()
    {
        if (playerDataManager == null)
        {
            DebugTool.Error("PlayerDataManager 참조가 없습니다.", DebugType.Summon, this);
            return -1;
        }

        SummonChanceData data = GetByEnhancementLevel(playerDataManager.Level);
        if (data == null)
        {
            DebugTool.Warnning(
                $"강화 단계 {playerDataManager.Level} 데이터가 없습니다.",
                DebugType.Data,
                this
            );
            return -1;
        }

        float randomValue = Random.Range(0f, 100f);

        if (randomValue < data.Tier1)
            return 0;

        if (randomValue < data.Tier1 + data.Tier2)
            return 1;

        if (randomValue < data.Tier1 + data.Tier2 + data.Tier3)
            return 2;

        return 3;
    }

    private SummonChanceData GetByEnhancementLevel(int enhancementLevel)
    {
        if (summonSO == null || summonSO.Rows == null)
            return null;

        for (int i = 0; i < summonSO.Rows.Count; i++)
        {
            SummonChanceData row = summonSO.Rows[i];

            if (row == null)
                continue;

            if (row.EnhancementLevel == enhancementLevel)
                return row;
        }

        return null;
    }

    private UnitData GetRandomUnit(int tierIndex)
    {
        if (tierIndex < 0 || tierIndex >= units.Count)
            return null;

        if (units[tierIndex] == null)
            return null;

        List<UnitData> tierUnits = units[tierIndex].Units;

        if (tierUnits == null || tierUnits.Count == 0)
        {
            DebugTool.Warnning($"티어 {tierIndex + 1} 유닛 데이터가 없습니다.", DebugType.Data, this);
            return null;
        }

        int randomIndex = Random.Range(0, tierUnits.Count);
        UnitData selected = tierUnits[randomIndex];

        DebugTool.Log(
            $"랜덤 티어 : {tierIndex + 1} | 랜덤 번호 : [{randomIndex + 1}] | 유닛 이름 : {selected.UnitName}",
            DebugType.Summon,
            this
        );

        return selected;
    }

    private GameObject GetUnitPrefab(UnitData data)
    {
        if (data == null)
            return null;

        if (unitPrefabMap.Count == 0)
            BuildPrefabMap();

        unitPrefabMap.TryGetValue(data.UnitName, out GameObject prefab);
        return prefab;
    }

    private void BuildPrefabMap()
    {
        unitPrefabMap.Clear();

        foreach (UnitPrefabEntry entry in unitPrefabTable)
        {
            if (entry == null || entry.prefab == null)
                continue;

            string key = string.IsNullOrWhiteSpace(entry.unitName)
                ? entry.prefab.name
                : entry.unitName;

            if (unitPrefabMap.ContainsKey(key))
            {
                DebugTool.Warnning($"중복 유닛 프리팹 키가 있습니다. key={key}", DebugType.Data, this);
                continue;
            }

            unitPrefabMap.Add(key, entry.prefab);
        }
    }

    private void PrepareSpawnedTower(TowerUnit towerUnit, UnitData data)
    {
        if (towerUnit == null)
            return;

        towerUnit.gameObject.name = data.UnitName;

        if (towerUnit.GetComponent<TowerLongPressDragHandler>() == null)
            towerUnit.gameObject.AddComponent<TowerLongPressDragHandler>();
    }

    private UnitStat ApplyUnitStat(GameObject unitObject, UnitData unitData)
    {
        UnitStat stat = unitObject.GetComponent<UnitStat>();
        if (stat == null)
            stat = unitObject.AddComponent<UnitStat>();

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

        return stat;
    }

    private void RegisterOwnedTower(UnitData data, TowerUnit towerUnit, UnitStat stat)
    {
        CleanupNullOwnedTowers();

        ownedTowers.Add(new SummonedTowerRecord
        {
            summonIndex = ++summonSequence,
            unitName = data.UnitName,
            tier = data.Tier,
            unitData = data,
            towerUnit = towerUnit,
            unitStat = stat
        });
    }

    private void CleanupNullOwnedTowers()
    {
        ownedTowers.RemoveAll(record => record == null || record.towerUnit == null);
    }
}