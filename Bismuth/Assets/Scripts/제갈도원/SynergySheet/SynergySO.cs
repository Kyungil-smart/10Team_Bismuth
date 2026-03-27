using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SynergySO", menuName = "Bismuth/Synergy SO")]
public class SynergySO : ScriptableObject
{
    [SerializeField] private List<SynergyData> _rows = new();

    public IReadOnlyList<SynergyData> Rows => _rows;

    public void SetRows(List<SynergyData> rows)
    {
        _rows = rows;
    }
}