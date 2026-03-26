using UnityEngine;

public class CellHighlight : MonoBehaviour
{
    public static CellHighlight Instance { get; private set; }

    public enum HighlightState
    {
        Move = 0,
        Swap = 1,
        Invalid = 2
    }

    [SerializeField] private float highlightScaleMultiplier = 0.5f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color moveColor = new Color(0f, 1f, 0f, 0.35f);
    [SerializeField] private Color swapColor = new Color(1f, 0.85f, 0f, 0.4f);
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
        Show(slot, canPlace ? HighlightState.Move : HighlightState.Invalid);
    }

    public void Show(PlacementSlot slot, HighlightState state)
    {
        gameObject.SetActive(true);
        transform.position = new Vector3(slot.WorldCenter.x, slot.WorldCenter.y, 0f);
        transform.localScale = new Vector3(
            slot.HighlightSize.x * highlightScaleMultiplier,
            slot.HighlightSize.y * highlightScaleMultiplier,
            1f
        );

        if (spriteRenderer == null)
            return;

        switch (state)
        {
            case HighlightState.Move:
                spriteRenderer.color = moveColor;
                break;

            case HighlightState.Swap:
                spriteRenderer.color = swapColor;
                break;

            default:
                spriteRenderer.color = invalidColor;
                break;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}