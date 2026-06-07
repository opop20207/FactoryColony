using UnityEngine;

namespace FactoryColony
{
    public sealed class GoalHudView : MonoBehaviour
    {
        private readonly Rect _panelRect = new Rect(500f, 12f, 330f, 160f);

        private GoalTracker _tracker;
        private GUIStyle _panelStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _completeStyle;

        public void Initialize(GoalTracker tracker)
        {
            _tracker = tracker;
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(_panelRect, GUIContent.none, _panelStyle);
            GUILayout.Label("Goals", _labelStyle);

            if (_tracker == null)
            {
                GUILayout.Label("Goal tracker is not initialized.", _labelStyle);
                GUILayout.EndArea();
                return;
            }

            foreach (ProductionGoalModel goal in _tracker.Goals)
            {
                string line = goal.DisplayName
                    + ": " + goal.CurrentAmount
                    + " / " + goal.RequiredAmount;

                if (goal.IsCompleted)
                {
                    line += " [Complete]";
                }

                GUILayout.Label(line, goal.IsCompleted ? _completeStyle : _labelStyle);
            }

            if (_tracker.AllCompleted)
            {
                GUILayout.Space(4f);
                GUILayout.Label("FACTORY GOAL COMPLETE", _completeStyle);
            }

            GUILayout.EndArea();
        }

        private void EnsureStyles()
        {
            if (_panelStyle != null && _labelStyle != null && _completeStyle != null)
            {
                return;
            }

            _panelStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal =
                {
                    textColor = Color.white
                }
            };

            _completeStyle = new GUIStyle(_labelStyle)
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = new Color(0.4f, 1f, 0.55f)
                }
            };
        }
    }
}
