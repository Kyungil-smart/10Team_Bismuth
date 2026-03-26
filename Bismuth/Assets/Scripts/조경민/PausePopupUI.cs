using TMPro;
using UnityEngine;

public class PausePopupUI : MonoBehaviour
{
    [Header("━━━━ 텍스트 ━━━━")]
    [Tooltip("Menu")]
    [SerializeField] private TMP_Text _menuText;
    [Tooltip("Resume")]
    [SerializeField] private TMP_Text _resumeText;
    [Tooltip("Settings")]
    [SerializeField] private TMP_Text _settingsText;
    [Tooltip("Restart")]
    [SerializeField] private TMP_Text _restartText;
    [Tooltip("Main Menu")]
    [SerializeField] private TMP_Text _mainMenuText;

    [Header("━━━━ 패널 ━━━━")]
    [SerializeField] private GameObject _settingsPopup;

    private void Start()
    {
        // TODO : 수정해야됨(로컬라이징)
        _menuText.text = "MENU";
        _resumeText.text = "Resume";
        _settingsText.text = "Settings";
        _restartText.text = "Restart";
        _mainMenuText.text = "Main Menu";
    }

    public void OnClickResume()
    {
        gameObject.SetActive(false);
        TimeScaleController.Instance.SetPausePopup(false);
    }

    public void OnClickSettings()
    {
        _settingsPopup.transform.SetAsLastSibling();
        _settingsPopup.SetActive(true);
        TimeScaleController.Instance.SetSettingsPopup(true);
    }

    public void OnClickRestart()
    {
        GameSceneManager.Instance.ReloadScene();
    }

    public void OnClickMainMenu()
    {
        GameSceneManager.Instance.LoadTitle();
    }
}
