using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// CSV 한 줄의 셀 배열에서 헤더 이름을 기준으로 값을 꺼내고,
/// 데이터 타입(string, int, float)으로 안전하게 변환
/// </summary>
public static class CsvValueParser
{
    /// <summary>
    /// 지정된 헤더의 셀 값을 문자열(string)로 읽음
    /// 성공 시 true, 실패 시 false 
    /// </summary>
    public static bool TryReadString(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        out string value)
    {
        value = string.Empty;

        if (!TryGetCell(cells, headerMap, headerName, out string cellValue))
        {
            return false;
        }

        value = cellValue.Trim();
        return true;
    }
    
    /// <summary>
    /// 지정된 헤더의 셀 값을 정수(int)로 변환하여 읽음
    /// 성공 시 true, 실패할 경우 false와 에러 로그 출력
    /// </summary>
    public static bool TryReadInt(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        out int value)
    {
        value = 0;

        if (!TryGetCell(cells, headerMap, headerName, out string cellValue))
        {
            return false;
        }

        string trimmedValue = cellValue.Trim();

        if (!int.TryParse(trimmedValue, out value))
        {
            DebugTool.Error(
                $"[CsvValueParser] int 변환 실패 - 헤더: {headerName}, 값: {trimmedValue}",
                DebugType.Data);
            return false;
        }

        return true;
    }
    
    
    /// <summary>
    /// 지정된 헤더의 셀 값을 실수(float)로 변환하여 읽음
    /// 성공 시 true, 실패 시 false와 에러 로그 출력
    /// </summary>
    public static bool TryReadFloat(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        out float value)
    {
        value = 0f;

        if (!TryGetCell(cells, headerMap, headerName, out string cellValue))
        {
            return false;
        }

        string trimmedValue = cellValue.Trim();

        if (!float.TryParse(
                trimmedValue,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out value))
        {
            DebugTool.Error(
                $"[CsvValueParser] float 변환 실패 - 헤더: {headerName}, 값: {trimmedValue}",
                DebugType.Data);
            return false;
        }

        return true;
    }

    private static bool TryGetCell(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        out string cellValue)
    {
        cellValue = string.Empty;

        if (cells == null)
        {
            DebugTool.Error("[CsvValueParser] cells 배열이 null입니다.", DebugType.Data);
            return false;
        }

        if (!CsvHeaderMap.TryGetRequiredIndex(headerMap, headerName, out int columnIndex))
        {
            return false;
        }

        if (columnIndex < 0 || columnIndex >= cells.Length)
        {
            DebugTool.Error(
                $"[CsvValueParser] 셀 인덱스가 범위를 벗어났습니다. 헤더: {headerName}, 인덱스: {columnIndex}, 셀 개수: {cells.Length}",
                DebugType.Data);
            return false;
        }

        cellValue = cells[columnIndex];
        return true;
    }
}