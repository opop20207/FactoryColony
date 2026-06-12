using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FactoryColony
{
    public sealed class RecipeSelectionPanelView : MonoBehaviour
    {
        private const float PanelWidth = 360f;
        private const float PanelHeight = 260f;

        private readonly List<GameObject> _rows = new List<GameObject>();
        private RecipeSelectionService _service;
        private BuildingModel _building;
        private Transform _rowRoot;
        private Text _titleText;
        private Text _messageText;
        private Font _font;

        public event Action<string> OnRecipeSelected;

        public void Initialize(RecipeSelectionService service)
        {
            _service = service;
            EnsureLayout();
            Refresh();
        }

        public void Show(BuildingModel building)
        {
            _building = building;
            gameObject.SetActive(true);
            Refresh();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetMessage(string message)
        {
            EnsureLayout();
            _messageText.text = string.IsNullOrEmpty(message) ? "Ready" : message;
        }

        public void Refresh()
        {
            EnsureLayout();
            ClearRows();

            if (_building == null)
            {
                _titleText.text = "Recipe Panel";
                _messageText.text = "No building selected.";
                return;
            }

            _titleText.text = _building.Definition.DisplayName;

            if (_service == null)
            {
                _messageText.text = "Recipe service unavailable.";
                return;
            }

            RecipeModel selectedRecipe = _service.GetSelectedRecipe(_building);
            _messageText.text = "Current: " + (selectedRecipe != null ? selectedRecipe.DisplayName : "None");

            IReadOnlyList<RecipeModel> recipes = _service.GetAvailableRecipesFor(_building);

            if (recipes.Count == 0)
            {
                CreateTextRow("No available recipes");
                return;
            }

            foreach (RecipeModel recipe in recipes)
            {
                CreateRecipeRow(recipe);
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

            rectTransform.anchorMin = new Vector2(1f, 0.5f);
            rectTransform.anchorMax = new Vector2(1f, 0.5f);
            rectTransform.pivot = new Vector2(1f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(-12f, -72f);
            rectTransform.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image background = GetComponent<Image>();
            if (background == null)
            {
                background = gameObject.AddComponent<Image>();
            }

            background.color = new Color(0.06f, 0.08f, 0.09f, 0.9f);

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

            if (_titleText == null)
            {
                _titleText = CreateText("RecipeTitle", "Recipe Panel", 14);
                _titleText.fontStyle = FontStyle.Bold;
                _titleText.transform.SetParent(transform, false);
            }

            if (_messageText == null)
            {
                _messageText = CreateText("RecipeMessage", "Ready", 12);
                _messageText.transform.SetParent(transform, false);
            }
        }

        private void CreateRecipeRow(RecipeModel recipe)
        {
            GameObject row = new GameObject("Recipe_" + recipe.Id, typeof(RectTransform), typeof(Image));
            row.transform.SetParent(_rowRoot, false);
            _rows.Add(row);

            Image image = row.GetComponent<Image>();
            image.color = new Color(0.16f, 0.19f, 0.2f, 0.95f);

            VerticalLayoutGroup layout = row.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(6, 6, 4, 4);
            layout.spacing = 2f;

            LayoutElement element = row.AddComponent<LayoutElement>();
            element.preferredHeight = 58f;

            Text label = CreateText("Label", recipe.DisplayName, 13);
            label.transform.SetParent(row.transform, false);

            Text detail = CreateText(
                "Detail",
                "In: " + ResourceTextFormatter.FormatCost(recipe.Inputs)
                + " | Out: " + ResourceTextFormatter.FormatCost(recipe.Outputs),
                11);
            detail.transform.SetParent(row.transform, false);

            Button button = row.AddComponent<Button>();
            button.onClick.AddListener(() => OnRecipeSelected?.Invoke(recipe.Id));
        }

        private void CreateTextRow(string text)
        {
            Text row = CreateText("RecipeInfo", text, 12);
            row.transform.SetParent(_rowRoot, false);
            _rows.Add(row.gameObject);
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
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
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
