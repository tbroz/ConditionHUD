using HarmonyLib;

namespace ConditionHUD
{

    [HarmonyPatch(typeof(InteractionObjectPickup), nameof(InteractionObjectPickup.PickUp))]
    class PickUpPatch
    {
        // used when picking up items
        static void Postfix(InteractionObjectPickup __instance, bool __result)
        {
            if (__result) Plugin.currentHeldItem = __instance;
        }

    }

    [HarmonyPatch(typeof(InteractionObjectPickup), nameof(InteractionObjectPickup.RefreshHeldParticles))]
    class RefreshHeldPatch
    {
        // used when switching between items in hand
        static void Postfix(InteractionObjectPickup __instance)
        {
            Plugin.currentHeldItem = __instance;
        }
    }

    [HarmonyPatch(typeof(InteractionObjectPickup), nameof(InteractionObjectPickup.PutDown))]
    class PutDownPatch
    {
        static void Postfix(InteractionObjectPickup __instance)
        {
            if (Plugin.currentHeldItem == __instance) Plugin.currentHeldItem = null;
        }
    }

    [HarmonyPatch(typeof(InteractionObjectPickup), nameof(InteractionObjectPickup.ThrowThis))]
    class ThrowThisPatch
    {
        // used when throwing an item
        static void Postfix(InteractionObjectPickup __instance)
        {
            if (Plugin.currentHeldItem == __instance) Plugin.currentHeldItem = null;
        }
    }

    [HarmonyPatch(typeof(InteractionObjectPickup), nameof(InteractionObjectPickup.PutOnSomething))]
    class PutOnPatch
    {
        // used when adding to shelf or table
        static void Postfix(InteractionObjectPickup __instance)
        {
            if (Plugin.currentHeldItem == __instance) Plugin.currentHeldItem = null;
        }
    }

}