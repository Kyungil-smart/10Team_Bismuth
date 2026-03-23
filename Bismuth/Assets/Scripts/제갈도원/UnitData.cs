using UnityEngine;

// 유닛 시트 직렬화
// https://docs.google.com/spreadsheets/d/1F8N8D9BqJ3phRCgWw5N1lCx_uYcSqAaO-PgpO3ll7vY

[System.Serializable]
public class UnitData
{
    [Header("━━━━ 기본 정보 ━━━━")] 
    
    [Tooltip("유닛 이름\n예: 인간 전사, 광전사, 웅혜")] [SerializeField]
    private string unitName;
    
    [Tooltip("유닛 고유 ID \n예: 10001 = 인간 전사")] [SerializeField]
    private int id;

    [Tooltip("유닛 등급 단계 (1~4)\n1: 1성, 2: 2성, 3: 3성, 4: 4성(레어)")] [Range(1, 4)] [SerializeField]
    private int tier;

    [Space(8)] [Header("━━━━ 전투 스탯 ━━━━")] 
    
    [Tooltip("기본 공격력")] [Min(0)] [SerializeField]
    private float attackPower;

    [Tooltip("공격 속도 - 초당 공격 횟수\n예: 0.6 = 초당 0.6회, 1.4 = 초당 1.4회")] [Min(0.01f)] [SerializeField]
    private float attackSpeed;

    [Tooltip("치명타 확률 (0~1)\n0.1 = 10%, 0.14 = 14%")] [Range(0f, 1f)] [SerializeField]
    private float criticalChance;

    [Tooltip("공격 사거리 (타일 수)")] [Min(1)] [SerializeField]
    private float attackRange;

    [Space(8)]
    [Header("━━━━ 공격 패턴 ━━━━")]
    [Tooltip("공격 범위 형식 \n예: 1(단일), 1*3(가로), 4*1(세로), 3*3(정사각형)\n'너비 * 높이' 형식")]
    [SerializeField]
    private float attackArea;

    [Tooltip("공격 대상 타입\n 타겟팅 = 1 ~ 3, 광역 = 대상과 대상 주변 모두")] [SerializeField]
    private AttackType attackType;

    [Space(8)]
    [Header("━━━━ 시너지 (조합) ━━━━")]
    [Tooltip("시너지 1 (구글시트 J열)\n예: 인간, 엘프, 오크, 수인, 정령 / 전사, 거너, 마법사 등")]
    [SerializeField]
    private string synergy1;

    [Tooltip("시너지 2")] [SerializeField] private string synergy2;

    [Tooltip("시너지 3\n3성 이상 유닛은 시너지 3개 보유")] [SerializeField]
    private string synergy3;

    [Space(8)]
    [Header("━━━━ 비주얼 ━━━━")]
    [Tooltip("유닛 외형 컨셉\n3성+ 유닛의 비주얼 디자인 가이드")]
    [TextArea(2, 4)]
    [SerializeField]
    private string visualConcept;


    // 프로퍼티 (외부 접근용)
    public int Id => id;
    public int Tier => tier;
    public string UnitName => unitName;
    public float AttackPower => attackPower;
    public float AttackSpeed => attackSpeed;
    public float CriticalChance => criticalChance;
    public float Range => attackRange;
    public float AttackArea => attackArea;
    public AttackType Attack => attackType;
    public string Synergy1 => synergy1;
    public string Synergy2 => synergy2;
    public string Synergy3 => synergy3;
    public string VisualConcept => visualConcept;


    // 공격 대상 타입 (단일/광역/정령)

    public enum AttackType
    {
        Targeting, // 단일
        AOE, // 광역
    }


    // 구글시트 문자열을 AttackTargetType으로 변환
    public static AttackType ParseAttackTarget(string value)
    {
        return value?.Trim() switch
        {
            "단일" => AttackType.Targeting,
            "광역" => AttackType.AOE,
            "2" => AttackType.AOE,
            "3" => AttackType.AOE,
            _ => AttackType.Targeting
        };
    }

    /// <summary>
    /// 구글시트 한 행을 UnitData로 파싱
    /// 컬럼: A=ID, B=단계, C=이름, D=공격력, E=공격속도, F=치명타, G=사거리, H=광역범위, I=공격타겟, J~L=시너지1~3, M=비주얼
    /// </summary>
    public static UnitData CreateFromSheetRow(string[] line)
    {
        if (line == null || line.Length < 9) return null;
        if (string.IsNullOrWhiteSpace(line[0]) || !int.TryParse(line[0].Trim(), out _)) return null;

        var data = new UnitData();
        data.InitFromSheetRow(line);
        return data;
    }

    private void InitFromSheetRow(string[] line)
    {
        int.TryParse(SafeGet(line, 0), out id);
        int.TryParse(SafeGet(line, 1), out tier);
        unitName = SafeGet(line, 2);
        float.TryParse(SafeGet(line, 3), out attackPower);
        attackSpeed = ParseAttackSpeed(SafeGet(line, 4));
        float.TryParse(SafeGet(line, 5).Replace(",", "."), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out criticalChance);
        float.TryParse(SafeGet(line, 6), out attackRange);
        float.TryParse(SafeGet(line, 7), out attackArea);
        attackType = ParseAttackTarget(SafeGet(line, 8));
        synergy1 = SafeGet(line, 9);
        synergy2 = SafeGet(line, 10);
        synergy3 = SafeGet(line, 11);
        visualConcept = SafeGet(line, 12);
    }

    private static string SafeGet(string[] arr, int index)
    {
        return (arr != null && index < arr.Length) ? arr[index]?.Trim() ?? "" : "";
    }

    private static float ParseAttackSpeed(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 1f;
        // "0.6/s", "1,8/s" 형식 처리
        string num = value.Split('/')[0].Replace(",", ".").Trim();
        return float.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float v) ? v : 1f;
    }
}