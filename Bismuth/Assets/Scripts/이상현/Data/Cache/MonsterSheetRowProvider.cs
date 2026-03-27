using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterSheetRowProvider : MonoBehaviour
{
    [FormerlySerializedAs("_monsterSheet")]
    [Header("===구글 시트 로더===")]
    [Tooltip("몬스터 시트 CSV를 불러오는 설정")]
    [SerializeField] private GoogleSheetLoader _monsterSheetLoader;

    [Header("===런타임 로드 상태===")]
    [Tooltip("몬스터 시트 로드 성공 여부")]
    [SerializeField] private bool _isLoaded;

    [Tooltip("보관 중인 몬스터 행 데이터 개수")] 
    [SerializeField] private int _loadedCount;
    
    private readonly List<MonsterSheetRow> _rows = new();
    private readonly Dictionary<int, MonsterSheetRow> _rowById = new();
    
    private MonsterSheetParser _monsterSheetParser;
    
    public IReadOnlyList<MonsterSheetRow> Rows => _rows;
    public bool IsLoaded => _isLoaded;
    public int LoadedCount => _loadedCount;

    private void Awake()
    {
        _monsterSheetParser = new MonsterSheetParser();
    }

    private void Start()
    {
        if (string.IsNullOrWhiteSpace(_monsterSheetLoader.Url))
        {
            ClearLoadedData();
            DebugTool.Error("GoogleSheetLoader URL이 비어 있습니다.",
                DebugType.Data, this);
            return;
        }

        StartCoroutine(_monsterSheetLoader.Load(OnMonsterSheetLoaded));
    }

    public bool TryGetRowById(int monsterId, out MonsterSheetRow row)
    {
        if (!_isLoaded)
        {
            row = default;
            return false;
        }
        
        return _rowById.TryGetValue(monsterId, out row);
    }
    
    private void OnMonsterSheetLoaded(string[] lines)
    {
        string csvText = string.Join("\n", lines);

        if (!_monsterSheetParser.TryParse(csvText, out List<MonsterSheetRow> rows))
        {
            ClearLoadedData();
            DebugTool.Error("몬스터 시트 파싱에 실패했습니다.", DebugType.Data, this);
            return;
        }

        if (rows.Count == 0)
        {
            ClearLoadedData();
            DebugTool.Warnning("몬스터 데이터가 비어 있습니다.", DebugType.Data, this);
            return;
        }
        
        StoreRows(rows);
    }
    
    private void StoreRows(List<MonsterSheetRow> rows)
    {
        _rows.Clear();
        _rows.AddRange(rows);

        RebuildLookup();
        
        _isLoaded = true;
        _loadedCount = _rows.Count;
    }

    private void RebuildLookup()
    {
        _rowById.Clear();

        foreach (MonsterSheetRow row in _rows)
        {
            if (_rowById.ContainsKey(row.Id))
            {
                DebugTool.Warnning(
                    $"중복 몬스터 ID가 있습니다." +
                    $"\n마지막으로 읽은 행으로 덮어씁니다. ID: {row.Id}",
                    DebugType.Data,
                    this);
            }
            _rowById[row.Id] = row;
        }
    }

    private void ClearLoadedData()
    {
        _rows.Clear();
        _rowById.Clear();
        
        _isLoaded = false;
        _loadedCount = 0;
    }
}