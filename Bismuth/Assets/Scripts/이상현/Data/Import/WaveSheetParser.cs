using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// 웨이브 시트 CSV 문자열을 읽어서 WaveSheetRow 리스트로 변환하는 파서.
/// 
/// CSV 텍스트를 줄 단위로 분리
/// 헤더 이름 기준으로 각 행을 WaveSheetRow로 변환
/// 필수 컬럼 누락 시 전체 파싱을 중단하고, 선택 컬럼은 기본값으로 대체
/// </summary>
public class WaveSheetParser
{
    private const string HEADER_WAVE_ENTRY_ID = "웨이브 ID";
    private const string HEADER_WAVE_NUMBER = "웨이브수";
    private const string HEADER_MONSTER_ID = "출현 몬스터(ID값)";
    private const string HEADER_COUNT = "수량";
    private const string HEADER_SPAWN_INTERVAL = "소환간격(초)";
    private const string HEADER_START_DELAY = "시작 지연";

    private enum OptionalCellState
    {
        Success,
        HeaderMissing,
        CellMissing,
        InvalidContext
    }

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
        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_WAVE_ENTRY_ID, out int waveEntryId))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_WAVE_NUMBER, out int waveNumber))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_MONSTER_ID, out int monsterId))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_COUNT, out int count))
            return false;

        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_SPAWN_INTERVAL, out float spawnInterval))
            return false;
        
        // 선택 컬럼
        if (!TryReadOptionalFloat(cells, headerMap, HEADER_START_DELAY, 0f, csvLineNumber, out float startDelay))
            return false;
        
        row = new WaveSheetRow
        {
            WaveEntryId = waveEntryId,
            WaveNumber = waveNumber,
            MonsterId = monsterId,
            Count = count,
            SpawnInterval = spawnInterval,
            StartDelay = startDelay
        };
        
        return true;
    }

    private bool TryReadOptionalFloat(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        float defaultValue,
        int csvLineNumber,
        out float value)
    {
        value = defaultValue;

        OptionalCellState state = TryGetOptionalCell(cells, headerMap, headerName, out string cellValue);

        if (state == OptionalCellState.HeaderMissing || state == OptionalCellState.CellMissing)
        {
            value = defaultValue;
            return true;
        }

        if (state == OptionalCellState.InvalidContext)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(cellValue))
        {
            value = defaultValue;
            return true;
        }

        if (!float.TryParse(
                cellValue.Trim(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out value))
        {
            DebugTool.Error(
                $"[WaveSheetParser] 선택 실수 컬럼 변환 실패 - " +
                $"라인: {csvLineNumber}, 헤더: {headerName}, 값: {cellValue}",
                DebugType.Data);
            return false;
        }

        return true;
    }

    private OptionalCellState TryGetOptionalCell(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        out string cellValue)
    {
        cellValue = string.Empty;

        if (cells == null)
        {
            DebugTool.Error("[WaveSheetParser] cells 배열이 null입니다.", DebugType.Data);
            return OptionalCellState.InvalidContext;
        }

        if (headerMap == null)
        {
            DebugTool.Error("[WaveSheetParser] headerMap이 null입니다.", DebugType.Data);
            return OptionalCellState.InvalidContext;
        }

        if (!headerMap.TryGetValue(headerName, out int columnIndex))
        {
            return OptionalCellState.HeaderMissing;
        }

        if (columnIndex < 0 || columnIndex >= cells.Length)
        {
            return OptionalCellState.CellMissing;
        }

        cellValue = cells[columnIndex];
        return OptionalCellState.Success;
    }
}