using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Data/Monster/Monster Data")]
public class MonsterDataSO : ScriptableObject
{
    [Header("====기본 정보====")]
    [Tooltip("몬스터 고유 ID")]
    [SerializeField] private int _id;
    
    [Tooltip("몬스터 이름")]
    [SerializeField] private string _displayName;
    
    [Tooltip("몬스터 종류\n일반, 보스")]
    [SerializeField] private MonsterCategory _category = MonsterCategory.Normal;
    
    [Space(5)]
    [Header("====프리팹 연결====")]
    [Tooltip("해당 몬스터의 프리팹 연결")]
    [SerializeField] private GameObject _prefab;

    [Space(5)] 
    [Header("====기본 능력치====")]
    [Tooltip("기본 체력")]
    [SerializeField, Min(1)] private int _baseHp = 1;
    
    [Tooltip("웨이브 증가에 따라 더해질 체력 증가치")]
    [SerializeField, Min(0)] private float _hpGrowth = 0f;
    
    [Tooltip("기본 방어력")]
    [SerializeField, Min(0)] private int _baseDefense = 0;
    
    [Tooltip("웨이브 증가에 따라 더해질 방어력 증가치")]
    [SerializeField, Min(0)] private float _defenseGrowth = 0f;

    [Space(5)]
    [Header("====이동 설정====")]
    [Tooltip("기본 이동 속도")]
    [SerializeField, Min(0)] private float _moveSpeed = 0f;
    
    [Tooltip("이동 전 대기 시간\n대기 후 순간이동 타입에서 사용")]
    [SerializeField, Min(0)] private float _waitTime = 0f;
    
    [Tooltip("몬스터의 이동 방식")]
    [SerializeField] private MonsterMoveType _moveType = MonsterMoveType.Normal; 
    
    [Space(5)]
    [Header("====전투 및 보상====")]
    [Tooltip("기지 도달 시 가하는 데미지")]
    [SerializeField, Min(0)] private int _baseDamageToBase = 0;
    
    [Tooltip("처치 시 지급하는 보상 값")]
    [SerializeField, Min(0)] private int _killReward = 0;
    
    
    public int Id => _id;
    public string DisplayName => _displayName;
    public MonsterCategory Category => _category;
    public GameObject Prefab => _prefab;
    public int BaseHp => _baseHp;
    public float HpGrowth => _hpGrowth;
    public int BaseDefense => _baseDefense;
    public float DefenseGrowth => _defenseGrowth;
    public float MoveSpeed => _moveSpeed;
    public float WaitTime => _waitTime;
    public MonsterMoveType MoveType => _moveType;
    public int BaseDamageToBase => _baseDamageToBase;
    public int KillReward => _killReward;

    /// <summary>
    /// Tools 계층에서 변환한 몬스터 데이터를 현재 SO에 덮어씀
    /// 시트가 소유하는 값만 갱신
    /// </summary>
    public void OverwriteData(MonsterDataValues values)
    {
        _id = values.Id;
        _displayName = values.DisplayName;
        _category = values.Category;
        _baseHp = values.BaseHp;
        _hpGrowth = values.HpGrowth;
        _baseDefense = values.BaseDefense;
        _defenseGrowth = values.DefenseGrowth;
        _moveSpeed = values.MoveSpeed;
        _waitTime = values.WaitTime;
        _moveType = values.MoveType;
        _baseDamageToBase = values.BaseDamageToBase;
        _killReward = values.KillReward;
    }
}

public struct MonsterDataValues
{
    public int Id;
    public string DisplayName;
    public MonsterCategory Category;
    
    public int BaseHp;
    public float HpGrowth;
    
    public int BaseDefense;
    public float DefenseGrowth;
    
    public float MoveSpeed;
    public float WaitTime;
    
    public MonsterMoveType MoveType;
    
    public int BaseDamageToBase;
    public int KillReward;
}

public enum MonsterCategory
{
    Normal,
    Boss
}

public enum MonsterMoveType
{
    Normal,
    WaitAndTeleport
}