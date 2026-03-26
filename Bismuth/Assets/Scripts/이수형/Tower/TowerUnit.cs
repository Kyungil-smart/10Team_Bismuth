using UnityEngine;

[DisallowMultipleComponent]
public class TowerUnit : MonoBehaviour
{
    [Header("Tower Info")]
    [SerializeField] private string towerId = "TempTower";

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D selectionCollider;

    [Header("Drag Visual")]
    [SerializeField][Range(0.1f, 1f)] private float dragAlpha = 0.65f;
    [SerializeField] private int dragSortingOrderBoost = 50;

    private Color defaultColor = Color.white;
    private int defaultSortingOrder;

    public string TowerId => towerId;
    public PlacementSlot CurrentSlot { get; private set; }
    public Collider2D SelectionCollider => selectionCollider;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        selectionCollider = GetComponent<Collider2D>();
    }

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (selectionCollider == null)
            selectionCollider = GetComponent<Collider2D>();

        if (selectionCollider == null)
            selectionCollider = gameObject.AddComponent<BoxCollider2D>();

        if (spriteRenderer != null)
        {
            defaultColor = spriteRenderer.color;
            defaultSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    public void SetPlacedSlot(PlacementSlot slot)
    {
        CurrentSlot = slot;
        SnapToCurrentSlot();
    }

    public void SnapToCurrentSlot()
    {
        if (CurrentSlot == null)
            return;

        transform.position = new Vector3(CurrentSlot.WorldCenter.x, CurrentSlot.WorldCenter.y, 0f);
    }

    public void SetDragVisual(bool isDragging)
    {
        if (spriteRenderer == null)
            return;

        if (isDragging)
        {
            Color color = defaultColor;
            color.a = dragAlpha;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = defaultSortingOrder + dragSortingOrderBoost;
        }
        else
        {
            spriteRenderer.color = defaultColor;
            spriteRenderer.sortingOrder = defaultSortingOrder;
        }
    }
}