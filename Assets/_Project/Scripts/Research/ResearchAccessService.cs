using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryColony
{
    public sealed class ResearchAccessService
    {
        private readonly GridModel _gridModel;
        private readonly bool _allowResearchWithoutLab;

        public ResearchAccessService(GridModel gridModel)
            : this(gridModel, false)
        {
        }

        public ResearchAccessService(GridModel gridModel, bool allowResearchWithoutLab)
        {
            _gridModel = gridModel ?? throw new ArgumentNullException(nameof(gridModel));
            _allowResearchWithoutLab = allowResearchWithoutLab;
        }

        public bool HasResearchLab()
        {
            return GetResearchLabs().Any();
        }

        public IReadOnlyList<BuildingModel> GetResearchLabs()
        {
            return _gridModel.GetAllBuildings()
                .Where(building => building.Definition.Type == BuildingType.ResearchLab)
                .OrderBy(building => building.Origin.X)
                .ThenBy(building => building.Origin.Y)
                .ToArray();
        }

        public bool CanOpenResearch(out string message)
        {
            if (_allowResearchWithoutLab || HasResearchLab())
            {
                message = "Research available.";
                return true;
            }

            message = "Build Research Lab first.";
            return false;
        }
    }
}
