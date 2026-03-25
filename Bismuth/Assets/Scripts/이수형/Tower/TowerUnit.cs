using UnityEngine;

public class TowerUnit : MonoBehaviour
{
    [SerializeField] private string towerId = "TempTower";

    public string TowerId => towerId;
    public PlacementSlot CurrentSlot { get; private set; }

    public void SetPlacedSlot(PlacementSlot slot)
    {
        CurrentSlot = slot;

        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}