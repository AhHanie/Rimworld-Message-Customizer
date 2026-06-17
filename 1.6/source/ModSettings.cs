using UnityEngine;
using Verse;

namespace Message_Customizer
{
    public class ModSettings : Verse.ModSettings
    {
        public static bool Enabled = true;
        public static GameFont Font = GameFont.Small;
        public static int FontSize = 13;
        public static Color TextColor = Color.white;
        public static bool TextStrokeEnabled = false;
        public static int TextStrokeThickness = 1;
        public static Color TextStrokeColor = Color.black;
        public static bool BackgroundEnabled = true;
        public static bool BackgroundUseVanillaVisibility = true;
        public static Color BackgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        public static float BackgroundOpacity = 0.8f;
        public static bool BorderEnabled = false;
        public static int BorderThickness = 1;
        public static Color BorderColor = Color.black;

        public static void ResetToDefaults()
        {
            Enabled = true;
            Font = GameFont.Small;
            FontSize = 13;
            TextColor = Color.white;
            TextStrokeEnabled = false;
            TextStrokeThickness = 1;
            TextStrokeColor = Color.black;
            BackgroundEnabled = true;
            BackgroundUseVanillaVisibility = true;
            BackgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            BackgroundOpacity = 0.8f;
            BorderEnabled = false;
            BorderThickness = 1;
            BorderColor = Color.black;
        }

        public static void Clamp()
        {
            FontSize = Mathf.Clamp(FontSize, 8, 32);
            TextStrokeThickness = Mathf.Clamp(TextStrokeThickness, 1, 4);
            BackgroundOpacity = Mathf.Clamp01(BackgroundOpacity);
            BorderThickness = Mathf.Clamp(BorderThickness, 1, 8);
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Enabled, "messageCustomizerEnabled", true);
            Scribe_Values.Look(ref Font, "messageCustomizerFont", GameFont.Small);
            Scribe_Values.Look(ref FontSize, "messageCustomizerFontSize", 13);
            Scribe_Values.Look(ref TextColor, "messageCustomizerTextColor", Color.white);
            Scribe_Values.Look(ref TextStrokeEnabled, "messageCustomizerTextStrokeEnabled", false);
            Scribe_Values.Look(ref TextStrokeThickness, "messageCustomizerTextStrokeThickness", 1);
            Scribe_Values.Look(ref TextStrokeColor, "messageCustomizerTextStrokeColor", Color.black);
            Scribe_Values.Look(ref BackgroundEnabled, "messageCustomizerBackgroundEnabled", true);
            Scribe_Values.Look(ref BackgroundUseVanillaVisibility, "messageCustomizerBackgroundUseVanillaVisibility", true);
            Scribe_Values.Look(ref BackgroundColor, "messageCustomizerBackgroundColor", new Color(0.15f, 0.15f, 0.15f, 1f));
            Scribe_Values.Look(ref BackgroundOpacity, "messageCustomizerBackgroundOpacity", 0.8f);
            Scribe_Values.Look(ref BorderEnabled, "messageCustomizerBorderEnabled", false);
            Scribe_Values.Look(ref BorderThickness, "messageCustomizerBorderThickness", 1);
            Scribe_Values.Look(ref BorderColor, "messageCustomizerBorderColor", Color.black);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Clamp();
            }
        }
    }
}
