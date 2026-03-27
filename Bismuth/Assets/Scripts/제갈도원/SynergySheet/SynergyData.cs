using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SynergyData
{
    private const int MaxColumnIndex = 16; // A~Q

    [Header("기본 정보")]
    [SerializeField] private string _synergyName;
    [SerializeField] private int _id;

    [Header("단계별 효과")]
    [SerializeField] private List<SynergyLevelData> _levels = new();

    public int ID => _id;
    public string SynergyName => _synergyName;
    public List<SynergyLevelData> Levels => _levels;

    public SynergyData(int id, string synergyName, List<SynergyLevelData> levels)
    {
        _id = id;
        _synergyName = synergyName;
        _levels = levels;
    }

    // 시너지 데이터 반환
    // line 한 줄 받아서, 시너지 데이터 객체로 반환
    // | A |   B   | C |    D     |  E   |    F        | G |     H    |
    // | 1 | 전사  | 2 | 공격력+10 |  4   |  공격력+20  | 6 | 공격력+30 |
    // activeColumns : 얘가 활성 단계 숫자가 들어있는 열 인덱스 -> 활성 단계 시작 인덱스 
    public static SynergyData Create(string[] line, IReadOnlyList<int> activeColumns)
    {
        // if (line == null || line.Length < 3 || activeColumns == null || activeColumns.Count == 0)
        //     return null;

        // ID 파싱
        if (!int.TryParse(GetCell(line, 0), out int id))
            return null;

        // B 열 에 있는 애들 이름 읽기
        string synergyName = GetCell(line, 1);
    
        // 레벨 리스트 인데 여기서 단계별로 뭐가 활성화가 되는지
        List<SynergyLevelData> levels = new();

        // 순서대로 처리하고
        for (int i = 0; i < activeColumns.Count; i++)
        {
            int activeColumn = activeColumns[i]; // activeColumn : 시작 되는 열
                                                 // nextActiveColumn : 다음 엑티브가 시작되는 열
            int nextActiveColumn = i + 1 < activeColumns.Count ? activeColumns[i + 1] : MaxColumnIndex + 1;
            string activeCountText = GetCell(line, activeColumn);

            // 공백
            if (string.IsNullOrWhiteSpace(activeCountText))
                continue;

            // 정수 반환
            if (!int.TryParse(activeCountText, out int activeCount))
                continue;

            List<string> effectValues = new();

            // 효과값 읽기 Q 열 까지
            for (int col = activeColumn + 1; col < nextActiveColumn; col++)
            {
                if (col > MaxColumnIndex || col >= line.Length)
                    break;

                string cell = GetCell(line, col);

                if (string.IsNullOrWhiteSpace(cell))
                    continue;

                effectValues.Add(cell);
            }

            SynergyLevelData levelData = new SynergyLevelData();
            levelData.SetData(activeCount, effectValues);
            levels.Add(levelData);
        }

        return new SynergyData(id, synergyName, levels);
    }

    
    // 셀 값 불러오기 계속 사용 중 ㅇㅇ
    private static string GetCell(string[] line, int index)
    {
        if (line.Length <= index)
            return string.Empty;

        return line[index].Trim();
    }

}