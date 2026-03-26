using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SummonChanceDataController : MonoBehaviour
{
    [Header("━━━━ 시트 설정 ━━━━")]
    [Tooltip("소환 확률 시트 Url 입니다.")]
    [SerializeField] private SheetData _summonChanceSheet;

    [Header("━━━━ 로그 결과 ━━━━")]
    [SerializeField]
    private List<SummonChanceData> _rows = new();

    public IReadOnlyList<SummonChanceData> Rows => _rows;
    [SerializeField] private bool log;


    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Data, log);

        if (_summonChanceSheet == null)
        {
            DebugTool.Warnning("summonChanceSheet 가 할당되지 않았습니다.", DebugType.Data, this);
            return;
        }

        StartCoroutine(_summonChanceSheet.Load(OnSheetLoaded));

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
        DebugTool.Log($"[소환 확률] {_rows.Count}행 로드 완료", DebugType.Data, this);
    }


    // 디버깅용 
    // SummonChanceTester.cs 에
    // 강화 단계에 해당하는 소환 확률 데이터를 반환

    public SummonChanceData GetByEnhancementLevel(int enhancementLevel)
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            if (_rows[i].EnhancementLevel == enhancementLevel)
                return _rows[i];
        }

        return null;
    }
    
    public int GetRandomTier(int enhancementLevel)
    {
        SummonChanceData data = GetByEnhancementLevel(enhancementLevel);

        if (data == null)
        {
            DebugTool.Warnning($"강화 단계 {enhancementLevel} 데이터가 없습니다.", DebugType.Data, this);
            return -1;
        }

        float randomValue = Random.Range(0f, 100f);

        if (randomValue < data.Tier1)
            return 1;

        if (randomValue < data.Tier1 + data.Tier2)
            return 2;

        if (randomValue < data.Tier1 + data.Tier2 + data.Tier3)
            return 3;

        return 4;
    }

    /// <summary>
    /// 현재 강화 단계 데이터 로그 출력
    /// </summary>
    public void PrintChanceData(int enhancementLevel)
    {
        SummonChanceData data = GetByEnhancementLevel(enhancementLevel);

        if (data == null)
        {
            DebugTool.Warnning($"강화 단계 {enhancementLevel} 데이터가 없습니다.", DebugType.Data, this);
            return;
        }

        DebugTool.Log(
            $"[확률 데이터] 강화 단계: {data.EnhancementLevel} / " +
            $"1등급: {data.Tier1}, 2등급: {data.Tier2}, 3등급: {data.Tier3}, 4등급: {data.Tier4}, 합계: {data.SumPersent}",
            DebugType.Data,
            this
        );
    }
}