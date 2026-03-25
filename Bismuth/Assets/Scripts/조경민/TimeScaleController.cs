using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    public static TimeScaleController Instance { get; private set; }

    private float _currentSpeed = 1f;

    private bool _isPausePopupOpened;
    private bool _isSettingsPopupOpened;
    private bool _isPausePanelOpened;
    public bool IsPaused => _isPausePopupOpened || _isSettingsPopupOpened || _isPausePanelOpened;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    // Time.timeScale 조정
    public void SetTimeScale()
    {
        if (IsPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = _currentSpeed;
    }

    // Esc 팝업
    public void SetPausePopup(bool isOpen)
    {
        _isPausePopupOpened = isOpen;
        SetTimeScale();
    }

    // 환경설정 팝업
    public void SetSettingsPopup(bool isOpen)
    {
        _isSettingsPopupOpened = isOpen;
        SetTimeScale();
    }

    // 일시정지 패널
    public void SetPausePanel(bool isOpen)
    {
        _isPausePanelOpened = isOpen;
        SetTimeScale();
    }

    // 속도 조절(배속 버튼)
    public void ToggleSpeed(float fastSpeed)
    {
        _currentSpeed = _currentSpeed == 1f ? fastSpeed : 1f;
        SetTimeScale();
    }
}
