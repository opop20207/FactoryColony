using System.Collections.Generic;
using System.Linq;

namespace FactoryColony
{
    public sealed class ResearchSystem
    {
        private readonly IReadOnlyDictionary<string, ResearchDefinition> _definitions;
        private readonly ResearchStateModel _state;
        private readonly BaseInventoryModel _baseInventory;

        public ResearchStateModel State
        {
            get { return _state; }
        }

        public int CompletedCount
        {
            get { return _state.GetCompletedResearchIds().Count; }
        }

        public ResearchSystem(
            IReadOnlyDictionary<string, ResearchDefinition> researchDefinitions,
            ResearchStateModel researchState,
            BaseInventoryModel baseInventory)
        {
            _definitions = researchDefinitions ?? new Dictionary<string, ResearchDefinition>();
            _state = researchState ?? new ResearchStateModel();
            _baseInventory = baseInventory;
        }

        public bool CanResearch(string researchId)
        {
            return TryGetDefinition(researchId, out ResearchDefinition definition)
                && !_state.IsCompleted(researchId)
                && ArePrerequisitesCompleted(definition)
                && (_baseInventory == null || _baseInventory.CanAfford(definition.Cost));
        }

        public bool TryResearch(string researchId, out string resultMessage)
        {
            if (!TryGetDefinition(researchId, out ResearchDefinition definition))
            {
                resultMessage = "Unknown research.";
                return false;
            }

            if (_state.IsCompleted(researchId))
            {
                resultMessage = "Already completed.";
                return false;
            }

            if (!ArePrerequisitesCompleted(definition))
            {
                resultMessage = "Prerequisite missing.";
                return false;
            }

            if (_baseInventory != null && !_baseInventory.TrySpend(definition.Cost))
            {
                resultMessage = "Insufficient resources.";
                return false;
            }

            _state.Complete(researchId);
            resultMessage = "Completed " + definition.DisplayName + ".";
            return true;
        }

        public IReadOnlyList<ResearchDefinition> GetAvailableResearch()
        {
            return _definitions.Values
                .Where(definition => !_state.IsCompleted(definition.Id) && ArePrerequisitesCompleted(definition))
                .OrderBy(definition => definition.Id)
                .ToArray();
        }

        public IReadOnlyList<ResearchDefinition> GetCompletedResearch()
        {
            return _definitions.Values
                .Where(definition => _state.IsCompleted(definition.Id))
                .OrderBy(definition => definition.Id)
                .ToArray();
        }

        public IReadOnlyList<ResearchDefinition> GetLockedResearch()
        {
            return _definitions.Values
                .Where(definition => !_state.IsCompleted(definition.Id) && !ArePrerequisitesCompleted(definition))
                .OrderBy(definition => definition.Id)
                .ToArray();
        }

        public bool ArePrerequisitesCompleted(ResearchDefinition definition)
        {
            if (definition == null)
            {
                return false;
            }

            return definition.PrerequisiteResearchIds.All(_state.IsCompleted);
        }

        public bool IsBuildingUnlocked(string buildingDefinitionId)
        {
            return IsUnlocked(buildingDefinitionId, definition => definition.UnlockBuildingDefinitionIds);
        }

        public bool IsRecipeUnlocked(string recipeId)
        {
            return IsUnlocked(recipeId, definition => definition.UnlockRecipeIds);
        }

        private bool IsUnlocked(string unlockId, System.Func<ResearchDefinition, IReadOnlyList<string>> selector)
        {
            if (string.IsNullOrEmpty(unlockId))
            {
                return true;
            }

            bool isLockedByResearch = false;

            foreach (ResearchDefinition definition in _definitions.Values)
            {
                if (!selector(definition).Contains(unlockId))
                {
                    continue;
                }

                isLockedByResearch = true;

                if (_state.IsCompleted(definition.Id))
                {
                    return true;
                }
            }

            return !isLockedByResearch;
        }

        private bool TryGetDefinition(string researchId, out ResearchDefinition definition)
        {
            if (string.IsNullOrEmpty(researchId))
            {
                definition = null;
                return false;
            }

            return _definitions.TryGetValue(researchId, out definition);
        }
    }
}
