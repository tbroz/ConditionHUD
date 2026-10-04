using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;

namespace ConditionHUD
{
    [BepInPlugin("tbroz.ros.conditionhud", "Condition HUD", "0.1.0")]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;
        public static InteractionObjectPickup currentHeldItem;

        public static ConfigEntry<bool> ShowHUD;
        public static ConfigEntry<KeyCode> ToggleKey;
        public static ConfigEntry<int> FontSize;
        public static ConfigEntry<int> FontOpacity;
        public static ConfigEntry<bool> ShowConditionOnly;

        public override void Load()
        {
            Logger = Log;

            ShowHUD = Config.Bind("General", "ShowHUD", true,
                "Set to True/False based on if you want to see the Item/Condition while holding an item.");

            ToggleKey = Config.Bind("General", "ToggleKey", KeyCode.F8,
                "Key that turns the HUD on and off while playing the game.");

            ShowConditionOnly = Config.Bind("General", "ShowConditionOnly", false,
                "By default, both the Item name and the condition are shown. Set to True/False if you want ONLY the condition shown");

            FontSize = Config.Bind("General", "FontSize", 22,
                new ConfigDescription("Text size in pixels. Please be aware of clipping the larger you go.", 
                    new AcceptableValueRange<int>(10, 50)));

            FontOpacity = Config.Bind("General", "FontOpacity", 100,
                new ConfigDescription("Text opacity. 0 (invisible) to 100 (solid). The lowest you can set your opacity is 10.", 
                new AcceptableValueRange<int>(10, 100)));

            new Harmony("tbroz.ros.conditionhud").PatchAll();
            Logger.LogInfo("Condition HUD Loaded");

            AddComponent<ConditionHUDComponent>();

        }
    }
}



