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

        Dictionary<int, MonsterDataSO> monsterDataById = BuildMonsterLookup();
        Dictionary<int, List<WaveSheetRow>> rowsByWaveNumber = GroupRowsByWaveNumber();

        int createdCount = 0;
        int updatedCount = 0;

        foreach (KeyValuePair<int, List<WaveSheetRow>> pair in rowsByWaveNumber)
        {
            int waveNumber = pair.Key;
            List<WaveSheetRow> waveRows = pair.Value;

            string assetPath = $"{_outputFolderPath}/Wave_{waveNumber}.asset";
            WaveDataSO waveData = AssetDatabase.LoadAssetAtPath<WaveDataSO>(assetPath);
            
            bool isCreated = false;
            if (waveData == null)
            {
                waveData = ScriptableObject.CreateInstance<WaveDataSO>();
                AssetDatabase.CreateAsset(waveData, assetPath);
                isCreated = true;
            }

            if (!TryConvertRowsToValues(waveRows, monsterDataById, out WaveDataValues values))
            {
                DebugTool.Error($"Wave_{waveNumber} 생성/갱신 중 웨이브 values 변환에 실패했습니다.",
                    DebugType.Data, this);
                continue;
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

    private bool TryConvertRowsToValues(
        List<WaveSheetRow> waveRows,
        Dictionary<int, MonsterDataSO> monsterDataById,
        out WaveDataValues values)
    {
        values = new WaveDataValues
        {
            WaveNumber = 0,
            SpawnEntries = new List<WaveSpawnEntryValues>()
        };

        if (waveRows == null || waveRows.Count == 0)
        {
            DebugTool.Error("변환할 웨이브 행 데이터가 없습니다.",
                DebugType.Data, this);
            return false;
        }

        values.WaveNumber = waveRows[0].WaveNumber;
        values.SpawnEntries = new List<WaveSpawnEntryValues>(waveRows.Count);
        
        foreach (WaveSheetRow row in waveRows)
        {
            if (!monsterDataById.TryGetValue(row.MonsterId, out MonsterDataSO monsterData))
            {
                DebugTool.Error($"MonsterId에 해당하는 MonsterDataSO를 찾지 못했습니다!! ID : {row.MonsterId}",
                    DebugType.Data, this);
                return false;
            }

            WaveSpawnEntryValues entryValues = new WaveSpawnEntryValues()
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
    
    // 웨이브 번호 기준으로 시트 행들을 그룹화
    private Dictionary<int, List<WaveSheetRow>> GroupRowsByWaveNumber()
    {
        Dictionary<int, List<WaveSheetRow>> rowsByWaveNumber = new();

        foreach (WaveSheetRow row in _waveSheetRowProvider.Rows)
        {
            if (!rowsByWaveNumber.TryGetValue(row.WaveNumber, out List<WaveSheetRow> list))
            {
                list = new List<WaveSheetRow>();
                rowsByWaveNumber.Add(row.WaveNumber, list);
            }
            
            list.Add(row);
        }
        
        return rowsByWaveNumber;
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
