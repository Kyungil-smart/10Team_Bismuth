using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardSystem : MonoBehaviour
{
    [System.Serializable]
    public class CellData
    {
        public Vector3Int cellPos;
        public Vector3 worldCenter;
        public bool isOccupied;
        public TowerUnit occupiedTower;

        public CellData(Vector3Int cellPos, Vector3 worldCenter)
        {
            this.cellPos = cellPos;
            this.worldCenter = worldCenter;
            this.isOccupied = false;
            this.occupiedTower = null;
        }
    }

    [Header("References")]
    [SerializeField] private Tilemap summonableTilemap;
    [SerializeField] private Transform placedTowerRoot;

    private Dictionary<Vector3Int, CellData> cellMap = new Dictionary<Vector3Int, CellData>();

    public Tilemap SummonableTilemap => summonableTilemap;

    private void Awake()
    {
        RebuildBoard();
    }

    [ContextMenu("Rebuild Board")]
    public void RebuildBoard()
    {
        cellMap.Clear();

        if (summonableTilemap == null)
        {
            Debug.LogError("[BoardSystem] SummonableTilemap이 비어 있습니다.");
            return;
        }

        BoundsInt bounds = summonableTilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (!summonableTilemap.HasTile(pos))
                continue;

            Vector3 center = summonableTilemap.GetCellCenterWorld(pos);
            cellMap[pos] = new CellData(pos, center);
        }
    }

    public bool TryGetCellFromWorld(Vector3 worldPos, out CellData cell)
    {
        cell = null;

        if (summonableTilemap == null)
            return false;

        Vector3Int cellPos = summonableTilemap.WorldToCell(worldPos);
        return cellMap.TryGetValue(cellPos, out cell);
    }

    public bool CanPlaceAtWorld(Vector3 worldPos, out CellData cell)
    {
        if (!TryGetCellFromWorld(worldPos, out cell))
            return false;

        return !cell.isOccupied;
    }

    public bool PlaceNewTower(GameObject towerPrefab, CellData targetCell, out TowerUnit createdTower)
    {
        createdTower = null;

        if (towerPrefab == null)
        {
            Debug.LogWarning("[BoardSystem] towerPrefab이 비어 있습니다.");
            return false;
        }

        if (targetCell == null)
            return false;

        if (targetCell.isOccupied)
            return false;

        if (placedTowerRoot == null)
        {
            GameObject root = new GameObject("PlacedTowers");
            placedTowerRoot = root.transform;
        }

        GameObject towerObj = Instantiate(
            towerPrefab,
            targetCell.worldCenter,
            Quaternion.identity,
            placedTowerRoot
        );

        createdTower = towerObj.GetComponent<TowerUnit>();
        if (createdTower == null)
        {
            createdTower = towerObj.AddComponent<TowerUnit>();
        }

        createdTower.SetPlacedCell(targetCell.cellPos);

        targetCell.isOccupied = true;
        targetCell.occupiedTower = createdTower;

        return true;
    }

    public bool TryPlaceNewTowerAtWorld(GameObject towerPrefab, Vector3 worldPos, out TowerUnit createdTower)
    {
        createdTower = null;

        if (!CanPlaceAtWorld(worldPos, out CellData cell))
            return false;

        return PlaceNewTower(towerPrefab, cell, out createdTower);
    }
}