using System.Collections.Generic;
using UnityEngine;

namespace FactoryColony
{
    public sealed class GridView : MonoBehaviour
    {
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private float cellSize = 1f;

        private readonly List<GameObject> _cellObjects = new List<GameObject>();

        public GridModel Model { get; private set; }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public float CellSize
        {
            get { return cellSize; }
        }

        public void Build(GridModel model)
        {
            Model = model;
            ClearCells();

            foreach (GridCell cell in model.GetAllCells())
            {
                CreateCell(cell);
            }
        }

        public GridModel CreateDefaultModel()
        {
            return new GridModel(width, height);
        }

        private void CreateCell(GridCell cell)
        {
            GameObject cellObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cellObject.name = $"Cell_{cell.Position.X}_{cell.Position.Y}";
            cellObject.transform.SetParent(transform, false);
            cellObject.transform.localPosition = new Vector3(
                cell.Position.X * cellSize,
                0f,
                cell.Position.Y * cellSize);

            GridCellView cellView = cellObject.AddComponent<GridCellView>();
            cellView.Initialize(cell.Position, cell.ResourceNodeType, cellSize);

            _cellObjects.Add(cellObject);
        }

        private void ClearCells()
        {
            for (int i = _cellObjects.Count - 1; i >= 0; i--)
            {
                GameObject cellObject = _cellObjects[i];

                if (cellObject == null)
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(cellObject);
                }
                else
                {
                    DestroyImmediate(cellObject);
                }
            }

            _cellObjects.Clear();
        }
    }
}
