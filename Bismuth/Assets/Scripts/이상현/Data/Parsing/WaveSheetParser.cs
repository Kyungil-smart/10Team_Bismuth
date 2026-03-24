using System;
using System.Collections.Generic;

/// <summary>
/// 웨이브 시트 CSV 문자열을 읽어서
/// WaveSheetRow 리스트로 변환하는 파서
///
/// GoolgleSheetLeader는 문자열 가져오기 까지 담당
/// CvsHeaderMap, CsvValueParser를 이용해
/// 헤더 이름 기준으로 값을 읽음
/// 전체 파싱 성공/실패를 bool로 반환
/// </summary>
public class WaveSheetParser
{
    private const string HEADER_WAVE_ENTRY_ID = "웨이브 ID";
    private const string HEADER_WAVE_NUMBER = "웨이브수";
    private const string HEADER_MONSTER_ID = "출현 몬스터(ID값)";
    private const string HEADER_COUNT = "수량";
    private const string HEADER_SPAWN_INTERVAL = "소환간격(초)";
    private const string HEADER_START_DELAY = "시작 지연";

    // CSV 전체 담당
    public bool TryParse(string csvText, out List<WaveSheetRow> rows)
    {
        rows = new List<WaveSheetRow>();
        
        if (string.IsNullOrWhiteSpace(csvText))
        {
            DebugTool.Error("[WaveSheetParser] CSV 텍스트가 비어 있습니다.", DebugType.Data);
            return false;
        }

        string[] lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
        {
            DebugTool.Error("[WaveSheetParser] 헤더 또는 데이터 행이 부족합니다.", DebugType.Data);
            return false;
        }

        if (!CsvHeaderMap.TryBuild(lines[0], out Dictionary<string, int> headerMap))
        {
            DebugTool.Error("[WaveSheetParser] 헤더 맵 생성에 실패했습니다.", DebugType.Data);
            return false;
        }
        
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] cells = line.Split(',');

            if (!TryParseRow(cells, headerMap, out WaveSheetRow row))
            {
                DebugTool.Error($"[WaveSheetParser] 웨이브 행 파싱 실패 - CSV 라인 번호: {i + 1}", DebugType.Data);
                return false;
            }

            rows.Add(row);
        }

        return true;
    }

    private bool TryParseRow(string[] cells, Dictionary<string, int> headerMap, out WaveSheetRow row)
    {
        row = new WaveSheetRow();
        
        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_WAVE_ENTRY_ID, out row.WaveEntryId))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_WAVE_NUMBER, out row.WaveNumber))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_MONSTER_ID, out row.MonsterId))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_COUNT, out row.Count))
            return false;

        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_SPAWN_INTERVAL, out row.SpawnInterval))
            return false;

        row.StartDelay = 0f;
        if (CsvHeaderMap.TryGetRequiredIndex(headerMap, HEADER_START_DELAY, out int _))
        {
            if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_START_DELAY, out row.StartDelay))
                return false;
        }
        
        return true;
    }
    
}
