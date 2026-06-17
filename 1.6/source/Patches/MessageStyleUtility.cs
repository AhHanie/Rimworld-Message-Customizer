using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Message_Customizer.Patches
{
    internal static class MessageStyleUtility
    {
        private static bool ShouldDrawBackground()
        {
            if (Current.ProgramState != ProgramState.Playing)
                return true;
            WindowStack stack = Find.WindowStack;
            for (int i = 0; i < stack.Count; i++)
            {
                if (stack[i].CausesMessageBackground())
                    return true;
            }
            return false;
        }

        private static GUIStyle BuildStyle(float alpha)
        {
            GameFont prevFont = Text.Font;
            Text.Font = ModSettings.Font;
            GUIStyle style = new GUIStyle(Text.CurFontStyle);
            Text.Font = prevFont;

            style.fontSize = ModSettings.FontSize;
            Color tc = ModSettings.TextColor;
            style.normal.textColor = new Color(tc.r, tc.g, tc.b, tc.a * alpha);
            style.alignment = TextAnchor.MiddleLeft;
            style.wordWrap = false;
            return style;
        }

        public static Rect CalculateRect(Message instance, float x, float y)
        {
            GUIStyle style = BuildStyle(1f);
            Vector2 size = style.CalcSize(new GUIContent(instance.text));
            Rect rect = new Rect(x, y, size.x, size.y).ExpandedBy(6f, 2f);
            instance.lastDrawRect = rect;
            return rect;
        }

        public static void DrawMessage(Message instance, int xOffset, int yOffset)
        {
            Rect rect = instance.CalculateRect(xOffset, yOffset);
            int id = instance.ID;

            Find.WindowStack.ImmediateWindow(Gen.HashCombineInt(id, 45574281), rect, WindowLayer.Super, delegate
            {
                Rect rect2 = rect.AtZero();
                float alpha = instance.Alpha;

                Color prevColor = GUI.color;
                GameFont prevFont = Text.Font;
                TextAnchor prevAnchor = Text.Anchor;
                bool prevWordWrap = Text.WordWrap;

                GUI.color = new Color(1f, 1f, 1f, alpha);

                // Background
                bool drawBg = ModSettings.BackgroundEnabled;
                if (drawBg && ModSettings.BackgroundUseVanillaVisibility)
                    drawBg = ShouldDrawBackground();

                if (drawBg)
                {
                    Color bg = ModSettings.BackgroundColor;
                    Widgets.DrawBoxSolid(rect2, new Color(bg.r, bg.g, bg.b, ModSettings.BackgroundOpacity * alpha));
                }

                // Highlight / click indicator
                if (CameraJumper.CanJump(instance.lookTargets.TryGetPrimaryTarget()) || (instance.quest != null && !instance.quest.hidden))
                {
                    UIHighlighter.HighlightOpportunity(rect2, "Messages");
                    Widgets.DrawHighlightIfMouseover(rect2);
                }

                // Flash
                if (instance.flashTime + 0.6f > RealTime.LastRealTime)
                {
                    float t = 1f - (RealTime.LastRealTime - instance.flashTime) / 0.6f;
                    Widgets.DrawTextHighlight(rect2, 0f, ColorLibrary.Yellow.ToTransparent(t));
                }

                // Border
                if (ModSettings.BorderEnabled)
                {
                    Color bc = ModSettings.BorderColor;
                    GUI.color = new Color(bc.r, bc.g, bc.b, bc.a * alpha);
                    Widgets.DrawBox(rect2, ModSettings.BorderThickness);
                    GUI.color = new Color(1f, 1f, 1f, alpha);
                }

                GUIStyle style = BuildStyle(alpha);
                Rect textRect = new Rect(6f, 2f, rect2.width - 6f, rect2.height - 2f);

                // Stroke
                if (ModSettings.TextStrokeEnabled)
                {
                    Color sc = ModSettings.TextStrokeColor;
                    GUIStyle strokeStyle = new GUIStyle(style);
                    strokeStyle.normal.textColor = new Color(sc.r, sc.g, sc.b, sc.a * alpha);
                    int n = ModSettings.TextStrokeThickness;
                    for (int dx = -n; dx <= n; dx++)
                    {
                        for (int dy = -n; dy <= n; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            GUI.Label(new Rect(textRect.x + dx, textRect.y + dy, textRect.width, textRect.height), instance.text, strokeStyle);
                        }
                    }
                }

                // Main text
                GUI.Label(textRect, instance.text, style);

                // Click behavior
                if (Current.ProgramState == ProgramState.Playing && Widgets.ButtonInvisible(rect2))
                {
                    if (Event.current.button == 1)
                    {
                        instance.startingTime = -99999f;
                    }
                    else if (CameraJumper.CanJump(instance.lookTargets.TryGetPrimaryTarget()))
                    {
                        CameraJumper.TryJumpAndSelect(instance.lookTargets.TryGetPrimaryTarget());
                        PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.ClickingMessages, KnowledgeAmount.Total);
                    }
                    else if (instance.quest != null && !instance.quest.hidden)
                    {
                        if (Find.MainTabsRoot.OpenTab == MainButtonDefOf.Quests)
                        {
                            SoundDefOf.Click.PlayOneShotOnCamera();
                        }
                        else
                        {
                            Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Quests);
                        }
                        ((MainTabWindow_Quests)MainButtonDefOf.Quests.TabWindow).Select(instance.quest);
                    }
                }

                Text.Font = prevFont;
                Text.Anchor = prevAnchor;
                Text.WordWrap = prevWordWrap;
                GUI.color = Color.white;

                if (Mouse.IsOver(rect2))
                    Messages.Notify_Mouseover(instance);

            }, doBackground: false, absorbInputAroundWindow: false, 0f);
        }
    }
}
