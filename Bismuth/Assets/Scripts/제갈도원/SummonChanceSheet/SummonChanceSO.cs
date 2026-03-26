using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonChanceTableSO", menuName = "Bismuth/Summon Chance Table")]
public class SummonChanceSO : ScriptableObject
{
    [Header("강화 단계별 소환 확률")]
    [SerializeField] private List<SummonChanceData> _rows = new();

    private Dictionary<int, SummonChanceData> _dataByEnhancementLevel = new();

    public IReadOnlyList<SummonChanceData> Rows => _rows;

    private void OnEnable()
    {
        RebuildCache();
    }

    public void SetRows(List<SummonChanceData> rows)
    {
        _rows = rows != null ? new List<SummonChanceData>(rows) : new List<SummonChanceData>();
        RebuildCache();
    }

    public void RebuildCache()
    {
        _dataByEnhancementLevel.Clear();

        for (int i = 0; i < _rows.Count; i++)
        {
            SummonChanceData data = _rows[i];
            if (data == null)
                continue;

            if (_dataByEnhancementLevel.ContainsKey(data.EnhancementLevel))
            {
                Debug.LogWarning($"중복 강화 단계 데이터가 있습니다. Level : {data.EnhancementLevel}", this);
                continue;
            }

            _dataByEnhancementLevel.Add(data.EnhancementLevel, data);
        }
    }

    
    // 딕셔너리에서 강화 단계 키 찾기
    public SummonChanceData GetByEnhancementLevel(int enhancementLevel)
    {
        if (_dataByEnhancementLevel.TryGetValue(enhancementLevel, out SummonChanceData data))
            return data;

        return null;
    }
}
