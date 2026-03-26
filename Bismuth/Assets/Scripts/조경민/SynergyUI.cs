using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 시너지 프리팹에 들어갈 스크립트
public class SynergyUI : MonoBehaviour
{
    [Header("━━━━ 헤더 ━━━━")]
    [SerializeField] private Image _icon; // 시너지 이름 앞 네모칸
    [SerializeField] private Sprite activeSprite; // 네모칸에 들어갈 이미지(활성화)
    [SerializeField] private Sprite inactiveSprite; // 네모칸에 들어갈 이미지(비활성화)
    [SerializeField] private TMP_Text _hNameText; // 헤더에 들어갈 시너지 이름 텍스트
    [SerializeField] private TMP_Text _countText; // 시너지 수 (현재 시너지 수/가능한 최대 시너지 수)

    [Header("━━━━ 설명 ━━━━")]
    [SerializeField] private TMP_Text _dNameText; // 설명칸에 들어갈 시너지 이름 텍스트
    [SerializeField] private TMP_Text _descriptionText; // 시너지 설명

    public void SetData(int synergyId, int count) // 매개변수로 데이터받기
    {
        SynergyManager.SynergyType type = (SynergyManager.SynergyType)synergyId;

        string name = type.ToString();
        string currentCount = count.ToString();
        _hNameText.text = name;
        _countText.text = $"{currentCount} / 0"; // TODO: 최대는 임시로 0으로 설정, 각 시너지에 맞게 수정
    }
}
/*
public void SetData() // 매개변수로 데이터받기
{
    //_icon.sprite = 데이터.isActive ? activeSprite : inactiveSprite;
    //_nameText.text = $"{데이터.name}";
    //_countText.text = $"{데이터.currentCount}/{데이터.MaxCount}";

    //_dNameText.text = $"{데이터.name}";
    //_descriptionText.text = $"{데이터.description}";
}
*/