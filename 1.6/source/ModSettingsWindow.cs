using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Message_Customizer
{
    public static class ModSettingsWindow
    {
        private static Vector2 _scrollPos;
        private static float _viewHeight = 800f;

        private const float RowHeight = 24f;
        private const float FontBtnWidth = 80f;
        private const float SectionGap = 6f;

        public static void Draw(Rect parent)
        {
            Rect outRect = parent;
            Rect viewRect = new Rect(0f, 0f, outRect.width - 20f, _viewHeight);

            Widgets.BeginScrollView(outRect, ref _scrollPos, viewRect);

            var listing = new Listing_Standard();
            listing.maxOneColumn = true;
            listing.Begin(viewRect);

            // Master enable
            listing.CheckboxLabeled("MessageCustomizer.Enabled".Translate(), ref ModSettings.Enabled);
            listing.GapLine();

            // Preview and Reset buttons
            Rect btnRow = listing.GetRect(32f);
            float btnW = 120f;
            if (Widgets.ButtonText(new Rect(btnRow.x, btnRow.y, btnW, 32f), "MessageCustomizer.Preview".Translate()))
                Messages.Message("MessageCustomizer.PreviewText".Translate(), MessageTypeDefOf.NeutralEvent, historical: false);
            if (Widgets.ButtonText(new Rect(btnRow.x + btnW + 8f, btnRow.y, btnW, 32f), "MessageCustomizer.Reset".Translate()))
                ModSettings.ResetToDefaults();
            listing.Gap(SectionGap);
            listing.GapLine();

            // Font preset
            listing.Label("MessageCustomizer.FontPreset".Translate());
            Rect fontRow = listing.GetRect(RowHeight);
            TooltipHandler.TipRegion(fontRow, "MessageCustomizer.UnsupportedFontTooltip".Translate());
            DrawFontButton(new Rect(fontRow.x, fontRow.y, FontBtnWidth, RowHeight), "Tiny", GameFont.Tiny);
            DrawFontButton(new Rect(fontRow.x + FontBtnWidth + 4f, fontRow.y, FontBtnWidth, RowHeight), "Small", GameFont.Small);
            DrawFontButton(new Rect(fontRow.x + (FontBtnWidth + 4f) * 2f, fontRow.y, FontBtnWidth, RowHeight), "Medium", GameFont.Medium);
            listing.Gap(SectionGap);

            // Font size
            DrawIntSlider(listing, "MessageCustomizer.FontSize".Translate(), ref ModSettings.FontSize, 8, 32);
            listing.GapLine();

            // Text color
            DrawColorRow(listing, "MessageCustomizer.TextColor".Translate(), ModSettings.TextColor, c => ModSettings.TextColor = c);
            listing.GapLine();

            // Text stroke
            listing.CheckboxLabeled(
                "MessageCustomizer.TextStroke".Translate(),
                ref ModSettings.TextStrokeEnabled,
                "MessageCustomizer.TextStrokeTooltip".Translate());
            if (ModSettings.TextStrokeEnabled)
            {
                DrawIntSlider(listing, "MessageCustomizer.TextStrokeThickness".Translate(), ref ModSettings.TextStrokeThickness, 1, 4);
                DrawColorRow(listing, "MessageCustomizer.TextStrokeColor".Translate(), ModSettings.TextStrokeColor, c => ModSettings.TextStrokeColor = c);
            }
            listing.GapLine();

            // Background
            listing.CheckboxLabeled("MessageCustomizer.Background".Translate(), ref ModSettings.BackgroundEnabled);
            if (ModSettings.BackgroundEnabled)
            {
                listing.CheckboxLabeled("MessageCustomizer.BackgroundVanillaVisibility".Translate(), ref ModSettings.BackgroundUseVanillaVisibility);
                DrawFloatSlider(listing, "MessageCustomizer.BackgroundOpacity".Translate(), ref ModSettings.BackgroundOpacity, 0f, 1f);
                DrawColorRow(listing, "MessageCustomizer.BackgroundColor".Translate(), ModSettings.BackgroundColor, c => ModSettings.BackgroundColor = c);
            }
            listing.GapLine();

            // Border
            listing.CheckboxLabeled("MessageCustomizer.Border".Translate(), ref ModSettings.BorderEnabled);
            if (ModSettings.BorderEnabled)
            {
                DrawIntSlider(listing, "MessageCustomizer.BorderThickness".Translate(), ref ModSettings.BorderThickness, 1, 8);
                DrawColorRow(listing, "MessageCustomizer.BorderColor".Translate(), ModSettings.BorderColor, c => ModSettings.BorderColor = c);
            }

            _viewHeight = listing.CurHeight + 30f;
            listing.End();

            Widgets.EndScrollView();
        }

        private static void DrawFontButton(Rect rect, string label, GameFont font)
        {
            if (ModSettings.Font == font)
                Widgets.DrawHighlight(rect);
            if (Widgets.ButtonText(rect, label))
                ModSettings.Font = font;
        }

        private static void DrawIntSlider(Listing_Standard listing, string label, ref int value, int min, int max)
        {
            Rect row = listing.GetRect(RowHeight);
            float lw = row.width * 0.35f;
            float sw = row.width * 0.45f;
            Widgets.Label(new Rect(row.x, row.y, lw, row.height), label);
            float raw = GUI.HorizontalSlider(new Rect(row.x + lw, row.y + 4f, sw, row.height - 4f), value, min, max);
            value = Mathf.Clamp(Mathf.RoundToInt(raw), min, max);
            Widgets.Label(new Rect(row.x + lw + sw + 6f, row.y, 46f, row.height), value.ToString());
        }

        private static void DrawFloatSlider(Listing_Standard listing, string label, ref float value, float min, float max)
        {
            Rect row = listing.GetRect(RowHeight);
            float lw = row.width * 0.35f;
            float sw = row.width * 0.45f;
            Widgets.Label(new Rect(row.x, row.y, lw, row.height), label);
            value = Mathf.Clamp(GUI.HorizontalSlider(new Rect(row.x + lw, row.y + 4f, sw, row.height - 4f), value, min, max), min, max);
            Widgets.Label(new Rect(row.x + lw + sw + 6f, row.y, 56f, row.height), value.ToStringPercent());
        }

        private static void DrawColorRow(Listing_Standard listing, string label, Color color, Action<Color> setter)
        {
            Rect row = listing.GetRect(RowHeight);
            // Swatch
            Widgets.DrawBoxSolid(new Rect(row.x, row.y, RowHeight, RowHeight), new Color(color.r, color.g, color.b, 1f));
            // Label
            Widgets.Label(new Rect(row.x + RowHeight + 6f, row.y, row.width - RowHeight - 110f, row.height), label);
            // Pick button
            if (Widgets.ButtonText(new Rect(row.xMax - 100f, row.y, 100f, row.height), "MessageCustomizer.PickColor".Translate()))
                Find.WindowStack.Add(new Dialog_ColorPicker(color, setter));
        }
    }
}
