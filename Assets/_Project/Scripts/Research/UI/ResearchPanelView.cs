using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FactoryColony
{
    public sealed class ResearchPanelView : MonoBehaviour
    {
        private const float PanelWidth = 360f;
        private const float PanelHeight = 330f;

        private readonly List<GameObject> _rows = new List<GameObject>();
        private ResearchSystem _researchSystem;
        private Transform _rowRoot;
        private Text _messageText;
        private Font _font;

        public event Action<string> OnResearchRequested;

        public void Initialize(ResearchSystem researchSystem)
        {
            _researchSystem = researchSystem;
            EnsureLayout();
            Refresh();
        }

        public void SetMessage(string message)
        {
            EnsureLayout();
            _messageText.text = string.IsNullOrEmpty(message) ? "Ready" : message;
        }

        public void ShowMessageOnly(string message)
        {
            EnsureLayout();
            ClearRows();
            SetMessage(message);
        }

        public void Refresh()
        {
            EnsureLayout();
            ClearRows();

            if (_researchSystem == null)
            {
                _messageText.text = "Research unavailable.";
                return;
            }

            foreach (ResearchDefinition definition in _researchSystem.GetCompletedResearch())
            {
                CreateRow(definition, "Completed", false);
            }

            foreach (ResearchDefinition definition in _researchSystem.GetAvailableResearch())
            {
                CreateRow(definition, "Available", _researchSystem.CanResearch(definition.Id));
            }

            foreach (ResearchDefinition definition in _researchSystem.GetLockedResearch())
            {
                CreateRow(definition, "Locked", false);
            }
        }

        private void EnsureLayout()
        {
            _font = _font != null ? _font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = gameObject.AddComponent<RectTransform>();
            }

            rectTransform.anchorMin = new Vector2(1f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 0f);
            rectTransform.pivot = new Vector2(1f, 0f);
            rectTransform.anchoredPosition = new Vector2(-12f, 12f);
            rectTransform.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image background = GetComponent<Image>();
            if (background == null)
            {
                background = gameObject.AddComponent<Image>();
            }

            background.color = new Color(0.06f, 0.08f, 0.09f, 0.88f);

            VerticalLayoutGroup layout = GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.spacing = 5f;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            _rowRoot = transform;

            if (_messageText == null)
            {
                _messageText = CreateText("ResearchMessage", "Ready", 12);
                _messageText.transform.SetParent(transform, false);
            }
        }

        private void CreateRow(ResearchDefinition definition, string state, bool canResearch)
        {
            GameObject row = new GameObject("Research_" + definition.Id, typeof(RectTransform), typeof(Image));
            row.transform.SetParent(_rowRoot, false);
            _rows.Add(row);

            Image image = row.GetComponent<Image>();
            image.color = new Color(0.16f, 0.19f, 0.2f, 0.95f);

            VerticalLayoutGroup layout = row.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(6, 6, 4, 4);
            layout.spacing = 3f;

            LayoutElement element = row.AddComponent<LayoutElement>();
            element.preferredHeight = 72f;

            Text label = CreateText("Label", definition.DisplayName + " [" + state + "]", 13);
            label.transform.SetParent(row.transform, false);
            Text detail = CreateText("Detail", "Cost: " + ResourceTextFormatter.FormatCost(definition.Cost), 11);
            detail.transform.SetParent(row.transform, false);

            Button button = row.AddComponent<Button>();
            button.interactable = canResearch;
            button.onClick.AddListener(() => OnResearchRequested?.Invoke(definition.Id));
        }

        private Text CreateText(string name, string value, int fontSize)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            Text text = textObject.GetComponent<Text>();
            text.font = _font;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.text = value;
            text.alignment = TextAnchor.MiddleLeft;
            return text;
        }

        private void ClearRows()
        {
            foreach (GameObject row in _rows)
            {
                if (row != null)
                {
                    Destroy(row);
                }
            }

            _rows.Clear();
        }
    }
}
