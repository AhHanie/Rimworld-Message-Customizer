using HarmonyLib;
using Verse;

namespace Message_Customizer.Patches
{
    [HarmonyPatch(typeof(Message), nameof(Message.Draw))]
    public static class Message_Draw_Patch
    {
        public static bool Prefix(Message __instance, int xOffset, int yOffset)
        {
            if (!ModSettings.Enabled)
                return true;

            MessageStyleUtility.DrawMessage(__instance, xOffset, yOffset);
            return false;
        }
    }
}
