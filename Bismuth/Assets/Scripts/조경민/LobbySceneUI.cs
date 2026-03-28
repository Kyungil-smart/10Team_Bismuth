using TMPro;
using UnityEngine;

public class LobbySceneUI : MonoBehaviour
{
    [Header("━━━━ 텍스트 ━━━━")]
    [Tooltip("쉬움")]
    [SerializeField] private TMP_Text _easyText;
    [Tooltip("보통")]
    [SerializeField] private TMP_Text _normalText;
    [Tooltip("어려움")]
    [SerializeField] private TMP_Text _hardText;
    [Tooltip("전투")]
    [SerializeField] private TMP_Text _combatText;
    [Tooltip("전투시작")]
    [SerializeField] private TMP_Text _startText;
    [Tooltip("어려움")]
    [SerializeField] private TMP_Text _infoText;

    [Header("━━━━ 테두리 ━━━━")]
    [Tooltip("전투 버튼")]
    [SerializeField] private GameObject _combatOutline;
    [Tooltip("쉬움")]
    [SerializeField] private GameObject _easyOutline;
    [Tooltip("보통")]
    [SerializeField] private GameObject _normalOutline;
    [Tooltip("어려움")]
    [SerializeField] private GameObject _hardOutline;

    [Header("━━━━ 패널 ━━━━")]
    [Tooltip("환경 설정 팝업")]
    [SerializeField] private GameObject _settingsPopup;
    [Tooltip("난이도")]
    [SerializeField] private GameObject _difficultyPanel;
    [Tooltip("난이도 정보")]
    [SerializeField] private GameObject _infoPanel;

    [Header("━━━━ 버튼 ━━━━")]
    [SerializeField] private GameObject _startButton;

    private Difficulty _currentDifficulty = Difficulty.None;

    private void Start()
    {
        // TODO : 로컬라이징 수정
        _easyText.text = "EASY";
        _normalText.text = "NORMAL";
        _hardText.text = "HARD";
        _combatText.text = "COMBAT";
        _startText.text = "START";
    }

    public void OnClickSettings()
    {
        _settingsPopup.SetActive(true);
    }

    public void OnClickCombat()
    {
        bool isActive = _difficultyPanel.activeSelf;
        _combatOutline.SetActive(!isActive);
        _difficultyPanel.SetActive(!isActive);
    }

    public void OnClickStart()
    {
        // TODO : 씬인덱스 설정후 주석해제
        // GameSceneManager.Instance.ChangeScene(2);
    }

    public void OnClickEasy()
    {
        SetDifficulty(Difficulty.Easy);
    }

    public void OnClickNormal()
    {
        SetDifficulty(Difficulty.Normal);
    }

    public void OnClickHard()
    {
        SetDifficulty(Difficulty.Hard);
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        if (_currentDifficulty == difficulty)
        {
            _currentDifficulty = Difficulty.None;
        }
        else
        {
            _currentDifficulty = difficulty;
        }
        UpdateDifficultyUI();
    }

    private void UpdateDifficultyUI()
    {
        _easyOutline.SetActive(_currentDifficulty == Difficulty.Easy);
        _normalOutline.SetActive(_currentDifficulty == Difficulty.Normal);
        _hardOutline.SetActive(_currentDifficulty == Difficulty.Hard);

        _infoPanel.SetActive(_currentDifficulty != Difficulty.None);
        _startButton.SetActive(_currentDifficulty != Difficulty.None);
    }
}

public enum Difficulty
{
    None,
    Easy,
    Normal,
    Hard
}
