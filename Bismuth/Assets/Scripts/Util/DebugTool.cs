using UnityEngine;

/// <summary>
/// 디버그 타입에 따라 로그를 필터링하고 색상 및 출처 정보를 함께 출력하는 공용 유틸 클래스이다.
/// 일반적인 로그 출력
///  - Log(..)
/// 중요한 경고, 치명적이진 않은 상황 로그 출력
///  - Warn(..)
/// 데이터 누락, null 참조, 치명적인 상황 로그 출력
///  - Error(..)
/// </summary>
public static class DebugTool
{
    public static bool[] DebugTypeSelect = new bool[System.Enum.GetValues(typeof(DebugType)).Length];

    public static void Log(string text, DebugType type, Object context = null)
    {
        if (!DebugTypeSelect[(int)type])
            return;

        string color = GetColor(type);
        string ctxSource = context != null ? context.name : "None";
        
        Debug.Log($"<color={color}>[{type}] {text}</color>\n" +
                  $"<color=#daa520>출처 : [{ctxSource}]</color>", context);
    }
    
    public static void Warnning(string text, DebugType type, Object context = null)
    {
        if (!DebugTypeSelect[(int)type])
            return;

        string color = GetColor(type);
        string ctxSource = context != null ? context.name : "None";
        
        Debug.LogWarning($"<color={color}>[{type}] {text}</color>\n" +
                  $"<color=#daa520>출처 : [{ctxSource}]</color>", context);
    }
    
    public static void Error(string text, DebugType type, Object context = null)
    {
        string color = GetColor(type);
        string ctxSource = context != null ? context.name : "None";
        
        Debug.LogError($"<color={color}>[{type}] {text}</color>\n" +
                  $"<color=#daa520>출처 : [{ctxSource}]</color>", context);
    }

    public static void DebugAllOn()
    {
        for (int i = 0; i < DebugTypeSelect.Length; i++)
            DebugTypeSelect[i] = true;
    }

    public static void DebugAllOff()
    {
        for (int i = 0; i < DebugTypeSelect.Length; i++)
            DebugTypeSelect[i] = false;
    }

    public static void DebugSelect(DebugType type, bool value)
    {
        DebugTypeSelect[(int)type] = value;
    }

    private static string GetColor(DebugType type)
    {
        switch (type)
        {
            case DebugType.Game: return "#c6a1fa";
            case DebugType.Tower: return "#d9c61c";
            case DebugType.Wave: return "#c53d34";
            case DebugType.Board: return "#bdd3b5";
            case DebugType.Enemy: return "#19cd48";
            case DebugType.UI: return "#b15b8b";
            case DebugType.Data: return "#e4ada4";
            case DebugType.Merge: return "#0eb6a6";
            case DebugType.Summon: return "#5eaad9";
            case DebugType.Combine: return "#F45911";
            case DebugType.Reforge: return "#A35ED3";
            case DebugType.Default: return "#251f59";
            default: return "#ffffff";
        }
    }
}

public enum DebugType
{
    Game = 0,
    Tower = 1,
    Wave = 2,
    Board = 3,
    Enemy = 4,
    UI = 5,
    Data = 6,
    Merge = 7,
    Summon = 8,
    Combine = 9,
    Reforge = 10,
    Default = 11
}