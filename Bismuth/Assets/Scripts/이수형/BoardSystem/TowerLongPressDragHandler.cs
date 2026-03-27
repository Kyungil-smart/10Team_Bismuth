using UnityEngine;


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

    public void Initialize(BoardSystem board, Camera cam, CellHighlight highlight)
    {
        boardSystem = board;
        worldCamera = cam;
        cellHighlight = highlight;
    }
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
    public bool BeginPress(Vector2 screenPos)
    {
        if (isActiveAndEnabled == false)
            return false;
        if (towerUnit == null || towerUnit.CurrentSlot == null) 
            return false;


        

        isPressed = true;
        isDragging = false;
        pressedTime = Time.time;
        pressedScreenPos = screenPos;

        DebugTool.Log("타워 홀드 시작", DebugType.Unit, this);
        return true;
    }
    //private void OnMouseDown()
    //{
    //    DebugTool.Log("타워 홀드 마우스인식", DebugType.Unit, this);
    //    if (towerUnit == null || towerUnit.CurrentSlot == null)
    //        return;

    //    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
    //        return;

    //    isPressed = true;
    //    isDragging = false;
    //    pressedTime = Time.time;
    //    pressedScreenPos = Input.mousePosition;

    //    DebugTool.Log("타워 홀드 시작", DebugType.Unit, this);
    //}

    private void BeginDrag()
    {
        Vector3 mouseWorld = GetMouseWorld();
        dragOffset = transform.position - mouseWorld;

        isDragging = true;

        towerUnit.SetSelectionColliderEnabled(false);
        towerUnit.SetDragVisual(true);

        UpdateDrag();

        DebugTool.Log("타워 드래그 시작", DebugType.Unit, this);
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

        if (!boardSystem.TryGetSlotFromWorld(mouseWorld, out BoardSystem.SlotData slotData))
        {
            if (cellHighlight != null)
                cellHighlight.Hide();
            return;
        }

        CellHighlight.HighlightState highlightState;

        if (slotData.slot == towerUnit.CurrentSlot)
        {
            highlightState = CellHighlight.HighlightState.Move;
        }
        else if (!slotData.isOccupied)
        {
            highlightState = CellHighlight.HighlightState.Move;
        }
        else if (slotData.occupiedTower != towerUnit)
        {
            highlightState = CellHighlight.HighlightState.Swap;
        }
        else
        {
            highlightState = CellHighlight.HighlightState.Move;
        }

        if (cellHighlight != null)
            cellHighlight.Show(slotData.slot, highlightState);
    }

    private void Release()
    {
        if (isDragging)
        {
            Vector3 mouseWorld = GetMouseWorld();

            BoardSystem.RelocateResult relocateResult = BoardSystem.RelocateResult.Invalid;

            bool success = false;
            if (boardSystem != null)
            {
                success = boardSystem.TryRelocateOrSwapFromWorld(
                    towerUnit,
                    mouseWorld,
                    out relocateResult
                );
            }

            towerUnit.SetDragVisual(false);
            towerUnit.SetSelectionColliderEnabled(true);

            if (!success || relocateResult == BoardSystem.RelocateResult.Invalid)
            {
                towerUnit.SnapToCurrentSlot();
                DebugTool.Log("드롭 실패 - 유효하지 않은 위치라 원위치 복귀", DebugType.Board, this);
            }
            else
            {
                switch (relocateResult)
                {
                    case BoardSystem.RelocateResult.SameSlot:
                        DebugTool.Log("같은 슬롯에 드롭", DebugType.Board, this);
                        break;

                    case BoardSystem.RelocateResult.Moved:
                        DebugTool.Log("빈 슬롯으로 이동 완료", DebugType.Board, this);
                        break;

                    case BoardSystem.RelocateResult.Swapped:
                        DebugTool.Log("다른 타워와 위치 교체 완료", DebugType.Board, this);
                        break;
                }
            }
        }

        ResetState();
    }

    private void CancelHold(string reason)
    {
        DebugTool.Log(reason, DebugType.Board, this);
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
            towerUnit.SetSelectionColliderEnabled(true);
        }

        ResetState();
    }

    
}