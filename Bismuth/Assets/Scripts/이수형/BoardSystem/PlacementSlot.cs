using UnityEngine;

[DisallowMultipleComponent]
public class PlacementSlot : MonoBehaviour
{
    [Header("Slot Info")]
    [SerializeField] private Vector2 highlightSize = new Vector2(2f, 2f);

    public Vector3 WorldCenter => transform.position;
    public Vector2 HighlightSize => highlightSize;
}