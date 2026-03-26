using System;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] SynergyManager _synergyManager;
    [SerializeField] UnitStat _unitStat;

    private void Awake()
    {
        
        if(_synergyManager == null)
            _synergyManager = GameObject.Find("GameManager")?.GetComponent<SynergyManager>();
    }

    private void Start()
    {
        _unitStat = GetComponent<UnitStat>();
    }
    
    private void OnEnable()
    {
        // Destroy(gameObject, 2f);
    }

    private void OnDestroy()
    {

        if (_unitStat == null)
        {
            DebugTool.Log("UnitStat 없음", DebugType.Unit, this);
            return;
        }
        
        _synergyManager.OnUnitRemoved?.Invoke(_unitStat);
    }
}
