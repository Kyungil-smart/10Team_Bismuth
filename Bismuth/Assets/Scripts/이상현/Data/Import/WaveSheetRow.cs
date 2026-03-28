using System;

[Serializable]
public class WaveSheetRow
{
    // 1.웨이브 번호 2.엔트리 순서 3.출현 몬스터 ID 4.수량	5.소환 간격(초)
    // 6.난이도 보정 ID 7.시작 지연(초) 8.웨이브 클리어 보상
    public int WaveNumber;
    public int EntryOrder;
    public int MonsterId;
    public int Count;
    public float SpawnInterval;
    public int DifficultyModifierId;
    public float StartDelay;
    public int ClearReward;
    
    public int WaveEntryId;
}
