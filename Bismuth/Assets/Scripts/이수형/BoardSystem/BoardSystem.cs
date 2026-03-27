using System.Collections.Generic;
using UnityEngine;

public class BoardSystem : MonoBehaviour
{
    public static BoardSystem Instance { get; private set; }

    public enum RelocateResult
    {
        Invalid = 0,
        SameSlot = 1,
        Moved = 2,
        Swapped = 3
    }

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
            DebugTool.Error("placementSlotRoot가 비어 있습니다.", DebugType.Board, this);
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

        DebugTool.Log($"보드 재구성 완료 - 슬롯 수: {slotMap.Count}", DebugType.Board, this);
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

    public bool TryGetSlotData(PlacementSlot slot, out SlotData slotData)
    {
        slotData = null;

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
            DebugTool.Warnning("towerPrefab이 비어 있습니다.", DebugType.Board, this);
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

        DebugTool.Log(
            $"새 타워 배치 성공 - {createdTower.TowerId} -> {targetSlot.slot.name}",
            DebugType.Board,
            this
        );

        return true;
    }

    public bool TryPlaceNewTowerAtWorld(GameObject towerPrefab, Vector3 worldPos, out TowerUnit createdTower)
    {
        createdTower = null;

        if (!CanPlaceAtWorld(worldPos, out SlotData slotData))
            return false;

        return PlaceNewTower(towerPrefab, slotData, out createdTower);
    }

    public bool TryRelocateOrSwapFromWorld(TowerUnit tower, Vector3 worldPos, out RelocateResult result)
    {
        result = RelocateResult.Invalid;

        if (tower == null || tower.CurrentSlot == null)
            return false;

        if (!TryGetSlotFromWorld(worldPos, out SlotData targetSlot))
            return false;

        return TryRelocateOrSwap(tower, targetSlot, out result);
    }

    public bool TryRelocateOrSwap(TowerUnit tower, SlotData targetSlot, out RelocateResult result)
    {
        result = RelocateResult.Invalid;

        if (tower == null || targetSlot == null || targetSlot.slot == null)
            return false;

        if (!TryGetSlotData(tower.CurrentSlot, out SlotData sourceSlot))
        {
            DebugTool.Error("현재 타워의 sourceSlot을 찾지 못했습니다.", DebugType.Board, this);
            return false;
        }

        if (!sourceSlot.isOccupied || sourceSlot.occupiedTower != tower)
        {
            DebugTool.Warnning(
                $"sourceSlot 점유 정보가 타워와 일치하지 않아 자동 보정합니다. slot={sourceSlot.slot.name}",
                DebugType.Board,
                this
            );

            sourceSlot.isOccupied = true;
            sourceSlot.occupiedTower = tower;
        }

        if (sourceSlot == targetSlot)
        {
            tower.SnapToCurrentSlot();
            result = RelocateResult.SameSlot;

            DebugTool.Log(
                $"같은 슬롯 드롭 - {tower.TowerId} / {sourceSlot.slot.name}",
                DebugType.Board,
                this
            );

            return true;
        }

        if (!targetSlot.isOccupied)
        {
            sourceSlot.isOccupied = false;
            sourceSlot.occupiedTower = null;

            targetSlot.isOccupied = true;
            targetSlot.occupiedTower = tower;

            tower.SetPlacedSlot(targetSlot.slot);

            result = RelocateResult.Moved;

            DebugTool.Log(
                $"타워 이동 성공 - {tower.TowerId}: {sourceSlot.slot.name} -> {targetSlot.slot.name}",
                DebugType.Board,
                this
            );

            return true;
        }

        TowerUnit targetTower = targetSlot.occupiedTower;

        if (targetTower == null)
        {
            DebugTool.Warnning(
                $"targetSlot이 점유 상태인데 occupiedTower가 null입니다. 이동으로 보정합니다. slot={targetSlot.slot.name}",
                DebugType.Board,
                this
            );

            sourceSlot.isOccupied = false;
            sourceSlot.occupiedTower = null;

            targetSlot.isOccupied = true;
            targetSlot.occupiedTower = tower;

            tower.SetPlacedSlot(targetSlot.slot);

            result = RelocateResult.Moved;
            return true;
        }

        if (targetTower == tower)
        {
            tower.SnapToCurrentSlot();
            result = RelocateResult.SameSlot;
            return true;
        }

        sourceSlot.isOccupied = true;
        sourceSlot.occupiedTower = targetTower;

        targetSlot.isOccupied = true;
        targetSlot.occupiedTower = tower;

        PlacementSlot sourcePlacementSlot = sourceSlot.slot;
        PlacementSlot targetPlacementSlot = targetSlot.slot;

        targetTower.SetPlacedSlot(sourcePlacementSlot);
        tower.SetPlacedSlot(targetPlacementSlot);

        result = RelocateResult.Swapped;

        DebugTool.Log(
            $"타워 스왑 성공 - {tower.TowerId}({sourcePlacementSlot.name} -> {targetPlacementSlot.name}) <-> {targetTower.TowerId}({targetPlacementSlot.name} -> {sourcePlacementSlot.name})",
            DebugType.Board,
            this
        );

        return true;
    }
    public bool TryGetFirstEmptySlot(out SlotData emptySlot)
    {
        emptySlot = null;

        if (placementSlotRoot == null)
        {
            DebugTool.Warnning("placementSlotRoot가 비어 있습니다.", DebugType.Board, this);
            return false;
        }

        PlacementSlot[] slots = placementSlotRoot.GetComponentsInChildren<PlacementSlot>(true);

        foreach (PlacementSlot slot in slots)
        {
            if (slot == null)
                continue;

            if (!slotMap.TryGetValue(slot, out SlotData slotData))
                continue;

            if (!slotData.isOccupied)
            {
                emptySlot = slotData;
                return true;
            }
        }

        return false;
    }
}