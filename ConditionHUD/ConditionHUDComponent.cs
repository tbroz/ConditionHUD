using UnityEngine;

namespace ConditionHUD
{
    public class ConditionHUDComponent : MonoBehaviour
    {
        private GUIStyle labelStyle;
        private GUIStyle shadowStyle;
        private float nextPoll;

        public void Update()
        {
            if (Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + 0.1f;   
            InventoryHelpers.PollHeld();
        }

        public void OnGUI()
        {

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
                    fontSize = 22,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.LowerRight
                };
                labelStyle.normal.textColor = Color.white;

                shadowStyle = new GUIStyle(labelStyle);
                shadowStyle.normal.textColor = Color.black;
            }

            string displayMessage = $"{name} [{condition}]";

            float width = 400f;
            float height = 50f;
            float marginX = 30f;
            float MarginY = 30f;

            Rect position = new Rect(
                Screen.width - width - marginX,
                Screen.height - height - MarginY,
                width,
                height
            );

            GUI.Label(new Rect(position.x + 2, position.y + 2, width, height), displayMessage, shadowStyle);
            GUI.Label(position, displayMessage, labelStyle);
        }

    }
}
