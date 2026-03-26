using UnityEngine;

public class CellHighlight : MonoBehaviour
{
    public static CellHighlight Instance { get; private set; }

    float highlightScaleMultiplier = 0.5f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color validColor = new Color(0f, 1f, 0f, 0.35f);
    [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.35f);

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Instance = this;
    }

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Show(PlacementSlot slot, bool canPlace)
    {
        if (slot == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);
        transform.position = new Vector3(slot.WorldCenter.x, slot.WorldCenter.y, 0f);
        transform.localScale = new Vector3(slot.HighlightSize.x * highlightScaleMultiplier, slot.HighlightSize.y * highlightScaleMultiplier, 1f);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = canPlace ? validColor : invalidColor;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}