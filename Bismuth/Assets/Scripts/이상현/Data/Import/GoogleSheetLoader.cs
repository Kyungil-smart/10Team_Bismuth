using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public struct GoogleSheetLoader
{
    [field: Header("구글 시트 설정")]
    [field: Tooltip("구글 시트 CSV URL")]
    [field: SerializeField] public string Url { get; private set; }
    
    // 구글시트 CSV를 다운로드하고, 줄 단위 문자열 배열로 외부에 넘김
    public IEnumerator Load(Action<string[]> onLoaded)
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

        string exportUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={gid}";

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

            onLoaded?.Invoke(lines);
        }
    }
    
    private static bool TryParseSheetInfo(string url, out string sheetId, out string gid)
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