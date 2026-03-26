using TMPro;
using UnityEngine;

public class ControlPanelUI : MonoBehaviour
{
    [Header("━━━━ 텍스트 ━━━━")]
    [Tooltip("시간")]
    [SerializeField] private TMP_Text _timeText;
    [Tooltip("재화")]
    [SerializeField] private TMP_Text _goldText;
    [Tooltip("합성소")]
    [SerializeField] private TMP_Text _craftingStationText;
    [Tooltip("시너지")]
    [SerializeField] private TMP_Text _synergyText;
    [Tooltip("일반 뽑기")]
    [SerializeField] private TMP_Text _drawText;
    [Tooltip("확률 +")]
    [SerializeField] private TMP_Text _upgradeText;

    [Header("━━━━ 패널 ━━━━")]
    [Tooltip("합성소")]
    [SerializeField] private GameObject _craftingStation;

    private bool _isCraftingStationOpened => _craftingStation.activeSelf;

    private void Start()
    {
        // TODO : 수정해야됨(로컬라이징)
        _craftingStationText.text = "Crafting Station";
        _drawText.text = "Draw";
        _upgradeText.text = "Upgrade";
    }

    private void Update()
    {
        // TODO : 수정해야됨 -> 시간, 재화정보 받아와야됨 / 로컬라이징(Gold)
        _timeText.text = "mmm : ss";
        _goldText.text = "Gold : 123456";
    }

    public void OnClickCraftingStation()
    {
        _craftingStation.SetActive(!_isCraftingStationOpened);
    }

    // TODO : 뽑기랑 확률증가는 다른곳에서 연결 필요
    public void OnClickDraw()
    {
        // 일반 뽑기
    }

    public void OnClickUpgrade()
    {
        // 확률 +
    }
}
