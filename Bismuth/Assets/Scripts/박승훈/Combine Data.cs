using UnityEngine;

[System.Serializable]
public class CombineData
{
    [Header("━━━━ 조합 정보 ━━━━")]
    [Tooltip("유닛 티어")] [SerializeField] private int tier;
    [Tooltip("조합 대상")] [SerializeField] private string resultUnit;
    [Tooltip("재료 유닛 1")] [SerializeField] private string sourceUnit1;
    [Tooltip("필요 개수 2")] [SerializeField] private int sourceUnit1Count;
    [Tooltip("재료 유닛 2")] [SerializeField] private string sourceUnit2;
    [Tooltip("필요 개수 2")] [SerializeField] private int sourceUnit2Count;
    [Tooltip("재료 유닛 2")] [SerializeField] private string sourceUnit3;
    [Tooltip("필요 개수 3")] [SerializeField] private int sourceUnit3Count;
    
    public int Tier => tier;
    public string ResultUnit => resultUnit;
    public string SourceUnit1 => sourceUnit1;
    public int SourceUnit1Count => sourceUnit1Count;
    public string SourceUnit2 => sourceUnit2;
    public int SourceUnit2Count => sourceUnit2Count;
    public string SourceUnit3 => sourceUnit3;
    public int SourceUnit3Count => sourceUnit3Count;
    
    public static CombineData CreateFromSheetRow(string[] line)
    {
        if (line == null || line.Length < 8) return null;
        if (string.IsNullOrWhiteSpace(line[0]) || !int.TryParse(line[0].Trim(), out _)) return null;

        var data = new CombineData();
        data.InitFromSheetRow(line);
        
        return data;
    }
    
    private void InitFromSheetRow(string[] line)
    {
        int.TryParse(SafeGet(line, 0), out tier);
        resultUnit = SafeGet(line, 1);
        sourceUnit1 = SafeGet(line, 2);
        int.TryParse(SafeGet(line, 3), out sourceUnit1Count);
        sourceUnit2 = SafeGet(line, 4);
        int.TryParse(SafeGet(line, 5), out sourceUnit2Count);
        sourceUnit3 = SafeGet(line, 6);
        int.TryParse(SafeGet(line, 7), out sourceUnit3Count);
    }

    private static string SafeGet(string[] arr, int index)
        => (arr != null && index < arr.Length) ? arr[index]?.Trim() ?? "" : "";
}
