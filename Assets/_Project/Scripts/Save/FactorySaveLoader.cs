using System;
using System.Collections.Generic;

namespace FactoryColony
{
    public sealed class FactorySaveLoader
    {
        private readonly Action<string> _warningLogger;

        public FactorySaveLoader()
            : this(null)
        {
        }

        public FactorySaveLoader(Action<string> warningLogger)
        {
            _warningLogger = warningLogger;
        }

        public GridModel RestoreGrid(
            FactorySaveData saveData,
            IReadOnlyDictionary<string, BuildingDefinition> definitionsById)
        {
            if (saveData == null)
            {
                throw new ArgumentNullException(nameof(saveData));
            }

            if (definitionsById == null)
            {
                throw new ArgumentNullException(nameof(definitionsById));
            }

            GridSaveData gridSaveData = saveData.Grid ?? new GridSaveData();
            GridModel gridModel = new GridModel(gridSaveData.Width, gridSaveData.Height);

            if (gridSaveData.ResourceNodes != null)
            {
                foreach (ResourceNodeSaveData resourceNode in gridSaveData.ResourceNodes)
                {
                    gridModel.SetResourceNode(
                        new GridPosition(resourceNode.X, resourceNode.Y),
                        resourceNode.ResourceType);
                }
            }

            if (gridSaveData.Buildings != null)
            {
                foreach (BuildingSaveData buildingSaveData in gridSaveData.Buildings)
                {
                    RestoreBuilding(gridModel, buildingSaveData, definitionsById);
                }
            }

            return gridModel;
        }

        public BaseInventoryModel RestoreBaseInventory(FactorySaveData saveData)
        {
            if (saveData == null)
            {
                throw new ArgumentNullException(nameof(saveData));
            }

            BaseInventoryModel baseInventory = new BaseInventoryModel();
            RestoreInventory(baseInventory.Inventory, saveData.BaseInventory != null ? saveData.BaseInventory.Inventory : null);
            return baseInventory;
        }

        public void RestoreGoals(FactorySaveData saveData, GoalTracker goalTracker)
        {
            if (goalTracker == null)
            {
                return;
            }

            goalTracker.UpdateGoals();
        }

        private void RestoreBuilding(
            GridModel gridModel,
            BuildingSaveData buildingSaveData,
            IReadOnlyDictionary<string, BuildingDefinition> definitionsById)
        {
            if (buildingSaveData == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(buildingSaveData.DefinitionId)
                || !definitionsById.TryGetValue(buildingSaveData.DefinitionId, out BuildingDefinition definition))
            {
                LogWarning("Skipped saved building '" + buildingSaveData.InstanceId + "': unknown definition '" + buildingSaveData.DefinitionId + "'.");
                return;
            }

            BuildingModel building = new BuildingModel(
                buildingSaveData.InstanceId,
                definition,
                new GridPosition(buildingSaveData.OriginX, buildingSaveData.OriginY),
                buildingSaveData.Direction);

            building.SelectedRecipeId = buildingSaveData.SelectedRecipeId;
            RestoreInventory(building.Inventory, buildingSaveData.Inventory);

            if (!gridModel.TryPlaceBuilding(building))
            {
                LogWarning("Skipped saved building '" + buildingSaveData.InstanceId + "': placement failed.");
            }
        }

        private static void RestoreInventory(InventoryModel inventory, InventorySaveData saveData)
        {
            if (inventory == null || saveData == null || saveData.Resources == null)
            {
                return;
            }

            inventory.Clear();

            foreach (ResourceAmountSaveData resourceAmount in saveData.Resources)
            {
                if (resourceAmount == null
                    || resourceAmount.ResourceType == ResourceType.None
                    || resourceAmount.Amount <= 0)
                {
                    continue;
                }

                inventory.Add(resourceAmount.ResourceType, resourceAmount.Amount);
            }
        }

        private void LogWarning(string message)
        {
            if (_warningLogger != null)
            {
                _warningLogger(message);
            }
        }
    }
}
