using TMPro;
using UnityEngine;

public class GameoverPopupUI : MonoBehaviour
{
    [Header("━━━━ 텍스트 ━━━━")]
    [Tooltip("Game Over")]
    [SerializeField] private TMP_Text _gameoverText;
    [Tooltip("다시시작")]
    [SerializeField] private TMP_Text _retryText;
    [Tooltip("메인화면")]
    [SerializeField] private TMP_Text _mainText;

    private void Start()
    {
        // TODO : 로컬라이징 수정
        _gameoverText.text = "GAME OVER";
        _retryText.text = "RETRY";
        _mainText.text = "MAIN";
    }

    public void OnClickRetry()
    {
        GameSceneManager.Instance.ReloadScene();
    }

    public void OnClickMain()
    {
        GameSceneManager.Instance.LoadTitle();
    }
}