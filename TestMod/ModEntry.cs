﻿using StardewModdingAPI;
using HarmonyLib;
using StardewValley.Locations;

namespace TestMod
{
    public class ModEntry : Mod
    {
        internal static IModHelper modHelper;
        internal static IMonitor modMonitor;

        public override void Entry(IModHelper helper)
        {
            modHelper = helper;
            modMonitor = Monitor;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(AccessTools.Method(typeof(MineShaft), nameof(MineShaft.checkAction)), transpiler: new HarmonyMethod(typeof(Patches.MineShaft_Patch), nameof(Patches.MineShaft_Patch.Transpiler)));

            Monitor.VerboseLog(ModManifest.Name + " loaded.");
        }
    }
}