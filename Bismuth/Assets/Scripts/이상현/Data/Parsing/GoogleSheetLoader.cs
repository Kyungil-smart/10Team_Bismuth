using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public struct GoogleSheetLoader
{
    [field: Header("구글 시트 설정")]
    [field: Tooltip("구글 시트 URL")]
    [field: SerializeField] public string Url { get; private set; }

    [field: Tooltip("가져올 시트 데이터 포맷")]
    [field: SerializeField] public SheetType1 Type { get; private set; }

    public char SplitSymbol => Type == SheetType1.CSV ? ',' : '\t';

    
    public IEnumerator Load()
    {
        yield return Load(null);
    }
    
    public IEnumerator Load(Action<char, string[]> successCallback)
    {
        if (string.IsNullOrWhiteSpace(Url))
        {
            DebugTool.Error("[GoogleSheetLoader] URL이 비어 있습니다.", DebugType.Data);
            yield break;
        }

        if (!TryParseSheetInfo(Url, out string sheetId, out string gid))
        {
            DebugTool.Error("[GoogleSheetLoader] 구글 시트 URL 형식이 올바르지 않습니다.", DebugType.Data);
            yield break;
        }

        string format = Type == SheetType1.CSV ? "csv" : "tsv";
        string exportUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format={format}&gid={gid}";

        using (UnityWebRequest uwr = UnityWebRequest.Get(exportUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                DebugTool.Error($"[GoogleSheetLoader] 구글시트 연결 실패 : {uwr.error}", DebugType.Data);
                yield break;
            }

            string sheetDataText = uwr.downloadHandler.text;
            string[] lines = sheetDataText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            DebugTool.Log(
                $"[GoogleSheetLoader] 시트 로드 성공 - 형식:{format}, 라인 수:{lines.Length}",
                DebugType.Data);

            successCallback?.Invoke(SplitSymbol, lines);
        }
    }

    private bool TryParseSheetInfo(string url, out string sheetId, out string gid)
    {
        sheetId = string.Empty;
        gid = string.Empty;

        if (!url.Contains("/d/"))
        {
            DebugTool.Error("[GoogleSheetLoader] URL에 '/d/' 구간이 없습니다.", DebugType.Data);
            return false;
        }

        string[] dSplit = url.Split("/d/");
        if (dSplit.Length < 2)
        {
            DebugTool.Error("[GoogleSheetLoader] sheetId를 추출하지 못했습니다.", DebugType.Data);
            return false;
        }

        string[] idSplit = dSplit[1].Split('/');
        if (idSplit.Length < 1 || string.IsNullOrWhiteSpace(idSplit[0]))
        {
            DebugTool.Error("[GoogleSheetLoader] sheetId가 비어 있습니다.", DebugType.Data);
            return false;
        }

        sheetId = idSplit[0];

        if (!url.Contains("gid="))
        {
            DebugTool.Error("[GoogleSheetLoader] URL에 'gid=' 구간이 없습니다.", DebugType.Data);
            return false;
        }

        string[] gidSplit = url.Split("gid=");
        if (gidSplit.Length < 2)
        {
            DebugTool.Error("[GoogleSheetLoader] gid를 추출하지 못했습니다.", DebugType.Data);
            return false;
        }

        gid = gidSplit[1].Split('&')[0].Split('#')[0];

        if (string.IsNullOrWhiteSpace(gid))
        {
            DebugTool.Error("[GoogleSheetLoader] gid가 비어 있습니다.", DebugType.Data);
            return false;
        }

        return true;
    }
}

public enum SheetType1
{
    CSV,
    TSV
}