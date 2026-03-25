using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerDragSpawnerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private BoardSystem boardSystem;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private CellHighlight cellHighlight;

    [Header("Tower Data")]
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Sprite iconSprite;

    [Header("Ghost UI")]
    [SerializeField] private Vector2 ghostSize = new Vector2(80f, 80f);
    [SerializeField][Range(0f, 1f)] private float ghostAlpha = 0.75f;

    private RectTransform ghostRect;
    private Image ghostImage;
    private bool isDragging;

    private void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        if (rootCanvas == null)
        {
            Canvas foundCanvas = GetComponentInParent<Canvas>();
            if (foundCanvas != null)
                rootCanvas = foundCanvas.rootCanvas;
        }

        if (iconSprite == null)
        {
            Image myImage = GetComponent<Image>();
            if (myImage != null)
                iconSprite = myImage.sprite;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (boardSystem == null || towerPrefab == null || rootCanvas == null)
            return;

        isDragging = true;

        EnsureGhost();
        ghostImage.sprite = iconSprite;
        ghostImage.color = new Color(1f, 1f, 1f, ghostAlpha);
        ghostRect.sizeDelta = ghostSize;
        ghostRect.gameObject.SetActive(true);

        UpdateGhostPosition(eventData);
        UpdateBoardPreview(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        UpdateGhostPosition(eventData);
        UpdateBoardPreview(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;

        Vector3 worldPos = ScreenToWorld(eventData.position);

        bool placed = boardSystem.TryPlaceNewTowerAtWorld(towerPrefab, worldPos, out TowerUnit createdTower);

        if (!placed)
        {
            Debug.Log("[TowerDragSpawnerUI] 배치 실패: 유효한 슬롯이 아니거나 이미 점유된 슬롯입니다.");
        }

        CleanupPreview();
    }

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        if (ghostRect == null || rootCanvas == null)
            return;

        RectTransform canvasRect = rootCanvas.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPos))
        {
            ghostRect.anchoredPosition = localPos;
        }
    }

    private void UpdateBoardPreview(Vector2 screenPos)
    {
        if (boardSystem == null)
            return;

        Vector3 worldPos = ScreenToWorld(screenPos);

        if (boardSystem.TryGetSlotFromWorld(worldPos, out BoardSystem.SlotData slotData))
        {
            bool canPlace = !slotData.isOccupied;

            if (cellHighlight != null)
                cellHighlight.Show(slotData.slot, canPlace);
        }
        else
        {
            if (cellHighlight != null)
                cellHighlight.Hide();
        }
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        float zDistance = -worldCamera.transform.position.z;
        Vector3 worldPos = worldCamera.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, zDistance)
        );

        worldPos.z = 0f;
        return worldPos;
    }

    private void EnsureGhost()
    {
        if (ghostRect != null)
            return;

        GameObject ghostObj = new GameObject("TowerDragGhost");
        ghostObj.transform.SetParent(rootCanvas.transform, false);
        ghostObj.transform.SetAsLastSibling();

        ghostRect = ghostObj.AddComponent<RectTransform>();
        ghostImage = ghostObj.AddComponent<Image>();
        ghostImage.raycastTarget = false;

        CanvasGroup canvasGroup = ghostObj.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
    }

    private void CleanupPreview()
    {
        if (ghostRect != null)
            ghostRect.gameObject.SetActive(false);

        if (cellHighlight != null)
            cellHighlight.Hide();
    }

    private void OnDisable()
    {
        CleanupPreview();
        isDragging = false;
    }
}