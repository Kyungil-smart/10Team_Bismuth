using System.IO;
using System.Runtime.CompilerServices;
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
    private static bool[] DebugTypeSelect = new bool[System.Enum.GetValues(typeof(DebugType)).Length];

    private static bool _debugAllOn = false;

    // 기본 로그 출력
    public static void Log(string text, DebugType type, Object context = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        // 디버그 타입과 전체 디버그 On이 false 면 리턴
        if (!_debugAllOn)
            return;
        if (!DebugTypeSelect[(int)type])
            return;

        // 타입에 따른 글자색 선택
        string color = GetColor(type);
        // 오브젝트 출처의 null 체크 null 이면 "None" 아니면 오브젝트 이름 출력
        string ctxSource = context != null ? context.name : "None";
        
        Debug.Log($"<color={color}>[{type}] {text}</color>\n" +
                  $"<color=#daa520>출처 : [{ctxSource}]</color>", context);
    }
    public static void Log(string text, DebugType type, object source = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        // 디버그 타입과 전체 디버그 On이 false 면 리턴
        if (!_debugAllOn)
            return;
        if (!DebugTypeSelect[(int)type])
            return;

        // 타입에 따른 글자색 선택
        string color = GetColor(type);
        // 오브젝트 출처의 null 체크 null 이면 "None" 아니면 오브젝트 이름 출력
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string sourceName = source?.GetType().Name ?? fileName;

        if (memberName == ".ctor")
            memberName = "생성자";
        
        Debug.Log($"<color={color}>[{type}] {text}</color>\n" +
                  $"<color=#daa520>출처 : [{sourceName}.{memberName} : {lineNumber}]</color>");
    }
    
    public static void Warnning(string text, DebugType type, Object context = null)
    {
        if (!_debugAllOn)
            return;
        if (!DebugTypeSelect[(int)type])
            return;

        string color = GetColor(type);
        string ctxSource = context != null ? context.name : "None";
        
        Debug.LogWarning($"<color={color}>[{type}] {text}</color>\n" +
                  $"<color=#daa520>출처 : [{ctxSource}]</color>", context);
    }
    
    public static void Error(string text, DebugType type, Object context = null)
    {
        if (!_debugAllOn)
            return;
        if (!DebugTypeSelect[(int)type])
            return;
        
        string color = GetColor(type);
        string ctxSource = context != null ? context.name : "None";
        
        Debug.LogError($"<color={color}>[{type}] {text}</color>\n" +
                  $"<color=#daa520>출처 : [{ctxSource}]</color>", context);
    }

    public static void MissingComponent(string text = null)
    {
        if (text == null)
        {
            Warnning($"컴포넌트를 찾을 수 없습니다.", DebugType.Missing);
            return;
        }
        Warnning($"{text}을(를) 찾을 수 없습니다.", DebugType.Missing);
    }

    public static void DebugPrintAll(bool value)
    {
        for (int i = 0; i < DebugTypeSelect.Length; i++)
            DebugTypeSelect[i] = value;
        _debugAllOn = value;
        Log($"모든 디버그 : {value}", DebugType.Game, null);
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
            case DebugType.Unit: return "#d9c61c";
            case DebugType.Synergy: return "#f0847f";
            case DebugType.Summon: return "#5eaad9";
            case DebugType.Combine: return "#F45911";
            case DebugType.Wave: return "#c53d34";
            case DebugType.Board: return "#bdd3b5";
            case DebugType.Enemy: return "#19cd48";
            case DebugType.UI: return "#b15b8b";
            case DebugType.Data: return "#e4ada4";
            case DebugType.Merge: return "#0eb6a6";
            case DebugType.Reforge: return "#A35ED3";
            case DebugType.Missing: return "#ffff00";
            case DebugType.Default: return "#251f59";
            default: return "#ffffff";
        }
    }
}

public enum DebugType
{
    Game = 0,
    Unit = 1,
    Synergy = 2,
    Summon = 3,
    Combine = 4,
    Wave = 5,
    Board = 6,
    Enemy = 7,
    UI = 8,
    Data = 9, 
    Merge = 10,
    Reforge = 11,
    Missing = 12,
    Default = 13
}