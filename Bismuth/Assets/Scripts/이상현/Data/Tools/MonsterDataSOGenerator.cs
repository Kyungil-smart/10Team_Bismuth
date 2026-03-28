using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MonsterDataSOGenerator : MonoBehaviour
{
    [Header("===입력 데이터===")] 
    [Tooltip("몬스터 행데이터를 보관한 MonsterSheetRowProvider")] 
    [SerializeField] private MonsterSheetRowProvider _monsterSheetRowProvider;

    [Header("===출력 경로===")] 
    [Tooltip("MonsterDataSO 생성 및 갱신할 폴더 경로\nAssets부터 시작")] 
    [SerializeField] private string _outputFolderPath =
        "Assets/Data/이상현/SO/Monsters";
    
    [ContextMenu("몬스터 SO 생성/갱신")]
    public void GenerateOrUpdateMonsterDataSO()
    {
#if UNITY_EDITOR
        if (_monsterSheetRowProvider == null)
        {
            DebugTool.Error("MonsterSheetRowProvider가 연결되지 않았습니다.",
                DebugType.Data, this);
            return;
        }

        if (!_monsterSheetRowProvider.IsLoaded)
        {
            DebugTool.Error("MonsterSheetRowProvider가 아직 로드되지 않았습니다.",
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

        int createdCount = 0;
        int updatedCount = 0;
        foreach (MonsterSheetRow row in _monsterSheetRowProvider.Rows)
        {
            string assetPath = $"{_outputFolderPath}/Monster_{row.Id}.asset";

            MonsterDataSO monsterData = AssetDatabase.LoadAssetAtPath<MonsterDataSO>(assetPath);
            
            bool isCreated = false;
            if (monsterData == null)
            {
                monsterData = ScriptableObject.CreateInstance<MonsterDataSO>();
                AssetDatabase.CreateAsset(monsterData, assetPath);
                isCreated = true;
            }

            MonsterDataValues values = ConvertRowToValues(row);
            monsterData.OverwriteData(values);

            EditorUtility.SetDirty(monsterData);

            if (isCreated)
                createdCount++;
            else
                updatedCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        DebugTool.Log($"몬스터 SO 생성/갱신 완료 - 생성 : {createdCount}, 갱신 : {updatedCount}",
            DebugType.Data, this);
#endif
    }

#if UNITY_EDITOR
    private MonsterDataValues ConvertRowToValues(MonsterSheetRow row)
    {
        MonsterDataValues values = new MonsterDataValues();
        values.Id = row.Id;
        values.DisplayName = row.Name;
        values.Category = ConvertCategory(row.Type);
        values.BaseHp = row.BaseHp;
        values.HpGrowth = row.HpGrowth;
        values.BaseDefense = row.BaseDefense;
        values.DefenseGrowth = row.DefenseGrowth;
        values.MoveSpeed = row.MoveSpeed;
        values.MoveType = ConvertMoveType(row.MoveType);
        values.BaseDamageToBase = row.BaseDamageToBase;
        values.KillReward = row.KillReward;
        return values;
    }

    private MonsterCategory ConvertCategory(string typeText)
    {
        if (string.IsNullOrWhiteSpace(typeText))
            return MonsterCategory.Normal;

        string normalized = typeText.Trim();

        if (normalized == "보스" || normalized.ToLower() == "boss")
            return MonsterCategory.Boss;

        return MonsterCategory.Normal;
    }

    private MonsterMoveType ConvertMoveType(string moveTypeText)
    {
        if (string.IsNullOrWhiteSpace(moveTypeText))
            return MonsterMoveType.Normal;

        string normalized = moveTypeText.Trim();

        if (normalized == "대기 후 순간이동")
            return MonsterMoveType.WaitAndTeleport;

        return MonsterMoveType.Normal;
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
