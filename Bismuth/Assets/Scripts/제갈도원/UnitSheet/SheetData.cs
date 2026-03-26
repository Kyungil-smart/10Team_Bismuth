using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class SheetData
{
    [field: SerializeField] public string Url { get; private set; }
    [field: SerializeField] public SheetType Type { get; private set; } = SheetType.TSV;

    public char SplitSymbol => Type == SheetType.CSV ? ',' : '\t';


    public IEnumerator Load(Action<char, string[]> onSuccessCall)
    {
        // 1. URL에서 sheetId, gid 추출
        string sheetId = Url.Split(new[] { "/d/" }, System.StringSplitOptions.None).Length > 1
            ? Url.Split(new[] { "/d/" }, System.StringSplitOptions.None)[1].Split('/')[0]
            : Url.Split('/').Length > 5 ? Url.Split('/')[5] : "0";
        string gid = Url.Contains("gid=") ? Url.Split("gid=")[1].Split('&')[0].Split('#')[0] : "0";
        string format = Type == SheetType.CSV ? "csv" : "tsv";

        string exportUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format={format}&gid={gid}";

        using (UnityWebRequest uwr = UnityWebRequest.Get(exportUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                DebugTool.Warnning($"Sheet Load Failed: {uwr.error}", DebugType.Data);
                yield break;
            }

            string sheetDataText = uwr.downloadHandler.text;
            // 줄 단위로 분리 (\n = 행 구분)
            string[] lines = sheetDataText.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

            onSuccessCall?.Invoke(SplitSymbol, lines);
        }
    }
}


public enum SheetType
{
    CSV, TSV
}