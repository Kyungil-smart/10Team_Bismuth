using UnityEngine;

public class CellHighlight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color validColor = new Color(0f, 1f, 0f, 0.35f);
    [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.35f);

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Show(Vector3 worldPos, bool canPlace)
    {
        gameObject.SetActive(true);
        transform.position = new Vector3(worldPos.x, worldPos.y, 0f);

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