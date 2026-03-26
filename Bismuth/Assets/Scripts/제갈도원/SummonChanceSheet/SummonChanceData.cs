using System;
using System.Globalization; // 국가 표준 표기법으로 변경 해줌
using UnityEngine;

[Serializable]
public class SummonChanceData
{
    [SerializeField] private string _level;
    [SerializeField] private int _enhancementLevel; // 강화 단계
    [SerializeField] private float _tier1;
    [SerializeField] private float _tier2;
    [SerializeField] private float _tier3;
    [SerializeField] private float _tier4;
    [SerializeField] private float _sumPersent;


    public int EnhancementLevel => _enhancementLevel;
    public float Tier1 => _tier1;
    public float Tier2 => _tier2;
    public float Tier3 => _tier3;
    public float Tier4 => _tier4;
    public float SumPersent => _sumPersent;


    public static SummonChanceData Create(string[] line)
    {
        if (line == null || line.Length < 6) return null;

        // 2. 강화 단계 파싱
        if (string.IsNullOrWhiteSpace(line[0]) || !int.TryParse(line[0].Trim(), out int nextLevel)) return null;

        // 3. 데이터 객체 생성
        SummonChanceData data = new();

        data._enhancementLevel = nextLevel; // 강화 단계
        data._level = $"{nextLevel + 1} 단계";
        data._tier1 = ParseFloat(SafeGet(line, 1));         // b
        data._tier2 = ParseFloat(SafeGet(line, 2));         // c
        data._tier3 = ParseFloat(SafeGet(line, 3));         // d
        data._tier4 = ParseFloat(SafeGet(line, 4));         // d
        data._sumPersent = ParseFloat(SafeGet(line, 5));    // f


        return data;
    }

    // 배열 범위 초과 , null 인 경우
    private static string SafeGet(string[] arr, int index)
    {
        return (arr != null && index < arr.Length) ? arr[index]?.Trim() ?? "" : "";
    }


    private static float ParseFloat(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0f;

        if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            return result;
        }

        return 0f;
    }
}