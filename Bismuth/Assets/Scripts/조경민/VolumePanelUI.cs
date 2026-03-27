using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VolumePanelUI : MonoBehaviour
{
    [Header("━━━━ 텍스트 ━━━━")]
    [SerializeField] private TMP_Text _masterText;
    [SerializeField] private TMP_Text _bgmText;
    [SerializeField] private TMP_Text _sfxText;
    [SerializeField] private TMP_Text _uiText;

    [Header("━━━━ 슬라이더 ━━━━")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _uiSlider;

    [Header("━━━━ 퍼센트 텍스트 ━━━━")]
    [SerializeField] private TMP_Text _masterPercentText;
    [SerializeField] private TMP_Text _bgmPercentText;
    [SerializeField] private TMP_Text _sfxPercentText;
    [SerializeField] private TMP_Text _uiPercentText;

    private void Start()
    {
        InitSlider(_masterSlider, AudioManager.Instance.MasterVolume, OnMasterChanged);
        InitSlider(_bgmSlider, AudioManager.Instance.BgmVolume, OnBgmChanged);
        InitSlider(_sfxSlider, AudioManager.Instance.SfxVolume, OnSfxChanged);
        InitSlider(_uiSlider, AudioManager.Instance.UIVolume, OnUIChanged);

        UpdatePercentText(_masterPercentText, _masterSlider.value);
        UpdatePercentText(_bgmPercentText, _bgmSlider.value);
        UpdatePercentText(_sfxPercentText, _sfxSlider.value);
        UpdatePercentText(_uiPercentText, _uiSlider.value);

        // TODO : 로컬라이징 수정
        _masterText.text = "MASTER VOLUME";
        _bgmText.text = "BGM";
        _sfxText.text = "SFX";
        _uiText.text = "UI";
    }

    private void InitSlider(Slider slider, float value, UnityAction<float> call)
    {
        if (slider == null) return;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = value;
        slider.onValueChanged.AddListener(call);
    }

    private void OnMasterChanged(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
        UpdatePercentText(_masterPercentText, value);
    }

    private void OnBgmChanged(float value)
    {
        AudioManager.Instance.SetBgmVolume(value);
        UpdatePercentText(_bgmPercentText, value);
    }

    private void OnSfxChanged(float value)
    {
        AudioManager.Instance.SetSfxVolume(value);
        UpdatePercentText(_sfxPercentText, value);
    }

    private void OnUIChanged(float value)
    {
        AudioManager.Instance.SetUIVolume(value);
        UpdatePercentText(_uiPercentText, value);
    }
    
    private void UpdatePercentText(TMP_Text text, float value)
    {
        if (text == null) return;
        text.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
}
