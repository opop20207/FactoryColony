using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FactoryColony
{
    public sealed class StorageCollector
    {
        private readonly GridModel _gridModel;
        private readonly BaseInventoryModel _baseInventory;

        public StorageCollector(GridModel gridModel, BaseInventoryModel baseInventory)
        {
            _gridModel = gridModel ?? throw new ArgumentNullException(nameof(gridModel));
            _baseInventory = baseInventory ?? throw new ArgumentNullException(nameof(baseInventory));
        }

        public int CollectAll()
        {
            int totalCollected = 0;

            foreach (int amount in CollectAllByType().Values)
            {
                totalCollected += amount;
            }

            return totalCollected;
        }

        public int CollectResource(ResourceType type)
        {
            if (type == ResourceType.None)
            {
                return 0;
            }

            int collectedAmount = 0;

            foreach (BuildingModel storage in GetStorageBuildings())
            {
                int removedAmount = storage.Inventory.RemoveAll(type);

                if (removedAmount <= 0)
                {
                    continue;
                }

                _baseInventory.Add(type, removedAmount);
                collectedAmount += removedAmount;
            }

            return collectedAmount;
        }

        public IReadOnlyDictionary<ResourceType, int> CollectAllByType()
        {
            Dictionary<ResourceType, int> collectedResources = new Dictionary<ResourceType, int>();

            foreach (BuildingModel storage in GetStorageBuildings())
            {
                ResourceStack[] stacks = storage.Inventory.GetStacks().ToArray();

                foreach (ResourceStack stack in stacks)
                {
                    int removedAmount = storage.Inventory.RemoveAll(stack.Type);

                    if (removedAmount <= 0)
                    {
                        continue;
                    }

                    _baseInventory.Add(stack.Type, removedAmount);

                    if (!collectedResources.ContainsKey(stack.Type))
                    {
                        collectedResources[stack.Type] = 0;
                    }

                    collectedResources[stack.Type] += removedAmount;
                }
            }

            return new ReadOnlyDictionary<ResourceType, int>(collectedResources);
        }

        public IReadOnlyDictionary<ResourceType, int> CollectFromStorage(BuildingModel storage)
        {
            Dictionary<ResourceType, int> collectedResources = new Dictionary<ResourceType, int>();

            if (storage == null || storage.Definition.Type != BuildingType.Storage)
            {
                return new ReadOnlyDictionary<ResourceType, int>(collectedResources);
            }

            ResourceStack[] stacks = storage.Inventory.GetStacks().ToArray();

            foreach (ResourceStack stack in stacks)
            {
                int removedAmount = storage.Inventory.RemoveAll(stack.Type);

                if (removedAmount <= 0)
                {
                    continue;
                }

                _baseInventory.Add(stack.Type, removedAmount);
                collectedResources[stack.Type] = removedAmount;
            }

            return new ReadOnlyDictionary<ResourceType, int>(collectedResources);
        }

        private IEnumerable<BuildingModel> GetStorageBuildings()
        {
            return _gridModel.GetAllBuildings()
                .Where(building => building.Definition.Type == BuildingType.Storage);
        }
    }
}
