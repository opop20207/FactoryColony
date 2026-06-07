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

        public static string FormatInventory(InventoryModel inventory)
        {
            if (inventory == null || inventory.IsEmpty)
            {
                return "Empty";
            }

            return FormatStacks(inventory.GetStacks());
        }

        public static string FormatStacks(IEnumerable<ResourceStack> stacks)
        {
            if (stacks == null)
            {
                return "Empty";
            }

            string result = string.Empty;

            foreach (ResourceStack stack in stacks)
            {
                if (stack == null || stack.Type == ResourceType.None || stack.Amount <= 0)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(result))
                {
                    result += ", ";
                }

                result += stack.Type + " x" + stack.Amount;
            }

            return string.IsNullOrEmpty(result) ? "Empty" : result;
        }
    }
}
