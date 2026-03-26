using System.Collections.Generic;
using UnityEngine;

public class UnitEnhanceDataController : MonoBehaviour
{
    [Header("━━━━ 시트 설정 ━━━━")]
    [SerializeField] private SheetData unitEnhanceSheet;

    [Header("━━━━ 로드 결과 ━━━━")]
    [SerializeField] private List<UnitEnhanceData> rows = new();

    [SerializeField] private bool log;

    public IReadOnlyList<UnitEnhanceData> Rows => rows;

    private Dictionary<int, UnitEnhanceData> _dataByUnitId = new();

    private void Start()
    {
        Debug.Log("Start 진입");

        if (unitEnhanceSheet == null)
        {
            Debug.LogError("unitEnhanceSheet가 할당되지 않았습니다.");
            return;
        }

        StartCoroutine(unitEnhanceSheet.Load(OnSheetLoaded));
    }

    private void OnSheetLoaded(char splitSymbol, string[] lines)
    {
        Debug.Log("OnSheetLoaded 호출됨");

        rows.Clear();
        _dataByUnitId.Clear();

        if (lines == null || lines.Length <= 1)
        {
            Debug.LogWarning("강화 수치 시트 데이터가 비어 있습니다.");
            return;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] line = lines[i].Split(splitSymbol);
            UnitEnhanceData data = UnitEnhanceData.Create(line);

            if (data == null)
                continue;

            rows.Add(data);

            if (_dataByUnitId.ContainsKey(data.UnitId))
            {
                Debug.LogWarning($"중복된 Unit ID가 있습니다. ID : {data.UnitId}");
                continue;
            }

            _dataByUnitId.Add(data.UnitId, data);

            if (log)
            {
                Debug.Log($"[강화 데이터 로드] Unit ID : {data.UnitId}, 강화 수치 : {data.EnhanceValue}");
            }
        }
    }

    public UnitEnhanceData GetByUnitId(int unitId)
    {
        if (_dataByUnitId.TryGetValue(unitId, out UnitEnhanceData data))
            return data;

        Debug.LogWarning($"해당 Unit ID 데이터를 찾을 수 없습니다. ID : {unitId}");
        return null;
    }

    public float GetEnhanceValueByUnitId(int unitId)
    {
        UnitEnhanceData data = GetByUnitId(unitId);
        return data != null ? data.EnhanceValue : 0f;
    }
}