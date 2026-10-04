using UnityEngine;

namespace ConditionHUD
{
    public class ConditionHUDComponent : MonoBehaviour
    {
        private GUIStyle labelStyle;
        private GUIStyle shadowStyle;
        private float nextPoll;

        // configurations
        private bool showConditionOnly;
        private int fontSize;
        private float fontOpacity;

        private bool ShowHUD
        {
            get => Plugin.ShowHUD.Value;
            set => Plugin.ShowHUD.Value = value;
        }

        public void Awake()
        {
            showConditionOnly = Plugin.ShowConditionOnly.Value;
            fontSize = Plugin.FontSize.Value;
            fontOpacity = Plugin.FontOpacity.Value;
        }
  
        public void Update()
        {
            if (Input.GetKeyDown(Plugin.ToggleKey.Value))
            {
                ShowHUD = !ShowHUD;
            }

            if (Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + 0.1f;   
            InventoryHelpers.PollHeld();
        }

        public void OnGUI()
        {
            if (!ShowHUD) return; 
            if (Plugin.currentHeldItem == null) return;

            var info = Plugin.currentHeldItem.GetInfo();
            if (info == null) return;

            string name = info.GetName();
            string condition = info.GetSimpleConditionText();

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(condition)) return;

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.LowerLeft,
                    wordWrap = true
                };
                labelStyle.normal.textColor = Color.white;

                shadowStyle = new GUIStyle(labelStyle);
                shadowStyle.normal.textColor = Color.black;
            }

            labelStyle.fontSize = fontSize;
            labelStyle.normal.textColor = new Color(1f, 1f, 1f, fontOpacity);

            shadowStyle.fontSize = fontSize;
            shadowStyle.normal.textColor = new Color(0f, 0f, 0f, fontOpacity);

            string displayMessage = showConditionOnly ? $"({condition})" : $"{name} ({condition})";

            float marginX = 30f;
            float MarginY = 30f;
            float maxWidth = 600f;

            GUIContent content = new GUIContent(displayMessage);
            float width = Mathf.Min(labelStyle.CalcSize(content).x + 10f, maxWidth);

            float height = labelStyle.CalcHeight(content, width);

            Rect position = new Rect(
                marginX, 
                Screen.height - height - MarginY,
                width,
                height
            );

            GUI.Label(new Rect(position.x + 2, position.y + 2, width, height), displayMessage, shadowStyle);
            GUI.Label(position, displayMessage, labelStyle);
        }

    }
}
