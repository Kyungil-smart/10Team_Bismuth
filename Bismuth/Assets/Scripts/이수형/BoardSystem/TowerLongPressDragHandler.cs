using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
[RequireComponent(typeof(TowerUnit))]
public class TowerLongPressDragHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardSystem boardSystem;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private CellHighlight cellHighlight;

    [Header("Long Press")]
    [SerializeField] private float holdDuration = 1f;
    [SerializeField] private float holdCancelThresholdPixels = 20f;

    private TowerUnit towerUnit;
    private bool isPressed;
    private bool isDragging;
    private float pressedTime;
    private Vector2 pressedScreenPos;
    private Vector3 dragOffset;

    private void Awake()
    {
        towerUnit = GetComponent<TowerUnit>();

        if (boardSystem == null)
            boardSystem = BoardSystem.Instance != null ? BoardSystem.Instance : FindAnyObjectByType<BoardSystem>();

        if (worldCamera == null)
            worldCamera = Camera.main;

        if (cellHighlight == null)
            cellHighlight = CellHighlight.Instance;
    }

    private void Update()
    {
        if (!isPressed)
            return;

        if (!Input.GetMouseButton(0))
        {
            Release();
            return;
        }

        if (!isDragging)
        {
            if (Vector2.Distance(pressedScreenPos, (Vector2)Input.mousePosition) > holdCancelThresholdPixels)
            {
                CancelHold("홀드 취소 - 1초 전에 마우스가 많이 움직였습니다.");
                return;
            }

            if (Time.time - pressedTime >= holdDuration)
            {
                BeginDrag();
            }

            return;
        }

        UpdateDrag();
    }

    private void OnMouseDown()
    {
        if (towerUnit == null || towerUnit.CurrentSlot == null)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        isPressed = true;
        isDragging = false;
        pressedTime = Time.time;
        pressedScreenPos = Input.mousePosition;

        DebugTool.Log("타워 홀드 시작", DebugType.Tower, this);
    }

    private void BeginDrag()
    {
        Vector3 mouseWorld = GetMouseWorld();
        dragOffset = transform.position - mouseWorld;

        isDragging = true;
        towerUnit.SetDragVisual(true);
        UpdateDrag();

        DebugTool.Log("타워 드래그 시작", DebugType.Tower, this);
    }

    private void UpdateDrag()
    {
        Vector3 mouseWorld = GetMouseWorld();

        Vector3 nextPos = mouseWorld + dragOffset;
        nextPos.z = 0f;
        transform.position = nextPos;

        UpdatePreview(mouseWorld);
    }

    private void UpdatePreview(Vector3 mouseWorld)
    {
        if (boardSystem == null)
            return;

        if (cellHighlight == null)
            cellHighlight = CellHighlight.Instance;

        if (boardSystem.TryGetSlotFromWorld(mouseWorld, out BoardSystem.SlotData slotData))
        {
            if (cellHighlight != null)
                cellHighlight.Show(slotData.slot, true);
        }
        else
        {
            if (cellHighlight != null)
                cellHighlight.Hide();
        }
    }

    private void Release()
    {
        if (isDragging)
        {
            towerUnit.SnapToCurrentSlot();
            towerUnit.SetDragVisual(false);
            DebugTool.Log("드래그 종료 - 1단계에서는 원위치 복귀", DebugType.Tower, this);
        }
        else
        {
            DebugTool.Log("홀드 해제 - 드래그 시작 전 종료", DebugType.Tower, this);
        }

        ResetState();
    }

    private void CancelHold(string reason)
    {
        DebugTool.Log(reason, DebugType.Tower, this);
        ResetState();
    }

    private void ResetState()
    {
        isPressed = false;
        isDragging = false;

        if (cellHighlight != null)
            cellHighlight.Hide();
    }

    private Vector3 GetMouseWorld()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        float zDistance = -worldCamera.transform.position.z;
        Vector3 worldPos = worldCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance)
        );

        worldPos.z = 0f;
        return worldPos;
    }

    private void OnDisable()
    {
        if (towerUnit != null)
        {
            towerUnit.SnapToCurrentSlot();
            towerUnit.SetDragVisual(false);
        }

        ResetState();
    }
}