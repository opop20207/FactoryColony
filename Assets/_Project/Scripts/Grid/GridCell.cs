namespace FactoryColony
{
    public sealed class GridCell
    {
        public GridPosition Position { get; }
        public bool IsBuildable { get; set; }
        public ResourceType ResourceNodeType { get; set; }
        public string OccupiedByBuildingId { get; set; }

        public bool HasBuilding
        {
            get { return !string.IsNullOrEmpty(OccupiedByBuildingId); }
        }

        public GridCell(GridPosition position)
        {
            Position = position;
            IsBuildable = true;
            ResourceNodeType = ResourceType.None;
        }
    }
}
