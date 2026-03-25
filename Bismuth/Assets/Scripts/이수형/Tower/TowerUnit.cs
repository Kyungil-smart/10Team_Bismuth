using UnityEngine;

public class TowerUnit : MonoBehaviour
{
    [SerializeField] private string towerId = "TempTower";

    public string TowerId => towerId;
    public Vector3Int CurrentCell { get; private set; }

    public void SetPlacedCell(Vector3Int cellPos)
    {
        CurrentCell = cellPos;

        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}