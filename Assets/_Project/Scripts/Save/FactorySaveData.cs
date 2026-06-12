using System;
using System.Collections.Generic;

namespace FactoryColony
{
    [Serializable]
    public sealed class FactorySaveData
    {
        public GridSaveData Grid = new GridSaveData();
        public BaseInventorySaveData BaseInventory = new BaseInventorySaveData();
        public PlayerInventorySaveData PlayerInventory = new PlayerInventorySaveData();
        public List<GoalSaveData> Goals = new List<GoalSaveData>();
        public List<string> CompletedResearchIds = new List<string>();
    }

    [Serializable]
    public sealed class GridSaveData
    {
        public int Width;
        public int Height;
        public List<ResourceNodeSaveData> ResourceNodes = new List<ResourceNodeSaveData>();
        public List<BuildingSaveData> Buildings = new List<BuildingSaveData>();
    }

    [Serializable]
    public sealed class ResourceNodeSaveData
    {
        public int X;
        public int Y;
        public ResourceType ResourceType;
    }

    [Serializable]
    public sealed class BuildingSaveData
    {
        public string InstanceId;
        public string DefinitionId;
        public BuildingType BuildingType;
        public int OriginX;
        public int OriginY;
        public BuildingDirection Direction;
        public InventorySaveData Inventory = new InventorySaveData();
        public string SelectedRecipeId;
    }

    [Serializable]
    public sealed class InventorySaveData
    {
        public List<ResourceAmountSaveData> Resources = new List<ResourceAmountSaveData>();
    }

    [Serializable]
    public sealed class ResourceAmountSaveData
    {
        public ResourceType ResourceType;
        public int Amount;
    }

    [Serializable]
    public sealed class BaseInventorySaveData
    {
        public InventorySaveData Inventory = new InventorySaveData();
    }

    [Serializable]
    public sealed class PlayerInventorySaveData
    {
        public int MaxTotalAmount = PlayerInventoryModel.DefaultMaxTotalAmount;
        public InventorySaveData Inventory = new InventorySaveData();
    }

    [Serializable]
    public sealed class GoalSaveData
    {
        public string Id;
        public int CurrentAmount;
        public bool IsCompleted;
    }
}
