using System;

[Serializable]
public class MonsterSheetRow
{
    // 1.적유닛 ID 2.분류	3.이름 4.기본 체력 5.체력 증가치
    // 6.기본 방어력 7.방어력 증가치 8.기본 이동 속도 9.이동 방식 10.기지 피해
    // 11. 처치 보상
    public int Id;
    public string Type;
    public string Name;
    public int BaseHp;
    public float HpGrowth;

    public int BaseDefense;
    public float DefenseGrowth;
    public float MoveSpeed;
    public string MoveType;
    public int BaseDamageToBase;
    
    public int KillReward;
}
