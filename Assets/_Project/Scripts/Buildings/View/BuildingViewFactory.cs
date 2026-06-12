using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    public sealed class BuildingViewFactory : MonoBehaviour
    {
        [SerializeField] private Transform buildingsRoot;

        private readonly List<GameObject> _buildingObjects = new List<GameObject>();
        private readonly List<BuildingView> _buildingViews = new List<BuildingView>();

        public IReadOnlyList<BuildingView> BuildingViews
        {
            get { return _buildingViews; }
        }

        public void Build(GridModel model, float cellSize)
        {
            EnsureBuildingsRoot();
            ClearBuildings();

            foreach (BuildingModel building in model.GetAllBuildings())
            {
                CreateBuildingView(building, cellSize);
            }
        }

        private void EnsureBuildingsRoot()
        {
            if (buildingsRoot != null)
            {
                return;
            }

            Transform existingRoot = transform.Find("BuildingsRoot");
            if (existingRoot != null)
            {
                buildingsRoot = existingRoot;
                return;
            }

            GameObject rootObject = new GameObject("BuildingsRoot");
            rootObject.transform.SetParent(transform, false);
            buildingsRoot = rootObject.transform;
        }

        private void CreateBuildingView(BuildingModel building, float cellSize)
        {
            GameObject buildingObject = new GameObject($"Building_{building.InstanceId}");
            buildingObject.transform.SetParent(buildingsRoot, false);

            BuildingView buildingView = buildingObject.AddComponent<BuildingView>();
            buildingView.Initialize(building, cellSize);

            _buildingObjects.Add(buildingObject);
            _buildingViews.Add(buildingView);
        }

        public void RefreshInventoryVisuals()
        {
            foreach (BuildingView buildingView in _buildingViews)
            {
                if (buildingView != null)
                {
                    buildingView.RefreshInventoryVisual();
                }
            }
        }

        private void ClearBuildings()
        {
            for (int i = buildingsRoot.childCount - 1; i >= 0; i--)
            {
                DestroyObject(buildingsRoot.GetChild(i).gameObject);
            }

            _buildingObjects.Clear();
            _buildingViews.Clear();
        }

        private static void DestroyObject(GameObject target)
        {
            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
        }
    }
}
