using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyModifierTableSO", menuName = "Data/Difficulty/Difficulty Modifier Table")]
public class DifficultyModifierTableSO : ScriptableObject
{
    [Header("====난이도 보정 목록====")]
    [Tooltip("난이도 보정 테이블은 ID 기준으로 보관")]
    [SerializeField] private List<DifficultyModifierEntry> _entries = new();
    
    public IReadOnlyList<DifficultyModifierEntry> Entries => _entries;
    
    // 난이도 보정 ID로 항목 조회
    public bool TryGetById(int id, out DifficultyModifierEntry entry)
    {
        foreach (DifficultyModifierEntry current in _entries)
        {
            if (current == null) continue;
            if (current.Id != id) continue;
            
            entry = current;
            return true;
        }

        entry = null;
        return false;
    }
}

[Serializable]
public class DifficultyModifierEntry
{
    // 1.난이도 보정 ID 2.난이도명 3.자원 수급 계수 4.적 기지 피해 계수 5.적 체력 계수
    // 6.아군 기지 체력 계수
    [Header("====기본 정보====")]
    [Tooltip("난이도 보정 ID")]
    [SerializeField, Min(0)] private int _id;

    [Tooltip("난이도 명을 입력해 주세요\n예)Easy, Normal, Hard")]
    [SerializeField] private string _difficultyName;
    
    [Space(5)]
    [Header("====보정 수치====")]
    [Tooltip("자원 수급 계수를 입력해 주세요")]
    [SerializeField, Min(0.1f)] private float _resourceGainMultiplier = 1f;
    
    [Tooltip("적 기지 피해 계수를 입력해 주세요")]
    [SerializeField, Min(0.1f)] private float _enemyToBaseDamageMultiplier = 1f;
    
    [Tooltip("적 체력 계수를 입력해 주세요")]
    [SerializeField, Min(0.1f)] private float _enemyHpMultiplier = 1f;
    
    [Tooltip("아군 기지 체력 계수를 입력해 주세요")]
    [SerializeField, Min(0.1f)] private float _teamBaseHpMultiplier = 1f;
    
    public int Id => _id;
    public string DifficultyName => _difficultyName;
    public float ResourceGainMultiplier => _resourceGainMultiplier;
    public float EnemyToBaseDamageMultiplier => _enemyToBaseDamageMultiplier;
    public float EnemyHpMultiplier => _enemyHpMultiplier;
    public float TeamBaseHpMultiplier => _teamBaseHpMultiplier;
}