using System.Collections.Generic;

namespace FactoryColony
{
    public static class ResourceTextFormatter
    {
        public static string FormatCost(IReadOnlyDictionary<ResourceType, int> cost)
        {
            if (cost == null || cost.Count == 0)
            {
                return "Free";
            }

            string result = string.Empty;

            foreach (KeyValuePair<ResourceType, int> item in cost)
            {
                if (item.Key == ResourceType.None || item.Value <= 0)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(result))
                {
                    result += ", ";
                }

                result += item.Key + " x" + item.Value;
            }

            return string.IsNullOrEmpty(result) ? "Free" : result;
        }
    }
}
