using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameViewPanelUI : MonoBehaviour
{
    [Header("━━━━ 텍스트 ━━━━")]
    [Tooltip("웨이브(Wave 0 (00/00)")]
    [SerializeField] private TMP_Text _WaveText;

    [Header("━━━━ 버튼(이미지) ━━━━")]
    [Tooltip("배속 버튼")]
    [SerializeField] private Image _fastButtonImage;
    [Tooltip("일시정지 버튼")]
    [SerializeField] private Image _PauseButtonImage;

    [Header("━━━━ 패널 ━━━━")]
    [Tooltip("환경설정 팝업")]
    [SerializeField] private GameObject _settingsPopup;
    [Tooltip("일시정지 패널")]
    [SerializeField] private GameObject _pausePanel;

    [Header("━━━━ 설정 ━━━━")]
    [Tooltip("배속할 속도")]
    [SerializeField] private float _fastSpeed = 2f;
    [Tooltip("어두운 정도")]
    [SerializeField][Range(0f, 1f)] private float _darkness;
    [Tooltip("일시정지 이미지")]
    [SerializeField] private Sprite _pauseSprite;
    [Tooltip("재생 이미지")]
    [SerializeField] private Sprite _playSprite;

    private bool _isPausePanelOpened => _pausePanel.activeSelf;
    private bool _isFast;
    private Color _originalColor;

    private void Awake()
    {
        _originalColor = _fastButtonImage.color;
    }

    private void Start()
    {
        // TODO : 수정해야됨(현재 웨이브정보 받아오기, 로컬라이징)
        _WaveText.text = "Wave 00 (00/00)";
    }

    // 환경설정 버튼
    public void OnClickSettings()
    {
        _settingsPopup.SetActive(true);
        TimeScaleController.Instance.SetSettingsPopup(true);
    }

    // 배속 버튼
    public void OnClickFast()
    {
        _isFast = !_isFast;
        UpdateFastButton();
        TimeScaleController.Instance.ToggleSpeed(_fastSpeed);
    }

    // 일시정지 버튼
    public void OnClickPause()
    {
        _pausePanel.SetActive(!_isPausePanelOpened);
        UpdatePauseButton();
        TimeScaleController.Instance.SetPausePanel(!_isPausePanelOpened);
    }

    // 배속 버튼 밝기 조정
    private void UpdateFastButton()
    {
        if (_isFast)
        {
            Color c = _originalColor;
            c.r *= _darkness;
            c.g *= _darkness;
            c.b *= _darkness;
            _fastButtonImage.color = c;
        }
        else
        {
            _fastButtonImage.color = _originalColor;
        }
    }

    // 일시정지 버튼 이미지 조정
    private void UpdatePauseButton()
    {
        _PauseButtonImage.sprite = _isPausePanelOpened ? _playSprite : _pauseSprite;
    }
}
