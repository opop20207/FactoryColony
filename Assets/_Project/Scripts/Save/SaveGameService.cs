using System;
using System.IO;
#if !UNITY_5_3_OR_NEWER
using System.Text.Json;
#endif

namespace FactoryColony
{
    public sealed class SaveGameService
    {
#if !UNITY_5_3_OR_NEWER
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true
        };
#endif

        public FactorySaveData CreateSaveData(
            GridModel gridModel,
            BaseInventoryModel baseInventory,
            GoalTracker goalTracker)
        {
            if (gridModel == null)
            {
                throw new ArgumentNullException(nameof(gridModel));
            }

            if (baseInventory == null)
            {
                throw new ArgumentNullException(nameof(baseInventory));
            }

            FactorySaveData saveData = new FactorySaveData();
            saveData.Grid.Width = gridModel.Width;
            saveData.Grid.Height = gridModel.Height;

            foreach (GridCell cell in gridModel.GetAllCells())
            {
                if (cell.ResourceNodeType == ResourceType.None)
                {
                    continue;
                }

                saveData.Grid.ResourceNodes.Add(new ResourceNodeSaveData
                {
                    X = cell.Position.X,
                    Y = cell.Position.Y,
                    ResourceType = cell.ResourceNodeType
                });
            }

            foreach (BuildingModel building in gridModel.GetAllBuildings())
            {
                saveData.Grid.Buildings.Add(new BuildingSaveData
                {
                    InstanceId = building.InstanceId,
                    DefinitionId = building.Definition.Id,
                    BuildingType = building.Definition.Type,
                    OriginX = building.Origin.X,
                    OriginY = building.Origin.Y,
                    Direction = building.Direction,
                    Inventory = CreateInventorySaveData(building.Inventory),
                    SelectedRecipeId = building.SelectedRecipeId
                });
            }

            saveData.BaseInventory.Inventory = CreateInventorySaveData(baseInventory.Inventory);

            if (goalTracker != null)
            {
                foreach (ProductionGoalModel goal in goalTracker.Goals)
                {
                    saveData.Goals.Add(new GoalSaveData
                    {
                        Id = goal.Id,
                        CurrentAmount = goal.CurrentAmount,
                        IsCompleted = goal.IsCompleted
                    });
                }
            }

            return saveData;
        }

        public void SaveToFile(FactorySaveData saveData, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Save path cannot be null or empty.", nameof(path));
            }

            string directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, ToJson(saveData));
        }

        public bool TryLoadFromFile(string path, out FactorySaveData saveData)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                saveData = null;
                return false;
            }

            return TryFromJson(File.ReadAllText(path), out saveData);
        }

        public string ToJson(FactorySaveData saveData)
        {
            if (saveData == null)
            {
                throw new ArgumentNullException(nameof(saveData));
            }

#if UNITY_5_3_OR_NEWER
            return UnityEngine.JsonUtility.ToJson(saveData, true);
#else
            return JsonSerializer.Serialize(saveData, _jsonOptions);
#endif
        }

        public bool TryFromJson(string json, out FactorySaveData saveData)
        {
            if (string.IsNullOrEmpty(json))
            {
                saveData = null;
                return false;
            }

            try
            {
#if UNITY_5_3_OR_NEWER
                saveData = UnityEngine.JsonUtility.FromJson<FactorySaveData>(json);
#else
                saveData = JsonSerializer.Deserialize<FactorySaveData>(json, _jsonOptions);
#endif
                return saveData != null;
            }
            catch
            {
                saveData = null;
                return false;
            }
        }

        private static InventorySaveData CreateInventorySaveData(InventoryModel inventory)
        {
            InventorySaveData saveData = new InventorySaveData();

            if (inventory == null)
            {
                return saveData;
            }

            foreach (ResourceStack stack in inventory.GetStacks())
            {
                saveData.Resources.Add(new ResourceAmountSaveData
                {
                    ResourceType = stack.Type,
                    Amount = stack.Amount
                });
            }

            return saveData;
        }
    }
}
