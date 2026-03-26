using TMPro;
using UnityEngine;

public class SettingsPopupUI : MonoBehaviour
{
    [Header("━━━━ 텍스트 ━━━━")]
    [Tooltip("Control")]
    [SerializeField] private TMP_Text _controlText;
    [Tooltip("Volume")]
    [SerializeField] private TMP_Text _volumeText;
    [Tooltip("Language")]
    [SerializeField] private TMP_Text _languageText;

    [Header("━━━━ 패널 ━━━━")]
    [SerializeField] private GameObject _controlPanel;
    [SerializeField] private GameObject _volumePanel;
    [SerializeField] private GameObject _languagePanel;

    private void Start()
    {
        // TODO : 수정해야됨(로컬라이징)
        _controlText.text = "Control";
        _volumeText.text = "Volume";
        _languageText.text = "Language";
    }

    public void OnClickShowControl()
    {
        ShowPanel(_controlPanel);
    }

    public void OnClickShowVolume()
    {
        ShowPanel(_volumePanel);
    }

    public void OnClickShowLanguage()
    {
        ShowPanel(_languagePanel);
    }

    public void OnClickClosePopup()
    {
        gameObject.SetActive(false);
    }

    private void ShowPanel(GameObject targetPanel)
    {
        _controlPanel.SetActive(false);
        _volumePanel.SetActive(false);
        _languagePanel.SetActive(false);

        targetPanel.SetActive(true);
    }
}
