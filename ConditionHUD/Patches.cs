using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace ConditionHUD
{
    public static class InventoryHelpers
    {

        /// <summary>
        /// Stores all active pickup items currently in the player's hand/stack container.
        /// </summary>
        public static List<InteractionObjectPickup> CurrentHandStack { get; private set; } = new List<InteractionObjectPickup>();

        /// <summary>
        /// the transform that holds carried items 
        /// </summary>
        public static Transform inventoryParent;

        /// <summary>
        /// Finds the actively held item
        /// </summary>
        public static InteractionObjectPickup FindActiveHeld(InteractionObjectPickup exclude = null)
        {
            if (inventoryParent == null) return null;

            for (int i = 0; i < inventoryParent.childCount; i++)
            {
                Transform child = inventoryParent.GetChild(i);
                if (child == null || !child.gameObject.activeInHierarchy) continue;

                var pickup = child.GetComponent<InteractionObjectPickup>();
                if (pickup == null) continue;
                if (exclude != null && pickup == exclude) continue;

                return pickup;
            }
            return null;
        }

        /// <summary>
        /// Looks at what is actually active in the hand right now and updates the HUD state
        /// only if it changed. Safe to call every frame or every few frames.
        /// </summary>
        public static void PollHeld(InteractionObjectPickup exclude = null)
        {
            var found = FindActiveHeld(exclude);
            if (found == Plugin.currentHeldItem) return;
            RefreshHandStack(found);
        }


        /// <summary>
        /// Scans the actual parent transform of the active pickup item to construct the hand stack.
        /// </summary>
        public static void RefreshHandStack(InteractionObjectPickup activePickup)
        {
            CurrentHandStack.Clear();

            if (activePickup == null || !activePickup.gameObject.activeInHierarchy)
            {
                Plugin.currentHeldItem = null;
                Plugin.Logger.LogInfo("[InventoryHelpers] Stack cleared (No active item).");
                return;
            }

            Transform handParent = activePickup.transform.parent;
            if (handParent == null || handParent == InteractionObjectPickup.defaultParent)
            {
                CurrentHandStack.Add(activePickup);
                Plugin.currentHeldItem = activePickup;
                Plugin.Logger.LogInfo($"[InventoryHelpers] Single held item: '{activePickup.name}' (Parent is world root or unparented)");
                return;
            }

            inventoryParent = handParent;

            for (int i = 0; i < handParent.childCount; i++)
            {
                Transform child = handParent.GetChild(i);
                if (child == null || !child.gameObject.activeInHierarchy) continue;

                var pickup = child.GetComponent<InteractionObjectPickup>();
                if (pickup != null)
                {
                    CurrentHandStack.Add(pickup);
                }
            }

            Plugin.currentHeldItem = activePickup;
            Plugin.Logger.LogInfo($"[InventoryHelpers] Refreshed Hand Stack! Socket: '{handParent.name}' | Count: {CurrentHandStack.Count} | Active Item: '{activePickup.name}'");
        }

    }

    [HarmonyPatch(typeof(InteractionObjectPickup), nameof(InteractionObjectPickup.PickUp))]
    class PickUpPatch
    {
        // used when picking up items
        static void Postfix(InteractionObjectPickup __instance, bool __result)
        {
            if (__result && __instance != null)
            {
                InventoryHelpers.RefreshHandStack(__instance);
            }
        }

    }

    [HarmonyPatch(typeof(InteractionObjectPickup), nameof(InteractionObjectPickup.RefreshHeldParticles))]
    class RefreshHeldPatch
    {
        // used when switching between items in hand
        static void Postfix(InteractionObjectPickup __instance)
        {
            Plugin.Logger.LogWarning($"===> [RefreshHeldParticles] Fired for: '{__instance?.name}' (Active: {__instance?.gameObject.activeInHierarchy})");

            if (__instance != null && __instance.gameObject.activeInHierarchy)
            {
                InventoryHelpers.RefreshHandStack(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(InteractionObjectPickup), nameof(InteractionObjectPickup.PutDown))]
    class PutDownPatch
    {
        static void Postfix(InteractionObjectPickup __instance)
        {
            InventoryHelpers.PollHeld(__instance);
        }
    }

    [HarmonyPatch(typeof(InteractionObjectPickup), nameof(InteractionObjectPickup.ThrowThis))]
    class ThrowThisPatch
    {
        // used when throwing an item
        static void Postfix(InteractionObjectPickup __instance)
        {
            InventoryHelpers.PollHeld(__instance);
        }
    }

    [HarmonyPatch(typeof(InteractionObjectPickup), nameof(InteractionObjectPickup.PutOnSomething))]
    class PutOnPatch
    {
        // used when adding to shelf or table
        static void Postfix(InteractionObjectPickup __instance)
        {
            InventoryHelpers.PollHeld(__instance);
        }
    }
}