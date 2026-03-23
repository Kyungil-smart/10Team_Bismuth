using System.Collections;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Header("━━━━ 시트 설정 ━━━━")]
    [Tooltip("구글시트 Unit 시트 URL\n공유 → 링크로 가져오기")]
    [SerializeField] private SheetData unitSheet;

    [Header("━━━━ 유닛 DB ━━━━")]
    [Tooltip("시트 데이터가 채워질 UnitSO 에셋\nCreate > Bismuth > Unit Database 로 생성 후 할당")]
    [SerializeField] private UnitSO unitDatabase;

    private void Start()
    {
        if (unitSheet == null || unitDatabase == null)
        {
            Debug.LogError("<color=red>DataManager: unitSheet 또는 unitDatabase가 할당되지 않았습니다.</color>");
            return;
        }
        StartCoroutine(unitSheet.Load(SetUnitDatas));
    }

    /// <summary>
    /// 구글시트 로드 완료 시 호출 - 각 행을 파싱하여 UnitDatabase에 추가
    /// </summary>
    private void SetUnitDatas(char splitSymbol, string[] lines)
    {
        if (lines == null || lines.Length < 2) return;

        unitDatabase.ClearUnits();

        // 0행: 헤더, 1행부터 데이터
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(splitSymbol);
            UnitData unitData = UnitData.CreateFromSheetRow(cells);
            if (unitData != null)
                unitDatabase.AddUnit(unitData);
        }

        Debug.Log($"<color=cyan>Unit Database 로드 완료: {unitDatabase.Units.Count}개 유닛</color>");
    }
}
