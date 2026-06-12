using System.Collections.Generic;
using System.Linq;

namespace FactoryColony
{
    public sealed class ResearchStateModel
    {
        private readonly HashSet<string> _completedResearchIds = new HashSet<string>();

        public bool IsCompleted(string researchId)
        {
            return !string.IsNullOrEmpty(researchId) && _completedResearchIds.Contains(researchId);
        }

        public void Complete(string researchId)
        {
            if (string.IsNullOrEmpty(researchId))
            {
                return;
            }

            _completedResearchIds.Add(researchId);
        }

        public bool TryComplete(string researchId)
        {
            return !string.IsNullOrEmpty(researchId) && _completedResearchIds.Add(researchId);
        }

        public IReadOnlyList<string> GetCompletedResearchIds()
        {
            return _completedResearchIds.OrderBy(id => id).ToArray();
        }

        public void Clear()
        {
            _completedResearchIds.Clear();
        }

        public void RestoreCompleted(IEnumerable<string> ids)
        {
            Clear();

            if (ids == null)
            {
                return;
            }

            foreach (string id in ids)
            {
                Complete(id);
            }
        }
    }
}
