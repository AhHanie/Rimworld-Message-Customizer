using HarmonyLib;
using UnityEngine;
using Verse;

namespace Message_Customizer.Patches
{
    [HarmonyPatch(typeof(Message), nameof(Message.CalculateRect))]
    public static class Message_CalculateRect_Patch
    {
        public static bool Prefix(Message __instance, float x, float y, ref Rect __result)
        {
            if (!ModSettings.Enabled)
                return true;

            __result = MessageStyleUtility.CalculateRect(__instance, x, y);
            return false;
        }
    }
}
