using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TestMod
{
    public static class Patches
    {
        public static class MineShaft_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var foundQuestionDialogue = false;
                var startIndex = -1;

                Label idkWhatThisIsYet = il.DefineLabel();

                var codes = new List<CodeInstruction>(instructions);
                for (var i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ret)
                    {
                        if (foundQuestionDialogue)
                        {
                            ModEntry.modMonitor.VerboseLog("END " + startIndex);
                            break;
                        }
                        else
                        {
                            ModEntry.modMonitor.VerboseLog("START " + (i - 2));

                            startIndex = i - 2;

                            for (var j = startIndex; j < codes.Count; j++)
                            {
                                if (codes[j].opcode == OpCodes.Ret)
                                {
                                    break;
                                }

                                var operand = codes[j].operand;
                                ModEntry.modMonitor.VerboseLog(operand?.ToString());

                                if (operand?.ToString() == "Void createQuestionDialogue(System.String, StardewValley.Response[], System.String)")
                                {
                                    foundQuestionDialogue = true;
                                    codes[j].labels.Add(idkWhatThisIsYet);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (foundQuestionDialogue)
                {
                    ModEntry.modMonitor.VerboseLog("found question dialogue! Nop-ing..");
                    codes[startIndex].opcode = OpCodes.Nop;
                    codes.RemoveRange(startIndex - 4, 4);
                    ModEntry.modMonitor.VerboseLog("Patching new code into " + nameof(MineShaft.checkAction) + "..");

                    var instructionsToInsert = new List<CodeInstruction>();
                    codes.Insert(startIndex - 4, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MineShaft_Patch), nameof(MineShaft_Patch.WarpFarmerOutOfMine)))); 
                }
                return codes;
            }

            public static void WarpFarmerOutOfMine()
            {
                if (Game1.currentLocation.Name.StartsWith("UndergroundMine"))
                {
                    MineShaft curMineShaft = Game1.currentLocation as MineShaft;
                    if (curMineShaft != null)
                    {
                        if (curMineShaft?.mineLevel > 121 && curMineShaft?.mineLevel < 77377)
                        {
                            Game1.warpFarmer("SkullCave", 0, 0, flip: true); //TODO: Find skullcave tilemap and position this.
                        }
                        else if (curMineShaft?.mineLevel == 77377)
                        {
                            Game1.warpFarmer("Mine", 68, 10, 1);
                        }
                        else
                        {
                            Game1.warpFarmer("Mine", 23, 8, 2);
                        }
                    }
                }

                Game1.currentLocation.playSound("stairsdown");
            }
        }
    }
}
