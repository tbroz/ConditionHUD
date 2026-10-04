using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace ConditionHUD
{
    [BepInPlugin("tbroz.ros.conditionhud", "Condition HUD", "0.1.0")]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;
        public static InteractionObjectPickup currentHeldItem;

        public override void Load()
        {
            Logger = Log;
            new Harmony("tbroz.ros.conditionhud").PatchAll();
            Logger.LogInfo("Condition HUD Loaded");

            AddComponent<ConditionHUDComponent>();

        }
    }
}



