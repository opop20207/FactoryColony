using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    public sealed class StorageCollectionController : MonoBehaviour
    {
        private StorageCollector _collector;

        public event Action OnStorageCollected;

        public string LastCollectionResult { get; private set; } = "None";

        public void Initialize(StorageCollector collector)
        {
            _collector = collector;
            LastCollectionResult = "Ready";
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.C))
            {
                return;
            }

            CollectAllStorageResources();
        }

        public void CollectAllStorageResources()
        {
            if (_collector == null)
            {
                LastCollectionResult = "Storage collector is not initialized.";
                Debug.LogWarning(LastCollectionResult);
                return;
            }

            IReadOnlyDictionary<ResourceType, int> collectedResources = _collector.CollectAllByType();
            LastCollectionResult = FormatCollectionResult(collectedResources);
            OnStorageCollected?.Invoke();
        }

        private static string FormatCollectionResult(IReadOnlyDictionary<ResourceType, int> collectedResources)
        {
            if (collectedResources == null || collectedResources.Count == 0)
            {
                return "Collected: None";
            }

            string result = "Collected: ";
            bool isFirst = true;

            foreach (KeyValuePair<ResourceType, int> item in collectedResources)
            {
                if (!isFirst)
                {
                    result += ", ";
                }

                result += item.Key + " x" + item.Value;
                isFirst = false;
            }

            return result;
        }
    }
}
