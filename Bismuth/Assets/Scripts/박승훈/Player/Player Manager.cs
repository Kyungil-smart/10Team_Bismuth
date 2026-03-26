using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    [Header("━━━━ 플레이어 스탯 데이터 ━━━━")]
    [Tooltip("플레이어 스탯 SO")]
    [SerializeField] private PlayerSO _playerStat;
    
    [Tooltip("플레이어 레벨(소환 레벨)")]
    [SerializeField] private int _level;

    public int Level
    {
        get => _level; 
        set => _level = value;
    }
    
    [Tooltip("플레이어 소지 골드")]
    [SerializeField] private int _gold;

    public int Gold
    {
        get => _gold; 
        set => _gold = value;
    }
    
    [Tooltip("최대 베이스 체력")]
    [SerializeField] private int _maxBaseHealth;
    public int MaxBaseHealth { get => _maxBaseHealth; set => _maxBaseHealth = value; }
    
    [Tooltip("현재 베이스 체력")]
    [SerializeField] private int _currentBaseHealth;

    public int CurrentBaseHealth
    {
        get => _currentBaseHealth; 
        set => _currentBaseHealth = value;
    }
    
    private Dictionary<int, List<Synergy>> synergies;
    
    [SerializeField] private SynergyManager _synergyManager;

    private void Awake()
    {
        
    }

    private void Start()
    {
        Init();
        PlayerStatInit();
    }

    private void Init()
    {
        
    }

    private void PlayerStatInit()
    {
        Level = _playerStat.Level;
        Gold = _playerStat.Gold;
        MaxBaseHealth = _playerStat.MaxBaseHealth;
        CurrentBaseHealth = MaxBaseHealth;
    }
}
