using System.Collections.Generic;
using UnityEngine;

public class MonsterDataManager : MonoBehaviour
{
    [Header("구글 시트 로더")]
    [Tooltip("몬스터 시트 CSV를 불러오는 설정입니다.")]
    public GoogleSheetLoader monsterSheet;

    private MonsterSheetParser _monsterSheetParser;

    private void Awake()
    {
        _monsterSheetParser = new MonsterSheetParser();
    }

    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Data, true);

        StartCoroutine(monsterSheet.Load(OnMonsterSheetLoaded));
    }

    private void OnMonsterSheetLoaded(char splitSymbol, string[] lines)
    {
        string csvText = string.Join("\n", lines);

        if (!_monsterSheetParser.TryParse(csvText, out List<MonsterSheetRow> rows))
        {
            DebugTool.Error("[MonsterDataManager] 몬스터 시트 파싱에 실패했습니다.", DebugType.Data, this);
            return;
        }

        DebugTool.Log(
            $"[MonsterDataManager] 몬스터 시트 파싱 성공 - 총 {rows.Count}행",
            DebugType.Data,
            this);

        if (rows.Count == 0)
        {
            DebugTool.Warnning("[MonsterDataManager] 몬스터 데이터가 비어 있습니다.", DebugType.Data, this);
            return;
        }

        MonsterSheetRow firstRow = rows[0];

        DebugTool.Log(
            $"[MonsterDataManager] 첫 행 확인 - ID:{firstRow.Id}, 이름:{firstRow.Name}, 이동속도:{firstRow.MoveSpeed}, 이동방식:{firstRow.MoveType}",
            DebugType.Data,
            this);
    }
}