using System;
using System.Collections.Generic;

namespace FactoryColony
{
    public sealed class BaseInventoryModel
    {
        public InventoryModel Inventory { get; }

        public BaseInventoryModel()
        {
            Inventory = new InventoryModel();
        }

        public bool CanAfford(IReadOnlyDictionary<ResourceType, int> cost)
        {
            ValidateCost(cost);

            if (cost == null || cost.Count == 0)
            {
                return true;
            }

            return Inventory.HasAll(cost);
        }

        public bool TrySpend(IReadOnlyDictionary<ResourceType, int> cost)
        {
            ValidateCost(cost);

            if (cost == null || cost.Count == 0)
            {
                return true;
            }

            return Inventory.TryRemoveAll(cost);
        }

        public void Add(ResourceType type, int amount)
        {
            Inventory.Add(type, amount);
        }

        public int GetAmount(ResourceType type)
        {
            return Inventory.GetAmount(type);
        }

        private static void ValidateCost(IReadOnlyDictionary<ResourceType, int> cost)
        {
            if (cost == null)
            {
                return;
            }

            foreach (KeyValuePair<ResourceType, int> item in cost)
            {
                if (item.Key == ResourceType.None)
                {
                    throw new ArgumentException("Cost cannot contain ResourceType.None.", nameof(cost));
                }

                if (item.Value <= 0)
                {
                    throw new ArgumentException("Cost amounts must be greater than zero.", nameof(cost));
                }
            }
        }
    }
}
