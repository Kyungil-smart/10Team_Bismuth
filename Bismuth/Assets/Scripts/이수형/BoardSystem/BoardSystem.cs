using System.Collections.Generic;
using UnityEngine;

public class BoardSystem : MonoBehaviour
{
    public static BoardSystem Instance { get; private set; }

    [System.Serializable]
    public class SlotData
    {
        public PlacementSlot slot;
        public Vector3 worldCenter;
        public bool isOccupied;
        public TowerUnit occupiedTower;

        public SlotData(PlacementSlot slot)
        {
            this.slot = slot;
            this.worldCenter = slot.WorldCenter;
            this.isOccupied = false;
            this.occupiedTower = null;
        }
    }

    [Header("References")]
    [SerializeField] private Transform placementSlotRoot;
    [SerializeField] private LayerMask summonableSlotLayer;
    [SerializeField] private Transform placedTowerRoot;

    private Dictionary<PlacementSlot, SlotData> slotMap = new Dictionary<PlacementSlot, SlotData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DebugTool.Warnning("BoardSystem이 중복 생성되었습니다.", DebugType.Board, this);
        }

        Instance = this;
        RebuildBoard();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    [ContextMenu("Rebuild Board")]
    public void RebuildBoard()
    {
        slotMap.Clear();

        if (placementSlotRoot == null)
        {
            Debug.LogError("[BoardSystem] placementSlotRoot가 비어 있습니다.");
            return;
        }

        PlacementSlot[] slots = placementSlotRoot.GetComponentsInChildren<PlacementSlot>(true);

        foreach (PlacementSlot slot in slots)
        {
            if (slot == null)
                continue;

            if (!slotMap.ContainsKey(slot))
            {
                slotMap.Add(slot, new SlotData(slot));
            }
        }
    }

    public bool TryGetSlotFromWorld(Vector3 worldPos, out SlotData slotData)
    {
        slotData = null;

        Collider2D hit = Physics2D.OverlapPoint(worldPos, summonableSlotLayer);
        if (hit == null)
            return false;

        PlacementSlot slot = hit.GetComponent<PlacementSlot>();
        if (slot == null)
            slot = hit.GetComponentInParent<PlacementSlot>();

        if (slot == null)
            return false;

        return slotMap.TryGetValue(slot, out slotData);
    }

    public bool CanPlaceAtWorld(Vector3 worldPos, out SlotData slotData)
    {
        if (!TryGetSlotFromWorld(worldPos, out slotData))
            return false;

        return !slotData.isOccupied;
    }

    public bool PlaceNewTower(GameObject towerPrefab, SlotData targetSlot, out TowerUnit createdTower)
    {
        createdTower = null;

        if (towerPrefab == null)
        {
            Debug.LogWarning("[BoardSystem] towerPrefab이 비어 있습니다.");
            return false;
        }

        if (targetSlot == null || targetSlot.slot == null)
            return false;

        if (targetSlot.isOccupied)
            return false;

        if (placedTowerRoot == null)
        {
            GameObject root = new GameObject("PlacedTowers");
            placedTowerRoot = root.transform;
        }

        GameObject towerObj = Instantiate(
            towerPrefab,
            targetSlot.worldCenter,
            Quaternion.identity,
            placedTowerRoot
        );

        createdTower = towerObj.GetComponent<TowerUnit>();
        if (createdTower == null)
            createdTower = towerObj.AddComponent<TowerUnit>();

        createdTower.SetPlacedSlot(targetSlot.slot);

        targetSlot.isOccupied = true;
        targetSlot.occupiedTower = createdTower;

        return true;
    }

    public bool TryPlaceNewTowerAtWorld(GameObject towerPrefab, Vector3 worldPos, out TowerUnit createdTower)
    {
        createdTower = null;

        if (!CanPlaceAtWorld(worldPos, out SlotData slotData))
            return false;

        return PlaceNewTower(towerPrefab, slotData, out createdTower);
    }
}