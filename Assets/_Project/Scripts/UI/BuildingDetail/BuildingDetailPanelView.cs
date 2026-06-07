using UnityEngine;
using UnityEngine.UI;

namespace FactoryColony
{
    public sealed class BuildingDetailPanelView : MonoBehaviour
    {
        private Text _titleText;
        private Text _bodyText;
        private Image _background;

        public void ShowBuilding(BuildingModel building)
        {
            EnsureLayout();

            if (building == null)
            {
                ShowEmpty();
                return;
            }

            _titleText.text = building.Definition.DisplayName;
            _bodyText.text =
                "InstanceId: " + building.InstanceId + "\n"
                + "BuildingType: " + building.Definition.Type + "\n"
                + "Direction: " + building.Direction + "\n"
                + "Size: " + GetSizeText(building) + "\n"
                + "Power Production: " + building.Definition.PowerProduction + "\n"
                + "Power Consumption: " + building.Definition.PowerConsumption + "\n"
                + "Inventory: " + ResourceTextFormatter.FormatInventory(building.Inventory) + "\n"
                + "Selected Recipe: " + GetSelectedRecipeText(building) + "\n"
                + "Can Store Resources: " + building.CanStoreResources + "\n"
                + "Can Produce Resources: " + building.CanProduceResources;

            gameObject.SetActive(true);
        }

        public void ShowEmpty()
        {
            EnsureLayout();

            _titleText.text = "Building Detail";
            _bodyText.text = "No Building Selected";
            gameObject.SetActive(true);
        }

        private void Awake()
        {
            EnsureLayout();
            ShowEmpty();
        }

        private void EnsureLayout()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = gameObject.AddComponent<RectTransform>();
            }

            rectTransform.anchorMin = new Vector2(1f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(1f, 1f);
            rectTransform.anchoredPosition = new Vector2(-12f, -12f);
            rectTransform.sizeDelta = new Vector2(340f, 330f);

            if (_background == null)
            {
                _background = GetComponent<Image>();
                if (_background == null)
                {
                    _background = gameObject.AddComponent<Image>();
                }

                _background.color = new Color(0.05f, 0.06f, 0.07f, 0.88f);
            }

            VerticalLayoutGroup layout = GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = gameObject.AddComponent<VerticalLayoutGroup>();
                layout.padding = new RectOffset(12, 12, 12, 12);
                layout.spacing = 8f;
                layout.childControlWidth = true;
                layout.childControlHeight = false;
                layout.childForceExpandWidth = true;
                layout.childForceExpandHeight = false;
            }

            _titleText = EnsureText("Title", _titleText, 18, FontStyle.Bold);
            _bodyText = EnsureText("Body", _bodyText, 14, FontStyle.Normal);
        }

        private Text EnsureText(string objectName, Text existingText, int fontSize, FontStyle fontStyle)
        {
            if (existingText != null)
            {
                return existingText;
            }

            Transform existingChild = transform.Find(objectName);
            GameObject textObject = existingChild != null
                ? existingChild.gameObject
                : new GameObject(objectName, typeof(RectTransform));

            textObject.transform.SetParent(transform, false);

            Text text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.AddComponent<Text>();
            }

            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            LayoutElement layoutElement = textObject.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = textObject.AddComponent<LayoutElement>();
            }

            layoutElement.preferredHeight = objectName == "Title" ? 28f : 245f;

            return text;
        }

        private static string GetSizeText(BuildingModel building)
        {
            int width = building.Definition.Width;
            int height = building.Definition.Height;

            if (building.Direction == BuildingDirection.East || building.Direction == BuildingDirection.West)
            {
                width = building.Definition.Height;
                height = building.Definition.Width;
            }

            return width + " x " + height;
        }

        private static string GetSelectedRecipeText(BuildingModel building)
        {
            return string.IsNullOrEmpty(building.SelectedRecipeId) ? "None" : building.SelectedRecipeId;
        }
    }
}
