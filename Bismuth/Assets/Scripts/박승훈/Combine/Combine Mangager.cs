using System.Collections.Generic;
using UnityEngine;

public class CombineMangager : MonoBehaviour
{
    [SerializeField] private SynergyManager _synergyManager;
    [SerializeField] private CombineSO _combineSO;
    private List<UnitStat> _unitStats = new();

    private void Awake()
    {
        Init();
    }
    
    public void CombineUnit()
    {
        
        
        
        
        UnitStat stat = new();
        
        _synergyManager.OnUnitCreated?.Invoke(stat);
    }

    private void GetUnitStat()
    {
        
    }

    private void Init()
    {
        _synergyManager = GetComponent<SynergyManager>();
    }
}
