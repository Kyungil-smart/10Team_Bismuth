using UnityEngine;

public class SummonManager : MonoBehaviour
{
    [SerializeField] private SummonUnit summonUnit;
    [SerializeField] private bool SummonLog = true;

    private void Awake()
    {
        if (summonUnit == null)
            summonUnit = GetComponent<SummonUnit>();
    }

    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Summon, SummonLog);
    }

    public void SummonUnit()
    {
        if (summonUnit == null)
        {
            DebugTool.Error("SummonUnit 참조가 없습니다.", DebugType.Summon, this);
            return;
        }

        bool success = summonUnit.TrySummonAndPlace();

        if (!success)
        {
            DebugTool.Warnning("소환에 실패했습니다.", DebugType.Summon, this);
        }
    }
}