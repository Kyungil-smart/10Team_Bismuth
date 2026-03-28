using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Data/Wave/Wave Data")]
public class WaveDataSO : ScriptableObject
{
    [Header("====웨이브 기본 정보====")]
    [Tooltip("웨이브 번호")]
    [SerializeField, Min(1)] private int _waveNumber = 1;
    
    [Tooltip("이 웨이브가 참조하는 난이도 보정 ID\"")]
    [SerializeField, Min(0)] private int _difficultyModifierId;

    [Tooltip("웨이브 클리어 시 지급할 보상")]
    [SerializeField, Min(0)] private int _clearReward;
    
    [Space(5)]
    [Header("====소환 엔트리 목록====")]
    [Tooltip("이번 웨이브에서 소환할 몬스터 구성 목록")]
    [SerializeField] private List<WaveSpawnEntry> _spawnEntries = new();

    public int WaveNumber => _waveNumber;
    public int DifficultyModifierId => _difficultyModifierId;
    public int ClearReward => _clearReward;
    public IReadOnlyList<WaveSpawnEntry> SpawnEntries => _spawnEntries;
    
    /// <summary>
    /// Tools 계층에서 변환한 웨이브 데이터를 현재 SO에 덮어씀
    /// 시트가 소유하는 값만 갱신하며, 엔트리 목록은 values 기준으로 다시 구성
    /// </summary>
    public void OverwriteData(WaveDataValues values)
    {
        _waveNumber = values.WaveNumber;
        _difficultyModifierId = values.DifficultyModifierId;
        _clearReward = values.ClearReward;
        
        _spawnEntries.Clear();
        
        // null 이면 엔트리 없는 웨이브처리
        if (values.SpawnEntries == null) return;

        foreach (WaveSpawnEntryValues entryValues in values.SpawnEntries)
        {
            WaveSpawnEntry entry = new();
            entry.OverwriteData(entryValues);
            
            _spawnEntries.Add(entry);
        }
    }
}

[Serializable]
public class WaveSpawnEntry
{
    [Header("====소환 대상====")]
    [Tooltip("엔트리에서 소환할 몬스터 데이터")]
    [SerializeField] private MonsterDataSO _monsterData;
    
    [Space(5)]
    [Header("====소환 수량 및 타이밍====")]
    [Tooltip("이번 엔트리에서 소환할 몬스터 수량")]
    [SerializeField, Min(1)] private int _count = 1;
    
    [Tooltip("몬스터를 몇 초 간격으로 소환할지 설정")]
    [SerializeField, Min(0)] private float _spawnInterval = 0f;
    
    [Tooltip("이 엔트리가 시작되기 전 대기 시간")]
    [SerializeField, Min(0)] private float _startDelay = 0f;
    
    public MonsterDataSO MonsterData => _monsterData;
    public int Count => _count;
    public float SpawnInterval => _spawnInterval;
    public float StartDelay => _startDelay;

    public void OverwriteData(WaveSpawnEntryValues values)
    {
        _monsterData = values.MonsterData;
        _count = values.Count;
        _spawnInterval = values.SpawnInterval;
        _startDelay = values.StartDelay;
    }
}

public struct WaveDataValues
{
    public int WaveNumber;
    public int DifficultyModifierId;
    public int ClearReward;
    public List<WaveSpawnEntryValues> SpawnEntries;
}

public struct WaveSpawnEntryValues
{
    public MonsterDataSO MonsterData;
    public int Count;
    public float SpawnInterval;
    public float StartDelay;
}