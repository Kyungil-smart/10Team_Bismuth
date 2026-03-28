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
    private readonly Dictionary<string, List<WaveSheetRow>> _rowsByModifierAndWave = new();
    
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
    
    public bool TryGetRows(int difficultyModifierId, int waveNumber, out IReadOnlyList<WaveSheetRow> rows)
    {
        rows = null;

        if (!_isLoaded)
        {
            return false;
        }

        string waveKey = BuildWaveKey(difficultyModifierId, waveNumber);
        
        if (!_rowsByModifierAndWave.TryGetValue(waveKey, out List<WaveSheetRow> waveRows))
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
    
    // 보정 계수 ID와 웨이브 번호를 묶어서 조회용 키
    private string BuildWaveKey(int difficultyModifierId, int waveNumber)
    {
        return $"{difficultyModifierId}_{waveNumber}";
    }
    
    /// <summary>
    /// 보정 계수 ID + 웨이브 번호 기준으로 생성
    /// 같은 웨이브 안에서 엔트리 순서 기준으로 정렬
    /// </summary>
    private void RebuildLookup()
    {
        _rowsByModifierAndWave.Clear();

        foreach (WaveSheetRow row in _rows)
        {
            string waveKey = BuildWaveKey(row.DifficultyModifierId, row.WaveNumber);
            
            if (!_rowsByModifierAndWave.TryGetValue(waveKey, out List<WaveSheetRow> waveRows))
            {
                waveRows = new List<WaveSheetRow>();
                _rowsByModifierAndWave.Add(waveKey, waveRows);
            }

            waveRows.Add(row);
        }
        
        foreach (List<WaveSheetRow> waveRows in _rowsByModifierAndWave.Values)
        {
            waveRows.Sort((a, b) => a.EntryOrder.CompareTo(b.EntryOrder));
        }
    }

    private void ClearLoadedData()
    {
        _rows.Clear();
        _rowsByModifierAndWave.Clear();
        
        _isLoaded = false;
        _loadedCount = 0;
    }
}