using System.Collections.Generic;

/// <summary>
/// CSV 헤더 한 줄을 읽어서
/// 헤더 이름 -> 열 인덱스 매핑
/// </summary>
public static class CsvHeaderMap
{

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
                DebugTool.Error($"[CsvHeaderMap] 비어 있는 헤더가 있습니다. 열 인덱스: {i}",
                    DebugType.Data);
                return false;
            }

            if (!headerMap.TryAdd(headerName, i))
            {
                DebugTool.Error($"[CsvHeaderMap] 중복된 헤더명이 있습니다." +
                                $"헤더: {headerName}", DebugType.Data);
                return false;
            }
        }

        return true;
    }
}
