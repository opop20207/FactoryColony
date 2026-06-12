using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FactoryColony
{
    public sealed class ResearchPanelController : MonoBehaviour
    {
        private ResearchPanelView _view;
        private ResearchSystem _researchSystem;
        private BuildMenuController _buildMenuController;
        private ResearchAccessService _accessService;

        public string LastResearchResult { get; private set; } = "None";

        public void Initialize(
            ResearchPanelView view,
            ResearchSystem researchSystem,
            BuildMenuController buildMenuController)
        {
            Initialize(view, researchSystem, buildMenuController, null);
        }

        public void Initialize(
            ResearchPanelView view,
            ResearchSystem researchSystem,
            BuildMenuController buildMenuController,
            ResearchAccessService accessService)
        {
            if (_view != null)
            {
                _view.OnResearchRequested -= HandleResearchRequested;
            }

            _view = view;
            _researchSystem = researchSystem;
            _buildMenuController = buildMenuController;
            _accessService = accessService;

            if (_view == null)
            {
                return;
            }

            _view.Initialize(_researchSystem);
            _view.OnResearchRequested += HandleResearchRequested;
            _view.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_view != null)
            {
                _view.OnResearchRequested -= HandleResearchRequested;
            }
        }

        private void Update()
        {
            if (!PlayerInputReader.WasKeyPressedThisFrame(Key.T) || IsPointerOverUi())
            {
                return;
            }

            if (_view != null)
            {
                ToggleResearchPanel();
            }
        }

        private void HandleResearchRequested(string researchId)
        {
            if (!CanOpenResearch())
            {
                return;
            }

            if (_researchSystem == null)
            {
                LastResearchResult = "Research system unavailable.";
                return;
            }

            _researchSystem.TryResearch(researchId, out string message);
            LastResearchResult = message;

            if (_view != null)
            {
                _view.SetMessage(message);
                _view.Refresh();
            }

            if (_buildMenuController != null)
            {
                _buildMenuController.Refresh();
            }
        }

        private void ToggleResearchPanel()
        {
            if (_view.gameObject.activeSelf)
            {
                _view.gameObject.SetActive(false);
                return;
            }

            if (!CanOpenResearch())
            {
                _view.gameObject.SetActive(true);
                _view.ShowMessageOnly(LastResearchResult);
                return;
            }

            _view.gameObject.SetActive(true);
            _view.Refresh();
        }

        private bool CanOpenResearch()
        {
            if (_accessService == null)
            {
                return true;
            }

            bool canOpen = _accessService.CanOpenResearch(out string message);

            if (!canOpen)
            {
                LastResearchResult = message;
            }

            return canOpen;
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
