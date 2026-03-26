using System.Collections.Generic;

/// <summary>
/// CSV 헤더 한 줄을 읽어서
/// 헤더 이름 -> 열 인덱스 매핑
/// </summary>
public static class CsvHeaderMap
{
    /// <summary>
    /// CSV 헤더 줄을 딕셔너리로 변환
    /// 실파하면 false 반환
    /// </summary>
    public static bool TryBuild(string headerLine, out Dictionary<string, int> headerMap)
    {
        headerMap = new Dictionary<string, int>();

        if (string.IsNullOrWhiteSpace(headerLine))
        {
            DebugTool.Error("[CsvHeaderMap] csv 헤더 줄이 없음]", DebugType.Data);
            return false;
        }
        
        string[] headers = headerLine.Split(',');

        for (int i = 0; i < headers.Length; i++)
        {
            string headerName = headers[i].Trim();

            if (string.IsNullOrWhiteSpace(headerName))
            {
                DebugTool.Error($"[CsvHeaderMap] 비어 있는 헤더가 있습니다. 열 인덱스: {i}", DebugType.Data);
                return false;
            }

            if (headerMap.ContainsKey(headerName))
            {
                DebugTool.Error($"[CsvHeaderMap] 중복된 헤더명이 있습니다 : {headerName}", DebugType.Data);
                return false;
            }
            headerMap.Add(headerName, i);
        }
        return true;
    }

    /// <summary>
    /// 필수 헤더의 인덱스 찾음
    /// 실패하면 false 반환
    /// </summary>
    public static bool TryGetRequiredIndex( Dictionary<string, int> headerMap, string headerName, out int index)
    {
        index = -1;

        if (headerMap == null)
        {
            DebugTool.Error("[CsvHeaderMap] 헤더 맵이 null입니다.", DebugType.Data);
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(headerName))
        {
            DebugTool.Error("[CsvHeaderMap] 찾으려는 헤더 이름이 비어 있습니다.", DebugType.Data);
            return false;
        }
        
        if (!headerMap.TryGetValue(headerName, out index))
        {
            DebugTool.Error($"[CsvHeaderMap] 필수 헤더를 찾을 수 없습니다: {headerName}", DebugType.Data);
            return false;
        }
        
        return true;
    }
        
        
}
