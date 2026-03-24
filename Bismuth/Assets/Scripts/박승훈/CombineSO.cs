using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombineDatabase", menuName = "Bismuth/Combine Database", order = 1)]
public class CombineSO : ScriptableObject
{
    [Header("━━━━ 합성 시트 데이터 ━━━━")] [Tooltip("구글시트 합성 시트 전체 데이터")] 
    [SerializeField] private List<CombineData> datas = new List<CombineData>();

    public List<CombineData> Data => datas;
    
    public void ClearUnits()
    {
        datas.Clear();
    }

    /// <summary>
    /// 파싱된 유닛 데이터 추가
    /// </summary>
    public void AddUnit(CombineData data)
    {
        if (data != null)
            datas.Add(data);
    }
}
