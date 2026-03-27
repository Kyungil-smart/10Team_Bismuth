using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// 몬스터 시트 CSV 문자열을 읽어서 MonsterSheetRow 리스트로 변환하는 파서.
/// 
/// CSV 텍스트를 줄 단위로 분리
/// 헤더 이름 기준으로 각 행을 MonsterSheetRow로 변환 
/// 필수 컬럼 누락 시 전체 파싱을 중단하고, 선택 컬럼은 기본값으로 대체
/// </summary>
public class MonsterSheetParser
{
    private const string HEADER_ID = "ID";
    private const string HEADER_TYPE = "종류";
    private const string HEADER_NAME = "이름";
    private const string HEADER_BASE_HP = "기본 체력";
    private const string HEADER_HP_GROWTH = "체력 증가치";
    private const string HEADER_BASE_DEFENSE = "기본 방어력";
    private const string HEADER_DEFENSE_GROWTH = "방어력 증가치";
    private const string HEADER_MOVE_SPEED = "기본 이동 속도";
    //private const string HEADER_MOVE_STEP_COUNT = "기본 이동 칸";  // 미정
    private const string HEADER_WAIT_TIME = "기본 대기시간";
    private const string HEADER_MOVE_TYPE = "이동 방식";
    private const string HEADER_BASE_DAMAGE = "데미지";
    private const string HEADER_KILL_REWARD = "처치 보상";

    private enum OptionalCellState
    {
        Success,
        HeaderMissing,
        CellMissing,
        InvalidContext
    }

    public bool TryParse(string csvText, out List<MonsterSheetRow> rows)
    {
        rows = new List<MonsterSheetRow>();

        if (string.IsNullOrWhiteSpace(csvText))
        {
            DebugTool.Error("[MonsterSheetParser] CSV 텍스트가 비어 있습니다.", DebugType.Data);
            return false;
        }

        string[] lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
        {
            DebugTool.Error("[MonsterSheetParser] 헤더 또는 데이터 행이 부족합니다.", DebugType.Data);
            return false;
        }

        if (!CsvHeaderMap.TryBuild(lines[0], out Dictionary<string, int> headerMap))
        {
            DebugTool.Error("[MonsterSheetParser] 헤더 맵 생성에 실패했습니다.", DebugType.Data);
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

            if (!TryParseRow(cells, headerMap, i + 1, out MonsterSheetRow row))
            {
                DebugTool.Error($"[MonsterSheetParser] 몬스터 행 파싱 실패 - CSV 라인 번호: {i + 1}", DebugType.Data);
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
        out MonsterSheetRow row)
    {
        row = null;

        // 필수 컬럼
        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_ID, out int id))
            return false;
        
        if (!CsvValueParser.TryReadString(cells, headerMap, HEADER_TYPE, out string type))
            return false;

        if (!CsvValueParser.TryReadString(cells, headerMap, HEADER_NAME, out string name))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_BASE_HP, out int baseHp))
            return false;

        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_HP_GROWTH, out float hpGrowth))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_BASE_DEFENSE, out int baseDefense))
            return false;

        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_DEFENSE_GROWTH, out float defenseGrowth))
            return false;

        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_MOVE_SPEED, out float moveSpeed))
            return false;

        if (!CsvValueParser.TryReadString(cells, headerMap, HEADER_MOVE_TYPE, out string moveType))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_BASE_DAMAGE, out int baseDamageToBase))
            return false;
        
        // 선택 컬럼
        //if (!TryReadOptionalInt(cells, headerMap, HEADER_MOVE_STEP_COUNT, 1, csvLineNumber, out int moveStepCount))
        //    return false;

        if (!TryReadOptionalFloat(cells, headerMap, HEADER_WAIT_TIME, 0f, csvLineNumber, out float waitTime))
            return false;

        if (!TryReadOptionalInt(cells, headerMap, HEADER_KILL_REWARD, 0, csvLineNumber, out int killReward))
            return false;
        
        row = new MonsterSheetRow
        {
            Id = id,
            Type = type,
            Name = name,
            BaseHp = baseHp,
            HpGrowth = hpGrowth,
            BaseDefense = baseDefense,
            DefenseGrowth = defenseGrowth,
            MoveSpeed = moveSpeed,
            //MoveStepCount = moveStepCount,
            WaitTime = waitTime,
            MoveType = moveType,
            BaseDamageToBase = baseDamageToBase,
            KillReward = killReward
        };
        
        return true;
    }

    private bool TryReadOptionalInt(
        string[] cells,
        Dictionary<string, int> headerMap,
        string headerName,
        int defaultValue,
        int csvLineNumber,
        out int value)
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

        if (!int.TryParse(cellValue.Trim(), out value))
        {
            DebugTool.Error(
                $"[MonsterSheetParser] 선택 정수 컬럼 변환 실패 - " +
                $"라인: {csvLineNumber}, 헤더: {headerName}, 값: {cellValue}",
                DebugType.Data);
            return false;
        }

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
                $"[MonsterSheetParser] 선택 실수 컬럼 변환 실패 - " +
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
            DebugTool.Error("[MonsterSheetParser] cells 배열이 null입니다.", DebugType.Data);
            return OptionalCellState.InvalidContext;
        }

        if (headerMap == null)
        {
            DebugTool.Error("[MonsterSheetParser] headerMap이 null입니다.", DebugType.Data);
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