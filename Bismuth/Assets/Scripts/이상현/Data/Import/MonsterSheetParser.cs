using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// 몬스터 시트 CSV 문자열을 읽어서 MonsterSheetRow 리스트로 변환하는 파서.
/// 
/// CSV 텍스트를 줄 단위로 분리
/// 헤더 이름 기준으로 각 행을 MonsterSheetRow로 변환
/// 컬럼 누락 또는 값 변환 실패 시 전체 파싱을 중단
/// </summary>
public class MonsterSheetParser
{
    // 1.적유닛 ID 2.분류	3.이름 4.기본 체력 5.체력 증가치
    // 6.기본 방어력 7.방어력 증가치 8.기본 이동 속도 9.이동 방식 10.기지 피해
    // 11. 처치 보상
    private const string HEADER_ID = "적유닛 ID";
    private const string HEADER_TYPE = "분류";
    private const string HEADER_NAME = "이름";
    private const string HEADER_BASE_HP = "기본 체력";
    private const string HEADER_HP_GROWTH = "체력 증가치";
    private const string HEADER_BASE_DEFENSE = "기본 방어력";
    private const string HEADER_DEFENSE_GROWTH = "방어력 증가치";
    private const string HEADER_MOVE_SPEED = "기본 이동 속도";
    private const string HEADER_MOVE_TYPE = "이동 방식";
    private const string HEADER_BASE_DAMAGE = "기지 피해";
    private const string HEADER_KILL_REWARD = "처치 보상";

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

        if (!CsvValueParser.TryReadInt(cells, headerMap, HEADER_KILL_REWARD, out int killReward))
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
            MoveType = moveType,
            BaseDamageToBase = baseDamageToBase,
            KillReward = killReward
        };
        
        return true;
    }
}