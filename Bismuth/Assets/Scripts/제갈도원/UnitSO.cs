using System.Collections.Generic;
using UnityEngine;

// 유닛 데이터베이스
// Create 메뉴: Assets > Create > Bismuth > Unit Database

[CreateAssetMenu(fileName = "UnitDatabase", menuName = "Bismuth/Unit Database", order = 0)]
public class UnitSO : ScriptableObject
{
    [Header("━━━━ 유닛 시트 데이터 ━━━━")] [Tooltip("구글시트 Unit 시트 전체 데이터\nID순으로 정렬 권장 (10001~)")] 
    public static int MaxTier = 4;
    [SerializeField] private List<UnitData> units = new List<UnitData>();

    // ID로 유닛 검색
    public UnitData GetUnitById(int id)
    {
        foreach (UnitData unit in units)
        {
            if (unit.Id == id) return unit;
        }
        return null;
    }
 
    // 단계(티어)별 유닛 목록 반환
    public List<UnitData> GetUnitsByTier(int tier)
    {
        List<UnitData> result = new List<UnitData>();
        foreach (UnitData unit in units)
        {
            if (unit.Tier == tier) result.Add(unit);
        }
        return result;
    }


    // 시너지 태그로 유닛 검색 
    // ex) 인간 , 전사
    public List<UnitData> GetUnitsBySynergy(string synergyTag)
    {
        List<UnitData> result = new List<UnitData>();
        foreach (UnitData unit in units)
        {
            if (unit.Synergy1 == synergyTag || unit.Synergy2 == synergyTag || unit.Synergy3 == synergyTag)
                result.Add(unit);
        }
        return result;
    }

    /// <summary>
    /// 시트 로드 시 기존 데이터 비우고 새로 채우기
    /// </summary>
    public void ClearUnits()
    {
        units.Clear();
    }

    /// <summary>
    /// 파싱된 유닛 데이터 추가
    /// </summary>
    public void AddUnit(UnitData unit)
    {
        if (unit != null)
            units.Add(unit);
    }
}
