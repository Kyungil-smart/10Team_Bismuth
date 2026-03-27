using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameclearPopupUI : MonoBehaviour
{
    [Header("━━━━ 텍스트 ━━━━")]
    [Tooltip("총 데미지")]
    [SerializeField] private TMP_Text _totalDamageText;
    [Tooltip("데미지( ex)192M )")]
    [SerializeField] private TMP_Text _damageText;
    [Tooltip("클리어")]
    [SerializeField] private TMP_Text _clearText;
    [Tooltip("스테이지 이름")]
    [SerializeField] private TMP_Text _stageText;
    [Tooltip("다시시작")]
    [SerializeField] private TMP_Text _retryText;
    [Tooltip("메인화면")]
    [SerializeField] private TMP_Text _mainText;

    [Header("━━━━ 이미지 ━━━━")]
    [Tooltip("MVP 이미지")]
    [SerializeField] private Image _mvpImage;

    private void Start()
    {
        // TODO : 로컬라이징 수정
        _totalDamageText.text = "Total Damage";
        _clearText.text = "CLEAR";
        _retryText.text = "RETRY";
        _mainText.text = "MAIN";
    }

    // TODO : 게임 진행 데이터 받아와서 스크롤뷰에 나올수 있게 해야됨


    public void OnClickRetry()
    {
        GameSceneManager.Instance.ReloadScene();
    }

    public void OnClickMain()
    {
        GameSceneManager.Instance.LoadTitle();
    }
}
