using System.Collections.Generic;
using UnityEngine;

// 스크롤뷰에 들어갈 스크립트
public class SynergyScrollViewUI : MonoBehaviour
{
    [SerializeField] private Transform _synergyContent; // 시너지 스크롤뷰 속 Content
    [SerializeField] private SynergyUI _synergyPrefab;
    [SerializeField] private SynergyManager _synergyManager;

    private List<SynergyUI> _synergys = new List<SynergyUI>(); // 존재하는 시너지 프리팹 리스트

    private void OnEnable()
    {
        _synergyManager.OnSynergyChanged += Refresh;
    }

    private void OnDisable()
    {
        _synergyManager.OnSynergyChanged -= Refresh;
    }

    // 시너지 목록 바뀔때 Refresh 호출
    public void Refresh(Dictionary<int, List<int>> dict)
    {
        ClearSynergys(); // 변화가 있을때마다 리스트 비우고 다시 채움

        // 받아온 딕셔너리에서 시너지프리팹 하나씩 생성
        foreach (KeyValuePair<int, List<int>> pair in dict)
        {
            int synergyId = pair.Key;
            int count = pair.Value.Count;

            SynergyUI synergy = Instantiate(_synergyPrefab, _synergyContent);
            synergy.SetData(synergyId, count);
            _synergys.Add(synergy);
        }
    }

    // _synergys 리스트에 있는 모든 프리팹 삭제 후 리스트 비워줌
    private void ClearSynergys()
    {
        for (int i = 0; i < _synergys.Count; i++)
        {
            if (_synergys[i] != null)
                Destroy(_synergys[i].gameObject);
        }

        _synergys.Clear();
    }
}
