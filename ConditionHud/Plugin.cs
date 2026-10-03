using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

[BepInPlugin("tbroz.ros.conditionhud", "Condition HUD", "0.1.0")]
public class Plugin : BasePlugin
{
    public static ManualLogSource Logger;

    public override void Load()
    {
        Logger = Log;
        new Harmony("tbroz.ros.conditionhud").PatchAll();
        Logger.LogInfo("Condition HUD Loaded");

        foreach(var m in Harmony.GetAllPatchedMethods())
        {
            Logger.LogInfo($"Patched: {m.DeclaringType?.Name}.{m.Name}");
        }
    }
}

[HarmonyPatch(typeof(ConditionTooltip), nameof(ConditionTooltip.Show))]
class ShowPatch
{
    static void Postfix(InventoryItem item, bool onSale)
    {
        Plugin.Logger.LogInfo($"ConditionTooltip.Show fired. item={item}, onSale={onSale}");
    }
}

[HarmonyPatch(typeof(OnHoverShow), nameof(OnHoverShow.OnPointerEnter))]
class HoverPatch
{
    static void Postfix(OnHoverShow __instance)
    {
        var t = __instance.transform;
        string path = t.name;
        while (t.parent != null) { t = t.parent; path = t.name + "/" + path; }

        string target = __instance._gameObject != null ? __instance._gameObject.name : "null";
        Plugin.Logger.LogInfo($"Hover: {path} -> shows: {target}");
    }
}
