using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FactoryColony
{
    public sealed class GoalTracker
    {
        private readonly BaseInventoryModel _baseInventory;
        private readonly List<ProductionGoalModel> _goals;
        private readonly ReadOnlyCollection<ProductionGoalModel> _readOnlyGoals;

        public IReadOnlyList<ProductionGoalModel> Goals
        {
            get { return _readOnlyGoals; }
        }

        public bool AllCompleted
        {
            get
            {
                if (_goals.Count == 0)
                {
                    return false;
                }

                foreach (ProductionGoalModel goal in _goals)
                {
                    if (!goal.IsCompleted)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public GoalTracker(BaseInventoryModel baseInventory, IReadOnlyList<ProductionGoalModel> goals)
        {
            _baseInventory = baseInventory ?? throw new ArgumentNullException(nameof(baseInventory));

            if (goals == null)
            {
                throw new ArgumentNullException(nameof(goals));
            }

            _goals = new List<ProductionGoalModel>(goals.Count);

            foreach (ProductionGoalModel goal in goals)
            {
                if (goal == null)
                {
                    throw new ArgumentException("Goal list cannot contain null.", nameof(goals));
                }

                _goals.Add(goal);
            }

            _readOnlyGoals = new ReadOnlyCollection<ProductionGoalModel>(_goals);
            UpdateGoals();
        }

        public void UpdateGoals()
        {
            foreach (ProductionGoalModel goal in _goals)
            {
                goal.UpdateProgress(_baseInventory.GetAmount(goal.ResourceType));
            }
        }

        public ProductionGoalModel GetGoal(string id)
        {
            if (TryGetGoal(id, out ProductionGoalModel goal))
            {
                return goal;
            }

            throw new KeyNotFoundException("Goal not found: " + id);
        }

        public bool TryGetGoal(string id, out ProductionGoalModel goal)
        {
            foreach (ProductionGoalModel candidate in _goals)
            {
                if (candidate.Id == id)
                {
                    goal = candidate;
                    return true;
                }
            }

            goal = null;
            return false;
        }
    }
}
