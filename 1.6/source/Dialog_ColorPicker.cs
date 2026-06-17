using System;
using UnityEngine;
using Verse;

namespace Message_Customizer
{
    public class Dialog_ColorPicker : Window
    {
        private Color _color;
        private readonly Color _oldColor;
        private readonly Action<Color> _onAccept;
        private bool _dragging;

        private const float WheelSize = 200f;
        private const float RowHeight = 28f;
        private const float ButtonHeight = 32f;
        private const float Gap = 8f;

        public override Vector2 InitialSize
        {
            get
            {
                float contentW = WheelSize;
                float contentH = WheelSize + Gap + RowHeight + Gap + RowHeight + Gap + ButtonHeight;
                float margin = StandardMargin * 2f;
                return new Vector2(contentW + margin, contentH + margin);
            }
        }

        public Dialog_ColorPicker(Color initialColor, Action<Color> onAccept)
        {
            _color = initialColor;
            _oldColor = initialColor;
            _onAccept = onAccept;
            forcePause = false;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            closeOnAccept = false;
            doCloseButton = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            float x = inRect.x;
            float y = inRect.y;
            float w = inRect.width;

            // HSV color wheel (H/S; V preserved from current color)
            float wheelSize = Mathf.Min(w, WheelSize);
            Rect wheelRect = new Rect(x + (w - wheelSize) / 2f, y, wheelSize, wheelSize);
            Widgets.HSVColorWheel(wheelRect, ref _color, ref _dragging);
            y += wheelSize + Gap;

            // Brightness (V) slider
            Color.RGBToHSV(_color, out float h, out float s, out float v);
            float labelW = 80f;
            Widgets.Label(new Rect(x, y, labelW, RowHeight), "MessageCustomizer.Brightness".Translate());
            float newV = GUI.HorizontalSlider(new Rect(x + labelW, y + 5f, w - labelW, 18f), v, 0f, 1f);
            if (!Mathf.Approximately(newV, v))
                _color = Color.HSVToRGB(h, s, newV);
            y += RowHeight + Gap;

            // Old / new color readback with border
            float halfW = (w - 4f) / 2f;
            Rect oldRect = new Rect(x, y, halfW, RowHeight);
            Rect newRect = new Rect(x + halfW + 4f, y, halfW, RowHeight);
            Widgets.DrawBoxSolid(oldRect, _oldColor);
            Widgets.DrawBoxSolid(newRect, _color);
            GUI.color = Color.gray;
            Widgets.DrawBox(oldRect, 1);
            Widgets.DrawBox(newRect, 1);
            GUI.color = Color.white;
            y += RowHeight + Gap;

            // Buttons
            float btnW = (w - Gap) / 2f;
            if (Widgets.ButtonText(new Rect(x, y, btnW, ButtonHeight), "Cancel".Translate()))
                Close();
            if (Widgets.ButtonText(new Rect(x + btnW + Gap, y, btnW, ButtonHeight), "Accept".Translate()))
            {
                _onAccept(_color);
                Close();
            }
        }
    }
}
