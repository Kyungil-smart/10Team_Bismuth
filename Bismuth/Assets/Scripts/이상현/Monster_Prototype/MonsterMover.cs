using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MonsterMover : MonoBehaviour
{
    [Header("컴포넌트 참조")]
    [Tooltip("몬스터 이동에 사용할 Rigidbody2D")]
    [SerializeField] private Rigidbody2D _rigidbody2D;

    [Header("이동 설정")] 
    [Tooltip("웨이포인트 도착으로 판정할 거리")] 
    [SerializeField, Min(0.01f)] private float _arriveDistance = 0.05f;
    
    private WaypointPath _path;
    private float _moveSpeed;
    private int _currentWaypointIndex;
    private bool _isInitialized;
    private bool _isPathCompleted;
    
    public bool IsMoving { get; private set; }
    public Vector2 MoveDirection { get; private set; }

    public Action PathCompleted;

    private void Reset()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_isInitialized == false || _isPathCompleted) return;
        
        MoveAlongPath();
    }
    
    public void Initialize(WaypointPath path, float moveSpeed)
    {
        _path = path;
        _moveSpeed = moveSpeed;
        _isPathCompleted = false;

        if (_path == null || _path.WaypointCount == 0)
        {
            DebugTool.Error($"{name} : WayPointPath가 비어 있거나 연결되지 않았습니다.", DebugType.Enemy);
            _isInitialized = false;
            IsMoving = false;
            MoveDirection = Vector2.zero;
            return;
        }
        
        transform.position = _path.GetWaypoint(0).position;
        
        // 첫 번째 웨이포인트는 스폰위치
        // 이동은 다음 웨이포인트부터 시작
        _currentWaypointIndex = 1;
        _isInitialized = true;

        if (_path.WaypointCount <= 1)
        {
            CompletePath();
        }
        else
        {
            IsMoving = true;
        }
    }

    private void MoveAlongPath()
    {
        if (_currentWaypointIndex >= _path.WaypointCount)
        {
            CompletePath();
            return;
        }

        Vector2 currentPosition = _rigidbody2D.position;
        Vector2 targetPosition = _path.GetWaypoint(_currentWaypointIndex).position;
        
        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            _moveSpeed * Time.fixedDeltaTime);
        
        MoveDirection = nextPosition - currentPosition;
        IsMoving = MoveDirection.sqrMagnitude > 0.000001f;
        
        _rigidbody2D.MovePosition(nextPosition);
        
        float remainingDistance = Vector2.Distance(nextPosition, targetPosition);
        if (remainingDistance <= _arriveDistance)
        {
            _currentWaypointIndex++;
            
            if (_currentWaypointIndex >= _path.WaypointCount)
            {
                CompletePath();
            }
        }

    }

    private void CompletePath()
    {
        _isPathCompleted = true;
        IsMoving = false;
        MoveDirection = Vector2.zero;
        PathCompleted?.Invoke();
    }
}
