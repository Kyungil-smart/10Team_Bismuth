using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// 웨이브 시트 CSV 문자열을 읽어서 WaveSheetRow 리스트로 변환하는 파서.
/// 
/// CSV 텍스트를 줄 단위로 분리
/// 헤더 이름 기준으로 각 행을 WaveSheetRow로 변환
/// 컬럼 누락 또는 값 변환 실패 시 전체 파싱을 중단
/// </summary>
public class WaveSheetParser
{
    // 1.웨이브 번호 2.엔트리 순서 3.출현 몬스터 ID 4.수량	5.소환 간격(초)
    // 6.난이도 보정 ID 7.시작 지연(초) 8.웨이브 클리어 보상
    private const string HEADER_WAVE_NUMBER = "웨이브 번호";
    private const string HEADER_ENTRY_ORDER = "엔트리 순서";
    private const string HEADER_MONSTER_ID = "출현 몬스터 ID";
    private const string HEADER_COUNT = "수량";
    private const string HEADER_SPAWN_INTERVAL = "소환 간격(초)";
    private const string HEADER_DIFFICULTY_MODIFIER_ID = "난이도 보정 ID";
    private const string HEADER_START_DELAY = "시작 지연(초)";
    private const string HEADER_CLEAR_REWARD = "웨이브 클리어 보상";
    
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

            if (!TryParseRow(cells, headerMap, i + 1,out WaveSheetRow row))
            {
                DebugTool.Error($"[WaveSheetParser] 웨이브 행 파싱 실패 - CSV 라인 번호: {i + 1}", DebugType.Data);
                return false;
            }

            rows.Add(row);
        }

        return true;
    }

    private bool TryParseRow(
        string[] cells,
        Dictionary<string, int> headerMap,
        int csvLineNumber,
        out WaveSheetRow row)
    {
        row = null;

        // 필수 컬럼
        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_WAVE_NUMBER, out int waveNumber))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_ENTRY_ORDER, out int entryOrder))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_MONSTER_ID, out int monsterId))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_COUNT, out int count))
            return false;

        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_SPAWN_INTERVAL, out float spawnInterval))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_DIFFICULTY_MODIFIER_ID, out int difficultyModifierId))
            return false;
        
        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_CLEAR_REWARD, out int clearReward))
            return false;
        
        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_START_DELAY, out float startDelay))
            return false;
        
        row = new WaveSheetRow
        {
            // 기존 코드와의 점진적 호환용 임시 값
            WaveEntryId = csvLineNumber,

            WaveNumber = waveNumber,
            EntryOrder = entryOrder,
            MonsterId = monsterId,
            Count = count,
            SpawnInterval = spawnInterval,
            DifficultyModifierId = difficultyModifierId,
            StartDelay = startDelay,
            ClearReward = clearReward
        };
        
        return true;
    }
}