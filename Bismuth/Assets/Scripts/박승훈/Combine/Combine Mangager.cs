using UnityEngine;

public class CombineMangager : MonoBehaviour
{
    [SerializeField] private SynergyManager _synergyManager;

    private void Awake()
    {
        Init();
    }
    
    public void CombineUnit()
    {
        // _synergyManager.OnUnitSummoned?.Invoke();
    }

    private void Init()
    {
        _synergyManager = GetComponent<SynergyManager>();
    }
}
