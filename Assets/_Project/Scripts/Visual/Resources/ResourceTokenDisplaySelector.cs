using System.Collections.Generic;

namespace FactoryColony
{
    public static class ResourceTokenDisplaySelector
    {
        public const int DefaultMaxTokenCount = 4;

        public static IReadOnlyList<ResourceTokenDisplayData> SelectTokens(
            IEnumerable<ResourceStack> stacks,
            int maxTokenCount = DefaultMaxTokenCount)
        {
            List<ResourceTokenDisplayData> tokens = new List<ResourceTokenDisplayData>();

            if (stacks == null || maxTokenCount <= 0)
            {
                return tokens;
            }

            foreach (ResourceStack stack in stacks)
            {
                if (stack == null || stack.Type == ResourceType.None || stack.Amount <= 0)
                {
                    continue;
                }

                tokens.Add(new ResourceTokenDisplayData(stack.Type, stack.Amount));

                if (tokens.Count >= maxTokenCount)
                {
                    break;
                }
            }

            return tokens;
        }
    }
}
