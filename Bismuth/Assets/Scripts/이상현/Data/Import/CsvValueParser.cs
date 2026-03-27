using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// CSV 한 줄의 셀 배열에서 헤더 이름을 기준으로 값을 꺼내고,
/// 데이터 타입(string, int, float)으로 변환
/// </summary>
public static class CsvValueParser
{
    private static bool TryReadRawValue(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        out string rawValue)
    {
        rawValue = string.Empty;

        if (cells == null)
        {
            DebugTool.Error("[CsvValueParser] cells 배열이 null입니다.",
                DebugType.Data);
            return false;
        }

        if (headerMap == null)
        {
            DebugTool.Error("[CsvValueParser] headerMap이 null입니다.",
                DebugType.Data);
            return false;
        }

        if (!headerMap.TryGetValue(headerName, out int columnIndex))
        {
            DebugTool.Error($"[CsvValueParser] 헤더를 찾지 못했습니다. 헤더 : " +
                            $"{headerName}", DebugType.Data);
            return false;
        }
        
        if (columnIndex < 0 || columnIndex >= cells.Length)
        {
            DebugTool.Error($"[CsvValueParser] 셀 인덱스가 범위밖 입니다." +
                            $"헤더 : {headerName}, 인덱스 : {columnIndex}", DebugType.Data);
            return false;
        }
        
        rawValue = cells[columnIndex]?.Trim() ?? string.Empty;
        return true;
    }
    
    public static bool TryReadString(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        out string value)
    {
        value = string.Empty;

        if (!TryReadRawValue(cells, headerMap, headerName, out string rawValue))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(rawValue))
        {
            DebugTool.Error($"[CsvValueParser] 문자열 값이 비어 있습니다." +
                            $"헤더 : {headerName}", DebugType.Data);
            return false;
        }

        value = rawValue;
        return true;
    }
    
    public static bool TryReadInt(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        out int value)
    {
        value = 0;

        if (!TryReadRawValue(cells, headerMap, headerName, out string rawValue))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(rawValue))
        {
            DebugTool.Error($"[CsvValueParser] Int 값이 비어 있습니다." +
                            $"헤더 : {headerName}", DebugType.Data);
            return false;
        }
        
        if (!int.TryParse(rawValue,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out value))
        {
            DebugTool.Error($"[CsvValueParser] Int 변환에 실패했습니다." +
                            $"헤더 : {headerName}, 값 : {rawValue}", DebugType.Data);
            return false;
        }

        return true;
    }
    
    public static bool TryReadFloat(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        out float value)
    {
        value = default;

        if (!TryReadRawValue(cells, headerMap, headerName, out string rawValue))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(rawValue))
        {
            DebugTool.Error($"[CsvValueParser] float 값이 비어 있습니다." +
                            $"헤더 : {headerName}", DebugType.Data);
            return false;
        }
        
        if (!float.TryParse(
                rawValue,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out value))
        {
            DebugTool.Error(
                $"[CsvValueParser] float 변환에 실패했습니다." +
                $"헤더 : {headerName}, 값 : {rawValue}", DebugType.Data);
            return false;
        }

        return true;
    }

}