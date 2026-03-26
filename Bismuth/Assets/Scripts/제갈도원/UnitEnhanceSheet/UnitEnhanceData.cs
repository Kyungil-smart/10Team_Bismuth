using System;
using System.Globalization;
using UnityEngine;

[Serializable]
public class UnitEnhanceData
{
    [SerializeField] private int _unitId;
    [SerializeField] private float _enhanceValue;

    public int UnitId => _unitId;
    public float EnhanceValue => _enhanceValue;

    public static UnitEnhanceData Create(string[] line)
    {
        if (line == null || line.Length < 2)
            return null;

        UnitEnhanceData data = new UnitEnhanceData();

        int.TryParse(line[0].Trim(), out data._unitId);
        float.TryParse(line[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out data._enhanceValue);

        return data;
    }
}