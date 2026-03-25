using System;

[Serializable]
public class MonsterSheetRow
{
    public int Id;
    public string Type;
    public string Name;

    public int BaseHp;
    public float HpGrowth;

    public int BaseDefense;
    public float DefenseGrowth;
    
    public float MoveSpeed;
    public int MoveStepCount; // 기본 이동 칸 : 미정
    public float WaitTime;

    public string MoveType;

    public int BaseDamageToBase;
    public int KillReward;  // 아직 없음
}
