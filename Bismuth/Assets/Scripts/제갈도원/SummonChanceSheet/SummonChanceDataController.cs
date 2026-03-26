using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SummonChanceDataController : MonoBehaviour
{
    [Header("━━━━ SO 설정 ━━━━")]
    [Tooltip("기본 소환 확률 데이터 SO 입니다.")]
    [SerializeField] private SummonChanceSO summonChanceSo;

    [Header("━━━━ 시트 설정 ━━━━")]
    [Tooltip("소환 확률 시트 Url 입니다.")]
    [SerializeField] private SheetData _summonChanceSheet;

    private List<SummonChanceData> _rows = new();

    [SerializeField] private bool log;

    private Dictionary<int, SummonChanceData> _dataByEnhancementLevel = new();

    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Data, log);

        // 1) SO가 있으면 먼저 기본값 로드
        if (summonChanceSo != null)
        {
            _rows = new List<SummonChanceData>(summonChanceSo.Rows);
            RebuildCache();
            DebugTool.Log($"[소환 확률] SO {_rows.Count}행 로드 완료", DebugType.Data, this);
        }

        // 2) SheetData가 있으면 런타임 시트 값으로 갱신
        if (_summonChanceSheet != null)
        {
            StartCoroutine(_summonChanceSheet.Load(OnSheetLoaded));
            return;
        }

        // 3) SO/시트 둘 다 없으면 경고
        if (summonChanceSo == null)
        {
            DebugTool.Warnning("SummonChanceTableSO 또는 summonChanceSheet 중 하나는 할당되어야 합니다.", DebugType.Data, this);
        }
    }

    private void OnSheetLoaded(char splitSymbol, string[] lines)
    {
        _rows.Clear();

        // 헤더 1줄만 있고 데이터 없으면 return 
        if (lines == null || lines.Length < 2) return;

        // 데이터 1행 부터 시작
        for (int i = 1; i < lines.Length; i++)
        {
            // 셀 배열을 객체로 반환 시키고
            string[] cells = lines[i].Split(splitSymbol);
            SummonChanceData row = SummonChanceData.Create(cells);

            // 반환이 되면 리스트에 담기
            if (row != null)
                _rows.Add(row);
        }

        RebuildCache();

        // 시트 값을 SO에도 반영(런타임 메모리 기준)
        if (summonChanceSo != null)
            summonChanceSo.SetRows(_rows);

        DebugTool.Log($"[소환 확률] {_rows.Count}행 로드 완료", DebugType.Data, this);
    }

    private void RebuildCache()
    {
        _dataByEnhancementLevel.Clear();

        for (int i = 0; i < _rows.Count; i++)
        {
            SummonChanceData data = _rows[i];
            if (data == null)
                continue;

            if (_dataByEnhancementLevel.ContainsKey(data.EnhancementLevel))
            {
                DebugTool.Warnning($"중복 강화 단계 데이터가 있습니다. Level : {data.EnhancementLevel}", DebugType.Data, this);
                continue;
            }

            _dataByEnhancementLevel.Add(data.EnhancementLevel, data);
        }
    }
}