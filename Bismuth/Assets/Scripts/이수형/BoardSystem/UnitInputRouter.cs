using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class UnitPointerInputRouter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera worldCamera;

    [Header("Layers")]
    [SerializeField] private LayerMask unitLayer;

    private void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            DebugTool.Log("유닛 입력 무시 - UI 위 클릭", DebugType.Unit, this);
            return;
        }

        Vector3 mouseWorld = GetMouseWorld();

        Collider2D hit = Physics2D.OverlapPoint(mouseWorld, unitLayer);
        if (hit == null)
            return;

        TowerLongPressDragHandler dragHandler = hit.GetComponent<TowerLongPressDragHandler>();
        if (dragHandler == null)
            dragHandler = hit.GetComponentInParent<TowerLongPressDragHandler>();

        if (dragHandler == null)
        {
            DebugTool.Warnning(
                $"Unit 레이어 클릭 대상에 TowerLongPressDragHandler가 없습니다. target={hit.name}",
                DebugType.Unit,
                this
            );
            return;
        }

        bool started = dragHandler.BeginPress((Vector2)Input.mousePosition);
        if (started)
        {
            DebugTool.Log(
                $"유닛 홀드 입력 라우팅 성공 - target={hit.name}",
                DebugType.Unit,
                this
            );
        }
    }

    private Vector3 GetMouseWorld()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        if (worldCamera == null)
            return Vector3.zero;

        float zDistance = -worldCamera.transform.position.z;
        Vector3 worldPos = worldCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance)
        );

        worldPos.z = 0f;
        return worldPos;
    }
}