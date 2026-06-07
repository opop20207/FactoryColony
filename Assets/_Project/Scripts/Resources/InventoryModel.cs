using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryColony
{
    public sealed class InventoryModel
    {
        private readonly Dictionary<ResourceType, int> _items;

        public IReadOnlyDictionary<ResourceType, int> Items
        {
            get { return _items; }
        }

        public bool IsEmpty
        {
            get { return TotalAmount == 0; }
        }

        public int TotalAmount
        {
            get
            {
                int totalAmount = 0;

                foreach (int amount in _items.Values)
                {
                    totalAmount += amount;
                }

                return totalAmount;
            }
        }

        public InventoryModel()
        {
            _items = new Dictionary<ResourceType, int>();
        }

        public void Add(ResourceType type, int amount)
        {
            if (type == ResourceType.None)
            {
                throw new ArgumentException("ResourceType.None cannot be stored in an inventory.", nameof(type));
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            }

            _items[type] = GetAmount(type) + amount;
        }

        public bool TryRemove(ResourceType type, int amount)
        {
            if (type == ResourceType.None || amount <= 0)
            {
                return false;
            }

            if (!Has(type, amount))
            {
                return false;
            }

            int remainingAmount = _items[type] - amount;

            if (remainingAmount == 0)
            {
                _items.Remove(type);
                return true;
            }

            _items[type] = remainingAmount;
            return true;
        }

        public int GetAmount(ResourceType type)
        {
            if (type == ResourceType.None)
            {
                return 0;
            }

            return _items.TryGetValue(type, out int amount) ? amount : 0;
        }

        public bool Has(ResourceType type, int amount)
        {
            if (type == ResourceType.None || amount <= 0)
            {
                return false;
            }

            return GetAmount(type) >= amount;
        }

        public bool HasAll(IReadOnlyDictionary<ResourceType, int> requirements)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            foreach (KeyValuePair<ResourceType, int> requirement in requirements)
            {
                if (requirement.Key == ResourceType.None || requirement.Value <= 0)
                {
                    return false;
                }

                if (!Has(requirement.Key, requirement.Value))
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryRemoveAll(IReadOnlyDictionary<ResourceType, int> requirements)
        {
            if (!HasAll(requirements))
            {
                return false;
            }

            foreach (KeyValuePair<ResourceType, int> requirement in requirements)
            {
                TryRemove(requirement.Key, requirement.Value);
            }

            return true;
        }

        public void AddAll(IReadOnlyDictionary<ResourceType, int> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            foreach (KeyValuePair<ResourceType, int> resource in resources)
            {
                Add(resource.Key, resource.Value);
            }
        }

        public void Clear()
        {
            _items.Clear();
        }

        public int RemoveAll(ResourceType type)
        {
            if (type == ResourceType.None)
            {
                return 0;
            }

            if (!_items.TryGetValue(type, out int amount))
            {
                return 0;
            }

            _items.Remove(type);
            return amount;
        }

        public IReadOnlyDictionary<ResourceType, int> ToDictionary()
        {
            return new Dictionary<ResourceType, int>(_items);
        }

        public IEnumerable<ResourceStack> GetStacks()
        {
            foreach (KeyValuePair<ResourceType, int> item in _items.OrderBy(item => item.Key))
            {
                if (item.Key == ResourceType.None || item.Value <= 0)
                {
                    continue;
                }

                yield return new ResourceStack(item.Key, item.Value);
            }
        }

        public bool TryTakeOne(out ResourceStack stack)
        {
            return TryTakeOne(type => true, out stack);
        }

        public bool TryPeekOne(out ResourceStack stack)
        {
            return TryPeekOne(type => true, out stack);
        }

        public bool TryPeekOne(Func<ResourceType, bool> predicate, out ResourceStack stack)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (ResourceType type in _items.Keys.OrderBy(type => type))
            {
                if (type == ResourceType.None || _items[type] <= 0 || !predicate(type))
                {
                    continue;
                }

                stack = new ResourceStack(type, 1);
                return true;
            }

            stack = null;
            return false;
        }

        public bool TryTakeOne(Func<ResourceType, bool> predicate, out ResourceStack stack)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (ResourceType type in _items.Keys.OrderBy(type => type).ToArray())
            {
                if (type == ResourceType.None || _items[type] <= 0 || !predicate(type))
                {
                    continue;
                }

                TryRemove(type, 1);
                stack = new ResourceStack(type, 1);
                return true;
            }

            stack = null;
            return false;
        }
    }
}
