using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    public sealed class BuildingInventoryVisualView : MonoBehaviour
    {
        private const float TokenSpacing = 0.22f;

        private readonly List<GameObject> _tokenObjects = new List<GameObject>();

        private BuildingModel _building;
        private float _cellSize = 1f;
        private ConveyorResourceFlowView _conveyorFlowView;

        public void Initialize(BuildingModel building, float cellSize)
        {
            _building = building;
            _cellSize = cellSize > 0f ? cellSize : 1f;
            gameObject.name = "InventoryVisualRoot";
            Refresh();
        }

        public void Refresh()
        {
            ClearTokens();

            if (_building == null || _building.Inventory == null)
            {
                return;
            }

            IReadOnlyList<ResourceTokenDisplayData> tokens =
                ResourceTokenDisplaySelector.SelectTokens(_building.Inventory.GetStacks(), GetMaxTokenCount());

            for (int i = 0; i < tokens.Count; i++)
            {
                CreateToken(tokens[i], i, tokens.Count);
            }

            UpdateConveyorFlow();
        }

        private void CreateToken(ResourceTokenDisplayData token, int index, int totalCount)
        {
            GameObject tokenObject = new GameObject("ResourceTokenAnchor");
            tokenObject.transform.SetParent(transform, false);
            tokenObject.transform.localPosition = IsConveyor() ? Vector3.up * VisualStyleConfig.ConveyorTokenHeight : GetTokenPosition(index, totalCount);

            ResourceTokenView tokenView = tokenObject.AddComponent<ResourceTokenView>();
            tokenView.Initialize(token.Type, token.Amount);
            _tokenObjects.Add(tokenObject);
        }

        private void UpdateConveyorFlow()
        {
            if (!IsConveyor())
            {
                if (_conveyorFlowView != null)
                {
                    _conveyorFlowView.SetTokens(null);
                    _conveyorFlowView.enabled = false;
                }

                return;
            }

            if (_conveyorFlowView == null)
            {
                _conveyorFlowView = gameObject.GetComponent<ConveyorResourceFlowView>();
            }

            if (_conveyorFlowView == null)
            {
                _conveyorFlowView = gameObject.AddComponent<ConveyorResourceFlowView>();
            }

            List<ResourceTokenView> tokenViews = new List<ResourceTokenView>();
            foreach (GameObject tokenObject in _tokenObjects)
            {
                if (tokenObject == null)
                {
                    continue;
                }

                ResourceTokenView tokenView = tokenObject.GetComponent<ResourceTokenView>();
                if (tokenView != null)
                {
                    tokenViews.Add(tokenView);
                }
            }

            _conveyorFlowView.Initialize(_building, _cellSize);
            _conveyorFlowView.SetTokens(tokenViews.ToArray());
        }

        private Vector3 GetTokenPosition(int index, int totalCount)
        {
            float startX = (totalCount - 1) * TokenSpacing * -0.5f;
            return new Vector3(startX + index * TokenSpacing, GetHeightOffset(), 0f);
        }

        private float GetHeightOffset()
        {
            switch (_building.Definition.Type)
            {
                case BuildingType.Conveyor:
                    return 0.38f;
                case BuildingType.Storage:
                    return 0.88f;
                case BuildingType.Smelter:
                case BuildingType.Assembler:
                    return 1.04f;
                case BuildingType.Miner:
                    return 0.82f;
                default:
                    return 0.78f;
            }
        }

        private int GetMaxTokenCount()
        {
            return IsConveyor() ? VisualStyleConfig.ConveyorTokenMaxCount : ResourceTokenDisplaySelector.DefaultMaxTokenCount;
        }

        private bool IsConveyor()
        {
            return _building != null && _building.Definition.Type == BuildingType.Conveyor;
        }

        private void ClearTokens()
        {
            for (int i = _tokenObjects.Count - 1; i >= 0; i--)
            {
                GameObject tokenObject = _tokenObjects[i];

                if (tokenObject == null)
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(tokenObject);
                }
                else
                {
                    DestroyImmediate(tokenObject);
                }
            }

            _tokenObjects.Clear();
        }
    }
}
