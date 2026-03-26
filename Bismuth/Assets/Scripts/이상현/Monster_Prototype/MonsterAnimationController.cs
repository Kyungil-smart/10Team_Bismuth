using UnityEngine;

public class MonsterAnimationController : MonoBehaviour
{
    [Header("컴포넌트 참조")]
    [SerializeField] private Animator _animator;
    
    [Tooltip("좌우 반전에 사용할 스프라이트랜더러")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    private static readonly int IS_MOVING_HASH = Animator.StringToHash("isMoving");

    private void Reset()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetMoving(bool isMoving)
    {
        if (_animator == null) return;
        _animator.SetBool(IS_MOVING_HASH, isMoving);
    }

    public void SetFaceDirection(Vector2 moveDirection)
    {
        if (_spriteRenderer == null) return;
        if (Mathf.Abs(moveDirection.x) < 0.001f) return;
        
        _spriteRenderer.flipX = moveDirection.x < 0f;
    }
    
}
