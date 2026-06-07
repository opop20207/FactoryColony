using System;

namespace FactoryColony
{
    public sealed class ProductionGoalModel
    {
        public string Id { get; }
        public string DisplayName { get; }
        public ResourceType ResourceType { get; }
        public int RequiredAmount { get; }
        public int CurrentAmount { get; private set; }
        public bool IsCompleted { get; private set; }
        public float ProgressRatio
        {
            get
            {
                if (RequiredAmount <= 0)
                {
                    return 0f;
                }

                float ratio = (float)CurrentAmount / RequiredAmount;

                if (ratio < 0f)
                {
                    return 0f;
                }

                return ratio > 1f ? 1f : ratio;
            }
        }

        public ProductionGoalModel(
            string id,
            string displayName,
            ResourceType resourceType,
            int requiredAmount)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Goal id cannot be null or empty.", nameof(id));
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentException("Goal display name cannot be null or empty.", nameof(displayName));
            }

            if (resourceType == ResourceType.None)
            {
                throw new ArgumentException("Goal resource type cannot be None.", nameof(resourceType));
            }

            if (requiredAmount <= 0)
            {
                throw new ArgumentException("Goal required amount must be greater than zero.", nameof(requiredAmount));
            }

            Id = id;
            DisplayName = displayName;
            ResourceType = resourceType;
            RequiredAmount = requiredAmount;
        }

        public void UpdateProgress(int currentAmount)
        {
            CurrentAmount = currentAmount < 0 ? 0 : currentAmount;
            IsCompleted = CurrentAmount >= RequiredAmount;
        }
    }
}
