using System.Collections.Generic;
using UnityEngine;

public class SynergyDataController : MonoBehaviour
{
    private const int MaxColumnIndex = 16; // A~Q

    [Header("━━━━ SO 설정 ━━━━")] [Tooltip("기본 시너지 데이터 SO 입니다.")] [SerializeField]
    private SynergySO synergySo;

    [Header("━━━━ 시트 설정 ━━━━")] [Tooltip("시너지 시트 Url 입니다.")] [SerializeField]
    private SheetData _synergySheet;

    [Header("━━━━ 로드 결과 ━━━━")] [SerializeField]
    private List<SynergyData> _rows = new();

    [SerializeField] private bool log;

    private Dictionary<int, SynergyData> _dataById = new();
    private List<int> _activeColumns = new(); // 활성 단계


    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Data, log);

        // 1) SO가 있으면 먼저 기본값 로드
        if (synergySo != null)
        {
            _rows = new List<SynergyData>(synergySo.Rows);
            RebuildCache();
            DebugTool.Log($"[시너지] SO {_rows.Count}행 로드 완료", DebugType.Data, this);
        }

        // 2) SheetData가 있으면 런타임 시트 값으로 갱신
        if (_synergySheet != null)
        {
            StartCoroutine(_synergySheet.Load(OnSheetLoaded));
            return;
        }
    }

    private void OnSheetLoaded(char splitSymbol, string[] lines)
    {
        _rows.Clear();

        // 헤더 1줄만 있고 데이터 없으면 return
        if (lines == null || lines.Length < 2)
            return;

        string[] headerCells = lines[0].Split(splitSymbol);
        CacheActiveColumns(headerCells);

        if (_activeColumns.Count == 0)
        {
            DebugTool.Warnning("[시너지] 활성 단계 컬럼을 찾지 못했습니다.", DebugType.Data, this);
            return;
        }

        // 데이터 1행부터 시작
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] cells = lines[i].Split(splitSymbol);
            SynergyData row = SynergyData.Create(cells, _activeColumns);

            if (row != null)
                _rows.Add(row);
        }

        RebuildCache();

        // 시트 값을 SO에도 반영
        if (synergySo != null)
            synergySo.SetRows(_rows);

        DebugTool.Log($"[시너지] {_rows.Count}행 로드 완료", DebugType.Data, this);

        if (log)
            PrintAllRows();
    }

    
    // 활성 단계만 찾기
    private void CacheActiveColumns(string[] headerCells)
    {
        _activeColumns.Clear();
    
        if (headerCells == null || headerCells.Length == 0)
            return;

        // Q 열 까지 확인 할 거임
        for (int i = 0; i < headerCells.Length; i++)
        {
            if (i > MaxColumnIndex)
                break;
    
            // 헤더 텍스르를 가져와 일단
            string header = headerCells[i]?.Trim();
            if (string.IsNullOrEmpty(header))
                continue;

            // 활성 단계가 1~5 헤더면 리스트에 담기 
            if (IsActiveColumn(header))
                _activeColumns.Add(i);
        }
    }

    
    // 활성단계만 찾음
    private static bool IsActiveColumn(string header)
    {
        string normalized = header.Replace(" ", "");

        switch (normalized)
        {
            case "활성단계1":
            case "활성단계2":
            case "활성단계3":
            case "활성단계4":
            case "활성단계5":
                return true;
            default:
                return false;
        }
    }

    private void RebuildCache()
    {
        _dataById.Clear();

        for (int i = 0; i < _rows.Count; i++)
        {
            SynergyData data = _rows[i];

            if (data == null)
                continue;

            if (_dataById.ContainsKey(data.ID))
            {
                DebugTool.Warnning($"중복 시너지 ID 데이터가 있습니다. ID : {data.ID}", DebugType.Data, this);
                continue;
            }

            _dataById.Add(data.ID, data);
        }
    }

    public SynergyData GetById(int id)
    {
        if (_dataById.TryGetValue(id, out SynergyData data))
            return data;

        return null;
    }

    private void PrintAllRows()
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            PrintRow(_rows[i]);
        }
    }

    private void PrintRow(SynergyData data)
    {
        if (data == null)
            return;

        string levelLog = string.Empty;

        for (int i = 0; i < data.Levels.Count; i++)
        {
            SynergyLevelData level = data.Levels[i];
            levelLog += $"[{level.ActiveCount}명 : {string.Join(", ", level.EffectValues)}] ";
        }

        DebugTool.Log(
            $"[시너지 데이터] ID : {data.ID} / " +
            $"이름 : {data.SynergyName} / " +
            $"단계 : {levelLog}",
            DebugType.Data,
            this
        );
    }
}