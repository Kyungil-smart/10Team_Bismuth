using System.Collections;
using UnityEngine;

public class UnitDataController : MonoBehaviour
{
    [Header("━━━━ 시트 설정 ━━━━")]
    [Tooltip("구글시트 Unit 시트 URL\n공유 → 링크로 가져오기")]
    [SerializeField] private SheetData unitSheet;

    [Header("━━━━ 유닛 DB ━━━━")]
    [Tooltip("시트 데이터가 채워질 UnitSO 에셋\nCreate > Bismuth > Unit Database 로 생성 후 할당")]
    
    public static int MaxTier = 4;
    public static int AllUnitCount;
    public static int Tier1UnitCount;
    public static int Tier2UnitCount;
    public static int Tier3UnitCount;
    public static int Tier4UnitCount;
    
    [SerializeField] private UnitSO[] unitDatabase = new UnitSO[4];
    public UnitSO[] UnitDatabase => unitDatabase;
    
    public bool DataLog = false;

    [SerializeField] private bool _log;
    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Data, _log);
        
        if (unitSheet == null || unitDatabase == null)
        {
            DebugTool.Warnning("unitSheet 또는 unitDatabase가 할당되지 않았습니다.", DebugType.Data, this);
            return;
        }
        
        DebugTool.Warnning("[UnitSheet] 와  [UnitDatabase]를 성공적으로 불러왔습니다.", DebugType.Data, this);
        
        StartCoroutine(unitSheet.Load(SetUnitDatas));
    }

    /// <summary>
    /// 구글시트 로드 완료 시 호출 - 각 행을 파싱하여 UnitDatabase에 추가
    /// </summary>
    private void SetUnitDatas(char splitSymbol, string[] lines)
    {
        if (lines == null || lines.Length < 2) return;

        foreach(UnitSO unit in unitDatabase)
            unit.ClearUnits();
        
        // 0행: 헤더, 1행부터 데이터
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(splitSymbol);
            UnitData unitData = UnitData.CreateFromSheetRow(cells);
            if (unitData != null)
            {
                unitDatabase[unitData.Tier-1].AddUnit(unitData);
                switch (unitData.Tier)
                {
                    case 1:
                        Tier1UnitCount++;
                        break;
                    case 2:
                        Tier2UnitCount++;
                        break;
                    case 3:
                        Tier3UnitCount++;
                        break;
                    case 4:
                        Tier4UnitCount++;
                        break;
                }
                AllUnitCount++;
            }
        }

        DebugTool.Log($"[총 {AllUnitCount} 개 유닛 로드 완료]", DebugType.Data, this);
        DebugTool.Log($"[티어 1 : {Tier1UnitCount} 개 유닛 로드 완료]", DebugType.Data, this);
        DebugTool.Log($"[티어 2 : {Tier2UnitCount} 개 유닛 로드 완료]", DebugType.Data, this);
        DebugTool.Log($"[티어 3 : {Tier3UnitCount} 개 유닛 로드 완료]", DebugType.Data, this);
        DebugTool.Log($"[티어 4 : {Tier4UnitCount} 개 유닛 로드 완료]", DebugType.Data, this);
    }
}
