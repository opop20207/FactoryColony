using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FactoryColony
{
    public sealed class BuildMenuView : MonoBehaviour
    {
        private const float PanelWidth = 260f;
        private const float PanelHeight = 310f;
        private const float ButtonHeight = 34f;

        private readonly List<ButtonEntry> _buttonEntries = new List<ButtonEntry>();

        private RectTransform _rectTransform;
        private Transform _buttonRoot;
        private Font _font;
        private BuildingDefinition _selectedDefinition;

        public event Action<BuildingDefinition> OnBuildingSelected;
        public event Action OnSelectionCleared;

        public void Initialize(IReadOnlyList<BuildingDefinition> definitions)
        {
            EnsureLayout();
            ClearButtons();

            if (definitions == null)
            {
                return;
            }

            foreach (BuildingDefinition definition in definitions)
            {
                CreateBuildingButton(definition);
            }

            CreateClearButton();
            RefreshSelectionLabels();
        }

        public void SetSelectedDefinition(BuildingDefinition definition)
        {
            _selectedDefinition = definition;
            RefreshSelectionLabels();
        }

        public void ClearSelectionState()
        {
            _selectedDefinition = null;
            RefreshSelectionLabels();
        }

        private void EnsureLayout()
        {
            _font = _font != null ? _font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _rectTransform = GetComponent<RectTransform>();

            if (_rectTransform == null)
            {
                _rectTransform = gameObject.AddComponent<RectTransform>();
            }

            _rectTransform.anchorMin = new Vector2(0f, 0f);
            _rectTransform.anchorMax = new Vector2(0f, 0f);
            _rectTransform.pivot = new Vector2(0f, 0f);
            _rectTransform.anchoredPosition = new Vector2(12f, 12f);
            _rectTransform.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image background = GetComponent<Image>();

            if (background == null)
            {
                background = gameObject.AddComponent<Image>();
            }

            background.color = new Color(0.05f, 0.07f, 0.08f, 0.82f);

            VerticalLayoutGroup layoutGroup = GetComponent<VerticalLayoutGroup>();

            if (layoutGroup == null)
            {
                layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layoutGroup.padding = new RectOffset(8, 8, 8, 8);
            layoutGroup.spacing = 5f;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;
            _buttonRoot = transform;
        }

        private void CreateBuildingButton(BuildingDefinition definition)
        {
            Button button = CreateButtonObject(definition.Type.ToString());
            Text label = button.GetComponentInChildren<Text>();
            ButtonEntry entry = new ButtonEntry(definition, button, label);
            _buttonEntries.Add(entry);

            button.onClick.AddListener(() =>
            {
                _selectedDefinition = definition;
                RefreshSelectionLabels();
                OnBuildingSelected?.Invoke(definition);
            });
        }

        private void CreateClearButton()
        {
            Button button = CreateButtonObject("ClearSelection");
            Text label = button.GetComponentInChildren<Text>();
            label.text = "Clear Selection";
            button.onClick.AddListener(() =>
            {
                ClearSelectionState();
                OnSelectionCleared?.Invoke();
            });
        }

        private Button CreateButtonObject(string name)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(_buttonRoot, false);

            RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(PanelWidth - 16f, ButtonHeight);

            LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = ButtonHeight;

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.18f, 0.22f, 0.24f, 0.95f);

            Button button = buttonObject.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = new Color(0.26f, 0.32f, 0.35f, 0.95f);
            colors.pressedColor = new Color(0.12f, 0.16f, 0.18f, 0.95f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;

            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(buttonObject.transform, false);

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 0f);
            textRect.offsetMax = new Vector2(-8f, 0f);

            Text text = textObject.GetComponent<Text>();
            text.font = _font;
            text.fontSize = 13;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = Color.white;

            return button;
        }

        private void RefreshSelectionLabels()
        {
            foreach (ButtonEntry entry in _buttonEntries)
            {
                string prefix = entry.Definition == _selectedDefinition ? "> " : string.Empty;
                entry.Label.text = prefix + entry.Definition.DisplayName + " / " + ResourceTextFormatter.FormatCost(entry.Definition.BuildCost);
            }
        }

        private void ClearButtons()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            _buttonEntries.Clear();
        }

        private sealed class ButtonEntry
        {
            public BuildingDefinition Definition { get; }
            public Text Label { get; }

            public ButtonEntry(BuildingDefinition definition, Button button, Text label)
            {
                Definition = definition;
                Label = label;
            }
        }
    }
}
