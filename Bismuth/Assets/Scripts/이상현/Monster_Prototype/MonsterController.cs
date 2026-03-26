using System;
using System.Collections;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Header("컴포넌트 참조")]
    [Tooltip("웨이포인트 이동을 담당하는 컴포넌트")]
    [SerializeField] private MonsterMover _mover;
    
    [Tooltip("애니메이션과 방향 포현 담당하는 컴포넌트")]
    [SerializeField] private MonsterAnimationController _animationController;
    
    [Header("테스트 설정")]
    [SerializeField, Min(0)] float _deactivateDelay = 0.2f;

    private bool _isInitialized;

    private void Reset()
    {
        _mover = GetComponent<MonsterMover>();
        _animationController = GetComponentInChildren<MonsterAnimationController>();
    }

    private void Awake()
    {
        if (_mover != null)
        {
            _mover.PathCompleted += HandlePathCompleted;
        }
    }

    private void OnDestroy()
    {
        if (_mover != null)
        {
            _mover.PathCompleted -= HandlePathCompleted;
        }
    }

    private void Update()
    {
        if (_isInitialized == false) return;
        if (_animationController == null || _mover == null) return;
        
        _animationController.SetMoving(_mover.IsMoving);
        _animationController.SetFaceDirection(_mover.MoveDirection);
    }

    public void Initialize(WaypointPath path, float moveSpeed)
    {
        _mover.Initialize(path, moveSpeed);
        _isInitialized = true;
    }

    private void HandlePathCompleted()
    {
        if (_animationController != null)
        {
            _animationController.SetMoving(false);
        }
        
        DebugTool.Log($"{name} : 경로 끝 도달", DebugType.Enemy, this);
        StartCoroutine(DeactivateAfterDelay());
    }

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(_deactivateDelay);
        gameObject.SetActive(false);
    }
}
