using TMPro;
using UnityEngine;

public class LanguagePanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _languageText;

    [SerializeField] private TMP_Dropdown _languageDropdown;

    private void Awake()
    {
        Language currentLanguage = LocalizationManager.Instance.CurrentLanguage;
        _languageDropdown.value = (int)currentLanguage;
        _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    private void Start()
    {
        // TODO : 로컬라이징
        _languageText.text = "LANGUAGE";
    }

    private void OnLanguageChanged(int index)
    {
        LocalizationManager.Instance.ChangeLanguage((Language)index);
    }
}