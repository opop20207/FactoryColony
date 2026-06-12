using System;
using System.Linq;

namespace FactoryColony
{
    public sealed class PlayerInventoryTransferService
    {
        private readonly PlayerInventoryModel _playerInventory;
        private readonly BaseInventoryModel _baseInventory;

        public PlayerInventoryTransferService(PlayerInventoryModel playerInventory, BaseInventoryModel baseInventory)
        {
            _playerInventory = playerInventory ?? throw new ArgumentNullException(nameof(playerInventory));
            _baseInventory = baseInventory ?? throw new ArgumentNullException(nameof(baseInventory));
        }

        public bool TryTakeFromStorage(BuildingModel storage, ResourceType type, int amount, out string message)
        {
            if (!ValidateStorage(storage, out message))
            {
                return false;
            }

            if (type == ResourceType.None || amount <= 0)
            {
                message = "Invalid resource request.";
                return false;
            }

            if (!storage.Inventory.Has(type, amount))
            {
                message = "Storage does not have enough " + type + ".";
                return false;
            }

            if (_playerInventory.TotalAmount + amount > _playerInventory.MaxTotalAmount)
            {
                message = "Player inventory is full.";
                return false;
            }

            storage.Inventory.TryRemove(type, amount);
            _playerInventory.Add(type, amount);
            message = "Took " + type + " x" + amount + " from " + storage.InstanceId + ".";
            return true;
        }

        public bool TryTakeAnyFromStorage(BuildingModel storage, int maxAmount, out string message)
        {
            if (!ValidateStorage(storage, out message))
            {
                return false;
            }

            if (maxAmount <= 0)
            {
                message = "Invalid take amount.";
                return false;
            }

            ResourceStack stack = storage.Inventory.GetStacks().FirstOrDefault();

            if (stack == null)
            {
                message = "Storage is empty.";
                return false;
            }

            int freeSpace = _playerInventory.MaxTotalAmount - _playerInventory.TotalAmount;
            int amount = Math.Min(maxAmount, Math.Min(stack.Amount, freeSpace));

            if (amount <= 0)
            {
                message = "Player inventory is full.";
                return false;
            }

            return TryTakeFromStorage(storage, stack.Type, amount, out message);
        }

        public bool TryDepositToBase(ResourceType type, int amount, out string message)
        {
            if (type == ResourceType.None || amount <= 0)
            {
                message = "Invalid deposit request.";
                return false;
            }

            if (!_playerInventory.Has(type, amount))
            {
                message = "Player inventory does not have enough " + type + ".";
                return false;
            }

            _playerInventory.TryRemove(type, amount);
            _baseInventory.Add(type, amount);
            message = "Deposited " + type + " x" + amount + " to Base.";
            return true;
        }

        public bool DepositAllToBase(out string message)
        {
            ResourceStack[] stacks = _playerInventory.GetStacks().ToArray();

            if (stacks.Length == 0)
            {
                message = "Player inventory is empty.";
                return false;
            }

            foreach (ResourceStack stack in stacks)
            {
                _playerInventory.TryRemove(stack.Type, stack.Amount);
                _baseInventory.Add(stack.Type, stack.Amount);
            }

            int totalAmount = 0;
            foreach (ResourceStack stack in stacks)
            {
                totalAmount += stack.Amount;
            }

            message = "Deposited " + totalAmount + " resources to Base.";
            return true;
        }

        private static bool ValidateStorage(BuildingModel storage, out string message)
        {
            if (storage == null || storage.Definition.Type != BuildingType.Storage)
            {
                message = "No Storage selected.";
                return false;
            }

            if (storage.Inventory.IsEmpty)
            {
                message = "Storage is empty.";
                return false;
            }

            message = string.Empty;
            return true;
        }
    }
}
