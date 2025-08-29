using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CompatLayer.BlockEntity;
using CompatLayer.Harmony.Patches;
using HarmonyLib;
using herbarium;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace CompatLayer.Harmony;

[HarmonyPatch(typeof(Herbarium))]
[HarmonyPatch(nameof(Herbarium.Start))]
public static class Herbarium_Start
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // Create CodeMatcher for Herbarium class Start method
        var codeMatcher = new CodeMatcher(instructions, generator);
        /* CodeMatch and replace BEHerbariumBerryBush class registration with patched version
         * --------------------------------------------------------------------------------
         * IL_01fe: ldtoken      herbarium.BEHerbariumBerryBush
         */
        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldtoken, typeof(BEHerbariumBerryBush)));
        codeMatcher.ThrowIfInvalid("Invalid code match");
        codeMatcher.Repeat(matchAction: cm => { cm.SetOperandAndAdvance(typeof(BEHerbariumBerryBushPatched)); });
        /* CodeMatch and replace BETallBerryBush class registration with patched version
         * --------------------------------------------------------------------------------
         * IL_01fe: ldtoken      herbarium.BETallBerryBush
         */
        codeMatcher.Start();
        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldtoken, typeof(BETallBerryBush)));
        codeMatcher.ThrowIfNotMatch("Unable to find Herbarium Tall Berry Bush");
        codeMatcher.SetOperandAndAdvance(typeof(BETallBerryBushPatched));
        
        /* CodeMatch and replace BEGroundBerryPlant class registration with patched version
         * --------------------------------------------------------------------------------
         * IL_01fe: ldtoken      herbarium.BEGroundBerryPlant
         */
        codeMatcher.Start();
        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldtoken, typeof(BEGroundBerryPlant)));
        codeMatcher.ThrowIfNotMatch("Unable to find Herbarium Ground Berry Plant");
        codeMatcher.SetOperandAndAdvance(typeof(BEGroundBerryPlantPatched));
        
        /* CodeMatch and replace MelonCropBehavior class registration with patched version
         * --------------------------------------------------------------------------------
         * IL_02ae: ldtoken      herbarium.MelonCropBehavior
         */
        codeMatcher.Start();
        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldtoken, typeof(MelonCropBehavior)));
        codeMatcher.ThrowIfNotMatch("Unable to find Herbarium Melon crop behavior");
        codeMatcher.SetOperandAndAdvance(typeof(MelonCropBehaviorPatched));
        
        /* CodeMatch and remove ItemHerbSeed class registration from Herbarium ModSystem file
         * ------------------------------------------------------------------------------------
         * IL_02eb: ldstr        "ItemHerbSeed"
         * IL_02f0: ldtoken      herbarium.ItemHerbSeed
         * IL_02f5: call         class [System.Runtime]System.Type [System.Runtime]System.Type::GetTypeFromHandle(valuetype [System.Runtime]System.RuntimeTypeHandle)
         * IL_02fa: callvirt     instance void [VintagestoryAPI]Vintagestory.API.Common.ICoreAPICommon::RegisterItemClass(string, class [System.Runtime]System.Type)
         */
        //codeMatcher.Start();
        //codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldtoken, typeof(ItemHerbSeed)));
        //codeMatcher.RemoveInstructions(4); // Remove 4 instructions starting with current
        
        return codeMatcher.InstructionEnumeration();
    }
}

[HarmonyPatch(typeof(ItemClipping))]
[HarmonyPatch(nameof(ItemClipping.OnHeldInteractStart))]
public static class Herbarium_ItemClipping_OnHeldInteractStart
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);
        
        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Isinst, typeof(BETallBerryBush)));
        codeMatcher.ThrowIfInvalid("Invalid code match");
        codeMatcher.Repeat(matchAction: cm => { cm.SetOperandAndAdvance(typeof(BETallBerryBushPatched)); });
        
        return codeMatcher.InstructionEnumeration();
    }
}

[HarmonyPatch(typeof(BlockClipping))]
[HarmonyPatch(nameof(BlockClipping.CanPlaceClipping))]
public static class Herbarium_BlockClipping_CanPlaceClipping
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);
        
        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Isinst, typeof(BETallBerryBush)));
        codeMatcher.SetOperandAndAdvance(typeof(BETallBerryBushPatched));
        
        return codeMatcher.InstructionEnumeration();
    }
}

[HarmonyPatch(typeof(HerbariumBerryBush))]
[HarmonyPatch(nameof(HerbariumBerryBush.OnBlockInteractStart))]
public static class Herbarium_HerbariumBerryBush_OnBlockInteractStart
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);
        
        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Isinst, typeof(BEHerbariumBerryBush)));
        codeMatcher.ThrowIfInvalid("Invalid code match");
        codeMatcher.SetOperandAndAdvance(typeof(BEHerbariumBerryBushPatched));
        
        codeMatcher.Start();
        codeMatcher.MatchStartForward(new CodeMatch(CodeInstruction.LoadField(typeof(BEHerbariumBerryBush), nameof(BEHerbariumBerryBush.Pruned))));
        codeMatcher.ThrowIfNotMatch("Code not matched");
        codeMatcher.SetInstruction(CodeInstruction.LoadField(typeof(BEHerbariumBerryBushPatched), nameof(BEHerbariumBerryBushPatched.Pruned)));
        
        return codeMatcher.InstructionEnumeration();
    }
}

[HarmonyPatch(typeof(HerbariumBerryBush))]
[HarmonyPatch(nameof(HerbariumBerryBush.OnBlockInteractStep))]
public static class Herbarium_HerbariumBerryBush_OnBlockInteractStep
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);
        
        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Isinst, typeof(BEHerbariumBerryBush)));
        codeMatcher.ThrowIfInvalid("Invalid code match");
        codeMatcher.SetOperandAndAdvance(typeof(BEHerbariumBerryBushPatched));
        
        codeMatcher.Start();
        codeMatcher.MatchStartForward(new CodeMatch(CodeInstruction.LoadField(typeof(BEHerbariumBerryBush), nameof(BEHerbariumBerryBush.Pruned))));
        codeMatcher.ThrowIfNotMatch("Code not matched");
        codeMatcher.SetInstruction(CodeInstruction.LoadField(typeof(BEHerbariumBerryBushPatched), nameof(BEHerbariumBerryBushPatched.Pruned)));
        
        return codeMatcher.InstructionEnumeration();
    }
}

[HarmonyPatch(typeof(HerbariumBerryBush))]
[HarmonyPatch(nameof(HerbariumBerryBush.OnBlockInteractStop))]
public static class Herbarium_HerbariumBerryBush_OnBlockInteractStop
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);
        
        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Isinst, typeof(BEHerbariumBerryBush)));
        codeMatcher.ThrowIfInvalid("Invalid code match");
        codeMatcher.SetOperandAndAdvance(typeof(BEHerbariumBerryBushPatched));
        
        codeMatcher.Start();
        codeMatcher.MatchStartForward(new CodeMatch(CodeInstruction.LoadField(typeof(BEHerbariumBerryBush), nameof(BEHerbariumBerryBush.Pruned))));
        codeMatcher.ThrowIfNotMatch("Code not matched");
        codeMatcher.SetInstruction(CodeInstruction.LoadField(typeof(BEHerbariumBerryBushPatched), nameof(BEHerbariumBerryBushPatched.Pruned)));
        
        return codeMatcher.InstructionEnumeration();
    }
}