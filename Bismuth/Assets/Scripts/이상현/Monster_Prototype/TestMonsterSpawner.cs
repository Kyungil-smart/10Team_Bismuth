using UnityEngine;

public class TestMonsterSpawner : MonoBehaviour
{
    [Header("스폰 설정")] 
    [Tooltip("몬스터 프리팹")]
    [SerializeField] private MonsterController _monsterPrefab;

    [Tooltip("몬스터가 따라갈 웨이포인트 경로")]
    [SerializeField] private WaypointPath _waypointPath;

    [SerializeField] private float _moveSpeed = 2f;
    
    [SerializeField] private bool _spawnOnStart = true;

    private void Start()
    {
        if (_spawnOnStart) SpawnMonster();
    }

    [ContextMenu("몬스터 생성")]
    public void SpawnMonster()
    {
        if (_monsterPrefab == null)
        {
            DebugTool.Error("몬스터 프리팹이 연결되지 않았습니다.", DebugType.Enemy, this);
            return;
        }

        if (_waypointPath == null)
        {
            DebugTool.Error("웨이포인트패쓰가 연결되지 않았습니다.", DebugType.Enemy, this);
            return;
        }

        MonsterController monsterInstance = Instantiate(
            _monsterPrefab,
            _waypointPath.GetWaypoint(0).position,
            Quaternion.identity);
        
        monsterInstance.Initialize(_waypointPath, _moveSpeed);
    }
    
    
}
