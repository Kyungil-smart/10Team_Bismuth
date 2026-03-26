using System.Collections.Generic;
using UnityEngine;

public class WaveDataManager : MonoBehaviour
{
    [Header("구글 시트 로더")]
    [Tooltip("웨이브 시트 CSV를 불러오는 설정입니다.")]
    public GoogleSheetLoader waveSheet;

    private WaveSheetParser _waveSheetParser;

    private void Awake()
    {
        _waveSheetParser = new WaveSheetParser();
    }

    private void Start()
    {
        DebugTool.DebugSelect(DebugType.Data, true);

        StartCoroutine(waveSheet.Load(OnWaveSheetLoaded));
    }

    private void OnWaveSheetLoaded(char splitSymbol, string[] lines)
    {
        string csvText = string.Join("\n", lines);

        if (!_waveSheetParser.TryParse(csvText, out List<WaveSheetRow> rows))
        {
            DebugTool.Error("[WaveDataManager] 웨이브 시트 파싱에 실패했습니다.", DebugType.Data, this);
            return;
        }

        DebugTool.Log(
            $"[WaveDataManager] 웨이브 시트 파싱 성공 - 총 {rows.Count}행",
            DebugType.Data,
            this);

        if (rows.Count == 0)
        {
            DebugTool.Warnning("[WaveDataManager] 웨이브 데이터가 비어 있습니다.", DebugType.Data, this);
            return;
        }

        WaveSheetRow firstRow = rows[0];

        DebugTool.Log(
            $"[WaveDataManager] 첫 행 확인 - 엔트리ID:{firstRow.WaveEntryId}, 웨이브:{firstRow.WaveNumber}, 몬스터ID:{firstRow.MonsterId}, 수량:{firstRow.Count}, 소환간격:{firstRow.SpawnInterval}",
            DebugType.Data,
            this);
    }
}