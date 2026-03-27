using System.Collections.Generic;
using UnityEngine;


public class WaveSheetRowProvider : MonoBehaviour
{
    [Header("===구글 시트 로더===")] 
    [Tooltip("웨이브 시트 CSV를 불러오는 설정입니다.")] 
    [SerializeField] private GoogleSheetLoader _waveSheetLoader;

    [Header("===런타임 로드 상태===")]
    [Tooltip("웨이브 시트 로드가 성공했는지 여부")]
    [SerializeField] private bool _isLoaded;
    
    [Tooltip("보관 중인 웨이브 행 데이터 개수")]
    [SerializeField] private int _loadedCount;
    
    private readonly List<WaveSheetRow> _rows = new();
    private readonly Dictionary<int, List<WaveSheetRow>> _rowsByWaveNumber = new();
    
    private WaveSheetParser _waveSheetParser;
    
    public IReadOnlyList<WaveSheetRow> Rows => _rows;
    public bool IsLoaded => _isLoaded;
    public int LoadedCount => _loadedCount;
    
    private void Awake()
    {
        _waveSheetParser = new WaveSheetParser();
    }

    private void Start()
    {
        if (string.IsNullOrWhiteSpace(_waveSheetLoader.Url))
        {
            ClearLoadedData();
            DebugTool.Error("GoogleSheetLoader URL이 비어 있습니다.",
                DebugType.Data, this);
            return;
        }
        
        StartCoroutine(_waveSheetLoader.Load(OnWaveSheetLoaded));
    }

    public bool TryGetRowsByWaveNumber(int waveNumber, out IReadOnlyList<WaveSheetRow> rows)
    {
        rows = null;

        if (!_isLoaded)
        {
            return false;
        }

        if (!_rowsByWaveNumber.TryGetValue(waveNumber, out List<WaveSheetRow> waveRows))
        {
            return false;
        }
        
        rows = waveRows;
        return true;
    }
    
    private void OnWaveSheetLoaded(string[] lines)
    {
        string csvText = string.Join("\n", lines);

        if (!_waveSheetParser.TryParse(csvText, out List<WaveSheetRow> rows))
        {
            ClearLoadedData();
            DebugTool.Error("웨이브 시트 파싱에 실패했습니다.",
                DebugType.Data, this);
            return;
        }

        if (rows.Count == 0)
        {
            ClearLoadedData();
            DebugTool.Warnning("웨이브 데이터가 비어 있습니다.",
                DebugType.Data, this);
            return;
        }
        
        StoreRows(rows);
    }
    
    private void StoreRows(List<WaveSheetRow> rows)
    {
        _rows.Clear();
        _rows.AddRange(rows);
        
        RebuildLookup();
        
        _isLoaded = true;
        _loadedCount = _rows.Count;
    }
    
    private void RebuildLookup()
    {
        _rowsByWaveNumber.Clear();

        foreach (WaveSheetRow row in _rows)
        {
            if (!_rowsByWaveNumber.TryGetValue(row.WaveNumber, out List<WaveSheetRow> waveRows))
            {
                waveRows = new List<WaveSheetRow>();
                _rowsByWaveNumber.Add(row.WaveNumber, waveRows);
            }

            waveRows.Add(row);
        }
    }

    private void ClearLoadedData()
    {
        _rows.Clear();
        _rowsByWaveNumber.Clear();
        
        _isLoaded = false;
        _loadedCount = 0;
    }
}