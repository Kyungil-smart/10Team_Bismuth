using UnityEngine;

public class SummonChanceTester : MonoBehaviour
{
    [Header("━━━━ 참조 ━━━━")] [SerializeField]
    private SummonChanceDataController _controller;

    [Header("━━━━ 테스트 설정 ━━━━")] [SerializeField]
    private int _testEnhancementLevel = 0;


    /// 현재 강화 단계의 확률 데이터를 출력
    public void PrintCurrentChanceData()
    {
        _controller.PrintChanceData(_testEnhancementLevel);
    }


    // 100회 소환 테스트
    public void TestSummon100Times()
    {
        int tier1Count = 0;
        int tier2Count = 0;
        int tier3Count = 0;
        int tier4Count = 0;

        for (int i = 0; i < 100; i++)
        {
            int resultTier = _controller.GetRandomTier(_testEnhancementLevel);

            switch (resultTier)
            {
                case 1:
                    tier1Count++;
                    break;
                case 2:
                    tier2Count++;
                    break;
                case 3:
                    tier3Count++;
                    break;
                case 4:
                    tier4Count++;
                    break;
            }
        }

        Debug.Log(
            $"[100회 테스트] 강화 단계 {_testEnhancementLevel} -> " +
            $"1등급: {tier1Count}, 2등급: {tier2Count}, 3등급: {tier3Count}, 4등급: {tier4Count}"
        );
    }
}