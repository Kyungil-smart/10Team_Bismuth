using UnityEngine;

public class CombineDataController : MonoBehaviour
{
    [Header("━━━━ 시트 설정 ━━━━")]
    [Tooltip("구글시트 Unit 시트 URL\n공유 → 링크로 가져오기")]
    [SerializeField] private SheetData combineSheet;

    [Header("━━━━ 합성 DB ━━━━")]
    [Tooltip("시트 데이터가 채워질 UnitSO 에셋\nCreate > Bismuth > Unit Database 로 생성 후 할당")]
    [SerializeField] private CombineSO combineDatabase;
    
    public CombineSO  CombineDatabase => combineDatabase;

    [SerializeField] private bool _log = true;

    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Combine, _log);
        
        if (combineSheet == null || combineDatabase == null)
        {
            DebugTool.Warnning("[CombineSheet] 또는 [CombineDatabase] 가 할당되지 않았습니다.", DebugType.Combine, this);
            return;
        }
        
        DebugTool.Warnning("[CombineSheet] 와  [CombineDatabase]를 성공적으로 불러왔습니다.", DebugType.Combine, this);
        
        StartCoroutine(combineSheet.Load(SetCombineDatas));
    }

    private void SetCombineDatas(char splitSymbol, string[] lines)
    {
        if (lines == null || lines.Length < 2) return;

        combineDatabase.ClearUnits();

        // 0행: 헤더, 1행부터 데이터
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(splitSymbol);
            CombineData data = CombineData.CreateFromSheetRow(cells);
            if (data != null)
            {
                DebugTool.Log($"{data.SourceUnit1}", DebugType.Combine, this);
                combineDatabase.AddUnit(data);
            }
        }
    }
}
