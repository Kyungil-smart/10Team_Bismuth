using System;
using System.Collections.Generic;

/// <summary>
/// 몬스터 시트 CSV 문자열을 읽어서
/// MonsterSheetRow 리스트로 변환하는 파서
///
/// GoolgleSheetLeader는 문자열 가져오기 까지 담당
/// CvsHeaderMap, CsvValueParser를 이용해
/// 헤더 이름 기준으로 값을 읽음
/// 전체 파싱 성공/실패를 bool로 반환
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
    private const string HEADER_MOVE_STEP_COUNT = "기본 이동 칸";
    private const string HEADER_WAIT_TIME = "기본 대기시간";
    private const string HEADER_MOVE_TYPE = "이동 방식";
    private const string HEADER_BASE_DAMAGE = "데미지";
    private const string HEADER_KILL_REWARD = "처치 보상";
    
    // CSV 전체 담당
    public bool TryParse(string csvText, out List<MonsterSheetRow> rows)
    {
        rows = new List<MonsterSheetRow>();

        if (string.IsNullOrWhiteSpace(csvText))
        {
            DebugTool.Error("[MonsterSheetParser] CSV 텍스트가 비어 있습니다.]", DebugType.Data);
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

            if (!TryParseRow(cells, headerMap, out MonsterSheetRow row))
            {
                DebugTool.Error($"[MonsterSheetParser] 몬스터 행 파싱 실패 - CSV 라인 번호: {i + 1}", DebugType.Data);
                return false;
            }

            rows.Add(row);
        }
        
        return true;
    }

    // 한 줄 변환 담당
    private bool TryParseRow(string[] cells, Dictionary<string, int> headerMap, out MonsterSheetRow row)
    {
        row = new MonsterSheetRow();
        
        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_ID, out row.Id))
            return false;
        if (!CsvValueParser.TryReadString(cells, headerMap, HEADER_TYPE, out row.Type))
            return false;

        if (!CsvValueParser.TryReadString(cells, headerMap, HEADER_NAME, out row.Name))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_BASE_HP, out row.BaseHp))
            return false;

        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_HP_GROWTH, out row.HpGrowth))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_BASE_DEFENSE, out row.BaseDefense))
            return false;

        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_DEFENSE_GROWTH, out row.DefenseGrowth))
            return false;

        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_MOVE_SPEED, out row.MoveSpeed))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_MOVE_STEP_COUNT, out row.MoveStepCount))
            return false;

        if (!CsvValueParser.TryReadFloat(cells, headerMap, HEADER_WAIT_TIME, out row.WaitTime))
            return false;

        if (!CsvValueParser.TryReadString(cells, headerMap, HEADER_MOVE_TYPE, out row.MoveType))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_BASE_DAMAGE, out row.BaseDamageToBase))
            return false;

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_KILL_REWARD, out row.KillReward))
            return false;

        return true;
    }
    
}
