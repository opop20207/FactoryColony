using System;

namespace FactoryColony
{
    public sealed class ResourceStack
    {
        public ResourceType Type { get; }
        public int Amount { get; private set; }

        public bool IsEmpty
        {
            get { return Amount == 0; }
        }

        public ResourceStack(ResourceType type, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount must not be negative.", nameof(amount));
            }

            if (type == ResourceType.None && amount > 0)
            {
                throw new ArgumentException("ResourceType.None cannot have a positive amount.", nameof(type));
            }

            Type = type;
            Amount = amount;
        }

        public void Add(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            }

            if (Type == ResourceType.None)
            {
                throw new InvalidOperationException("Cannot add resources to a ResourceType.None stack.");
            }

            Amount += amount;
        }

        public bool Remove(int amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            if (Amount < amount)
            {
                return false;
            }

            Amount -= amount;
            return true;
        }

        public bool CanMerge(ResourceStack other)
        {
            return other != null
                && Type == other.Type
                && Type != ResourceType.None;
        }

        public void Merge(ResourceStack other)
        {
            if (!CanMerge(other))
            {
                throw new InvalidOperationException("Only stacks with the same non-empty resource type can be merged.");
            }

            if (other.Amount == 0)
            {
                return;
            }

            Add(other.Amount);
            other.Amount = 0;
        }
    }
}
