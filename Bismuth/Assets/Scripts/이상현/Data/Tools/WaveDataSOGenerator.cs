using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaveDataSOGenerator : MonoBehaviour
{
    [Header("===입력 데이터===")]    
    [Tooltip("웨이브 행 데이터를 보관한 프로바이더")]
    [SerializeField] private WaveSheetRowProvider _waveSheetRowProvider;
    
    [Header("===몬스터 SO 검색 경로===")]
    [Tooltip("MonsterDataSO를 검색할 폴더 경로\nAssets부터 시작")]
    [SerializeField] private string _monsterDataFolderPath =
        "Assets/Data/이상현/SO/Monsters";
    
    [Header("===출력 경로===")]
    [Tooltip("WaveDataSO 생성 및 갱신할 폴더 경로\nAssets부터 시작")]
    [SerializeField] private string _outputFolderPath = 
        "Assets/Data/이상현/SO/Waves";

    [ContextMenu("웨이브 SO 생성/갱신")]
    public void GenerateOrUpdateWaveDataSO()
    {
#if UNITY_EDITOR
        if (_waveSheetRowProvider == null)
        {
            DebugTool.Error("WaveSheetRowProvider가 연결되지 않았습니다.",
                DebugType.Data, this);
            return;
        }

        if (!_waveSheetRowProvider.IsLoaded)
        {
            DebugTool.Error("WaveSheetRowProvider가 아직 로드되지 않았습니다.",
                DebugType.Data, this);
            return;
        }

        if (string.IsNullOrWhiteSpace(_monsterDataFolderPath)
            || !_monsterDataFolderPath.StartsWith("Assets"))
        {
            DebugTool.Error("MonsterDataSO 검색 경로는 'Assets로 시작해야 합니다.",
                DebugType.Data, this);
            return;    
        }

        if (string.IsNullOrWhiteSpace(_outputFolderPath)
            || !_outputFolderPath.StartsWith("Assets"))
        {
            DebugTool.Error("출력 경로는 'Assets'로 시작해야 합니다.",
                DebugType.Data, this);
            return;
        }
        
        EnsureFolderExists(_outputFolderPath);

        // 몬스터 ID -> MonsterDataSO 생성
        Dictionary<int, MonsterDataSO> monsterDataById = BuildMonsterLookup();
        // 난이도 보정 ID + 웨이브 번호 기준으로 그룹화
        Dictionary<string, List<WaveSheetRow>> rowsByWaveKey = GroupRowsByWaveKey();

        int createdCount = 0;
        int updatedCount = 0;

        // 그룹별로 WaveDataSo 생성/갱신
        foreach (KeyValuePair<string, List<WaveSheetRow>> pair in rowsByWaveKey)
        {
            List<WaveSheetRow> waveRows = pair.Value;
            WaveSheetRow firstRow = waveRows[0];

            int difficultyModifierId = firstRow.DifficultyModifierId;
            int waveNumber = firstRow.WaveNumber;
            
            // 같은 웨이브 번호라도 난이도 보정 ID가 다르면 다른게 에셋 분리
            // 예) 난이도보정아이디_웨이브번호.asset
            string assetPath = $"{_outputFolderPath}/Wave_{difficultyModifierId}_{waveNumber}.asset";
            
            // Row 묶음을 SO 전달용 values로 변환  
            if (!TryConvertRowsToValues(waveRows, monsterDataById, out WaveDataValues values))
            {
                DebugTool.Error($"Wave_{difficultyModifierId}_{waveNumber} 생성/갱신 중 웨이브 values 변환에 실패했습니다.",
                    DebugType.Data, this);
                continue;
            }
            
            WaveDataSO waveData = AssetDatabase.LoadAssetAtPath<WaveDataSO>(assetPath);
            
            bool isCreated = false;
            if (waveData == null)
            {
                waveData = ScriptableObject.CreateInstance<WaveDataSO>();
                AssetDatabase.CreateAsset(waveData, assetPath);
                isCreated = true;
            }
            
            waveData.OverwriteData(values);
            EditorUtility.SetDirty(waveData);
            
            if (isCreated)
                createdCount++;
            else
                updatedCount++;
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        DebugTool.Log($"웨이브 SO 생성/갱신 완료 - 생성 : {createdCount}, 갱신 : {updatedCount}",
            DebugType.Data, this);
#endif
    }
    
#if UNITY_EDITOR
    
    /// 같은 웨이브 그룹을 WaveDataSO에 넣을 values 구조로 변환
    private bool TryConvertRowsToValues(
        List<WaveSheetRow> waveRows,
        Dictionary<int, MonsterDataSO> monsterDataById,
        out WaveDataValues values)
    {
        values = new WaveDataValues
        {
            WaveNumber = 0,
            DifficultyModifierId = 0,
            ClearReward = 0,
            SpawnEntries = new List<WaveSpawnEntryValues>()
        };

        if (waveRows == null || waveRows.Count == 0)
        {
            DebugTool.Error("변환할 웨이브 행 데이터가 없습니다.",
                DebugType.Data, this);
            return false;
        }
        
        // 같은 그룹의 대표값은 첫행 기준
        WaveSheetRow firstRow = waveRows[0];

        values.WaveNumber = firstRow.WaveNumber;
        values.DifficultyModifierId = firstRow.DifficultyModifierId;
        values.ClearReward = firstRow.ClearReward;
        values.SpawnEntries = new List<WaveSpawnEntryValues>(waveRows.Count);
        
        foreach (WaveSheetRow row in waveRows)
        {
            if (row.WaveNumber != values.WaveNumber)
            {
                DebugTool.Error("같은 웨이브 그룹 안에 다른 웨이브 번호가 섞여 있습니다.",
                    DebugType.Data, this);
                return false;
            }

            if (row.DifficultyModifierId != values.DifficultyModifierId)
            {
                DebugTool.Error("같은 웨이브 그룹 안에 다른 난이도 보정 ID가 섞여 있습니다.",
                    DebugType.Data, this);
                return false;
            }
            
            if (row.ClearReward != values.ClearReward)
            {
                DebugTool.Error("같은 웨이브 그룹 안에 다른 클리어 보상 값이 섞여 있습니다.",
                    DebugType.Data, this);
                return false;
            }
            
            if (!monsterDataById.TryGetValue(row.MonsterId, out MonsterDataSO monsterData))
            {
                DebugTool.Error($"MonsterId에 해당하는 MonsterDataSO를 찾지 못했습니다. ID : {row.MonsterId}",
                    DebugType.Data, this);
                return false;
            }
            
            WaveSpawnEntryValues entryValues = new WaveSpawnEntryValues
            {
                MonsterData = monsterData,
                Count = row.Count,
                SpawnInterval = row.SpawnInterval,
                StartDelay = row.StartDelay
            };
            
            values.SpawnEntries.Add(entryValues);
        }
        
        return true;
    }
    
    /// <summary>
    /// 웨이브 시트 행을 "난이도 보정 ID + 웨이브 번호" 기준으로 묶음
    /// 같은 그룹 안에서는 엔트리 순서로 정렬 (오름 차순)
    /// </summary>
    private Dictionary<string, List<WaveSheetRow>> GroupRowsByWaveKey()
    {
        Dictionary<string, List<WaveSheetRow>> rowsByWaveKey = new();

        foreach (WaveSheetRow row in _waveSheetRowProvider.Rows)
        {
            string waveKey = BuildWaveKey(row.DifficultyModifierId, row.WaveNumber);
            
            if (!rowsByWaveKey.TryGetValue(waveKey, out List<WaveSheetRow> list))
            {
                list = new List<WaveSheetRow>();
                rowsByWaveKey.Add(waveKey, list);
            }
            
            list.Add(row);
        }

        foreach (List<WaveSheetRow> waveRows in rowsByWaveKey.Values)
        {
            waveRows.Sort((a, b) => a.EntryOrder.CompareTo(b.EntryOrder));
        }
        
        return rowsByWaveKey;
    }
    
    private string BuildWaveKey(int difficultyModifierId, int waveNumber)
    {
        return $"{difficultyModifierId}_{waveNumber}";
    }
    
    // MonsterDataSO 에셋을 찾아 ID 기준 조회용 Dictionary를 생성
    private Dictionary<int, MonsterDataSO> BuildMonsterLookup()
    {
        Dictionary<int, MonsterDataSO> monsterDataById = new();
        
        string[] guids = AssetDatabase.FindAssets(
            "t:MonsterDataSO", new [] {_monsterDataFolderPath});

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            MonsterDataSO monsterData = AssetDatabase.LoadAssetAtPath<MonsterDataSO>(assetPath);

            if (monsterData == null) continue;

            monsterDataById[monsterData.Id] = monsterData;
        }
        
        return monsterDataById;
    }
    
    // SO 저장 경로가 없으면 상위 폴더부터 순서대로 생성
    private void EnsureFolderExists(string assetsFolderPath)
    {
        string[] parts = assetsFolderPath.Split('/');

        if (parts.Length < 2 || parts[0] != "Assets")
            return;

        string currentPath = "Assets";

        for (int i = 1; i < parts.Length; i++)
        {
            string nextFolderName = parts[i];
            string nextPath = $"{currentPath}/{nextFolderName}";

            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, nextFolderName);
            }

            currentPath = nextPath;
        }
    }
#endif
}
