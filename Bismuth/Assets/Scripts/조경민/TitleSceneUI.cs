using TMPro;
using UnityEngine;

public class TitleSceneUI : MonoBehaviour
{
    [Header("━━━━ 텍스트 ━━━━")]
    [Tooltip("게임 시작")]
    [SerializeField] private TMP_Text _startText;
    [Tooltip("환경설정")]
    [SerializeField] private TMP_Text _settingsText;
    [Tooltip("게임종료")]
    [SerializeField] private TMP_Text _quitText;

    [Header("━━━━ 패널 ━━━━")]
    [Tooltip("환경 설정 팝업")]
    [SerializeField] private GameObject _settingsPopup;

    private void Start()
    {
        // TODO : 로컬라이징 수정
        _startText.text = "START";
        _settingsText.text = "SETTINGS";
        _quitText.text = "QUIT";
    }

    // 게임 시작 버튼 (로비화면으로)
    public void OnClickStart()
    {
        GameSceneManager.Instance.LoadNextStage();
    }

    // 환경 설정 버튼
    public void OnClickSettings()
    {
        _settingsPopup.SetActive(true);
    }

    // 게임 종료 버튼
    public void OnClickQuit()
    {
        GameSceneManager.Instance.GameQuit();
    }
}
