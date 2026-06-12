using System;
using System.Collections.Generic;

namespace FactoryColony
{
    public sealed class PlayerInventoryModel
    {
        public const int DefaultMaxTotalAmount = 100;

        public InventoryModel Inventory { get; }
        public int MaxTotalAmount { get; }

        public bool IsEmpty
        {
            get { return Inventory.IsEmpty; }
        }

        public int TotalAmount
        {
            get { return Inventory.TotalAmount; }
        }

        public PlayerInventoryModel()
            : this(DefaultMaxTotalAmount)
        {
        }

        public PlayerInventoryModel(int maxTotalAmount)
        {
            if (maxTotalAmount <= 0)
            {
                throw new ArgumentException("Max total amount must be greater than zero.", nameof(maxTotalAmount));
            }

            MaxTotalAmount = maxTotalAmount;
            Inventory = new InventoryModel();
        }

        public void Add(ResourceType type, int amount)
        {
            if (!TryAdd(type, amount))
            {
                throw new InvalidOperationException("Player inventory cannot accept the requested resources.");
            }
        }

        public bool TryAdd(ResourceType type, int amount)
        {
            ValidateResource(type, amount);

            if (TotalAmount + amount > MaxTotalAmount)
            {
                return false;
            }

            Inventory.Add(type, amount);
            return true;
        }

        public bool TryRemove(ResourceType type, int amount)
        {
            if (type == ResourceType.None || amount <= 0)
            {
                return false;
            }

            return Inventory.TryRemove(type, amount);
        }

        public int GetAmount(ResourceType type)
        {
            return Inventory.GetAmount(type);
        }

        public bool Has(ResourceType type, int amount)
        {
            return Inventory.Has(type, amount);
        }

        public IEnumerable<ResourceStack> GetStacks()
        {
            return Inventory.GetStacks();
        }

        public void Clear()
        {
            Inventory.Clear();
        }

        private static void ValidateResource(ResourceType type, int amount)
        {
            if (type == ResourceType.None)
            {
                throw new ArgumentException("ResourceType.None cannot be stored in player inventory.", nameof(type));
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            }
        }
    }
}
