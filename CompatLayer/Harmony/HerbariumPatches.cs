﻿using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using herbarium;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace CompatLayer.Harmony;

public class HerbariumPatches
{
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Herbarium), nameof(Herbarium.Start))]
    public static IEnumerable<CodeInstruction> Start(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldtoken, typeof(MelonCropBehavior)));
        codeMatcher.SetOperandAndAdvance(typeof(MelonCropBehaviorPatched));
        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldtoken, typeof(ItemHerbSeed)));
        codeMatcher.RemoveInstructions(4);
        
        return codeMatcher.InstructionEnumeration();
    }
}

public static class HerbariumExtensions
{
    
}

public class MelonCropBehaviorPatched : CropBehavior
{
    //Minimum stage at which vines can grow
    private int vineGrowthStage = 3;

    //Stage at which vines will wither
    private int vineWitherStage = 8;

    //Probability of vine growth once the minimum vine growth stage is reached
    private float vineGrowthQuantity;

    private AssetLocation vineBlockLocation;
    NatFloat vineGrowthQuantityGen;
    string melonBlockCode;
    string domainCode;

    public MelonCropBehaviorPatched(Block block) : base(block)
    {
    }

    public override void Initialize(JsonObject properties)
    {
        base.Initialize(properties);

        vineGrowthStage = properties["vineGrowthStage"].AsInt(3);
        vineWitherStage = properties["vineWitherStage"].AsInt(8);
        vineGrowthQuantityGen = properties["vineGrowthQuantity"].AsObject<NatFloat>();
        melonBlockCode = properties["melonBlockCode"].AsString();
        domainCode = properties["domainCode"].AsString("game");

        vineBlockLocation = new AssetLocation(domainCode + ":" + melonBlockCode + "-vine-1-normal");
    }

    public override void OnPlanted(ICoreAPI api, ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel)
    {
        vineGrowthQuantity = vineGrowthQuantityGen.nextFloat(1, api.World.Rand);
    }

    public override bool TryGrowCrop(ICoreAPI api, IFarmlandBlockEntity farmland, double currentTotalHours,
        int newGrowthStage, ref EnumHandling handling)
    {
        if (vineGrowthQuantity == 0)
        {
            vineGrowthQuantity = farmland.CropAttributes.GetFloat("vineGrowthQuantity",
                vineGrowthQuantityGen.nextFloat(1, api.World.Rand));
            farmland.CropAttributes.SetFloat("vineGrowthQuantity", vineGrowthQuantity);
        }

        handling = EnumHandling.PassThrough;

        if (newGrowthStage >= vineGrowthStage)
        {
            if (newGrowthStage == vineWitherStage)
            {
                bool allWithered = true;
                foreach (BlockFacing facing in BlockFacing.HORIZONTALS)
                {
                    Block block = api.World.BlockAccessor.GetBlock(farmland.Pos.AddCopy(facing).Up());
                    if (block.Code.PathStartsWith(melonBlockCode + "-vine"))
                    {
                        allWithered &= block.LastCodePart() == "withered";
                    }
                }

                if (!allWithered)
                {
                    handling = EnumHandling.PreventDefault;
                }

                return false;
            }

            if (api.World.Rand.NextDouble() < vineGrowthQuantity)
            {
                return TrySpawnVine(api, farmland, currentTotalHours);
            }
        }

        return false;
    }

    private bool TrySpawnVine(ICoreAPI api, IFarmlandBlockEntity farmland, double currentTotalHours)
    {
        BlockPos motherplantPos = farmland.UpPos;
        foreach (BlockFacing facing in BlockFacing.HORIZONTALS)
        {
            BlockPos candidatePos = motherplantPos.AddCopy(facing);
            Block block = api.World.BlockAccessor.GetBlock(candidatePos);
            if (CanReplace(block))
            {
                if (CanSupportMelon(api, candidatePos.DownCopy()))
                {
                    DoSpawnVine(api, candidatePos, motherplantPos, facing, currentTotalHours);
                    return true;
                }
            }
        }

        return false;
    }

    private void DoSpawnVine(ICoreAPI api, BlockPos vinePos, BlockPos motherplantPos, BlockFacing facing,
        double currentTotalHours)
    {
        Block vineBlock = api.World.GetBlock(vineBlockLocation);
        api.World.BlockAccessor.SetBlock(vineBlock.BlockId, vinePos);

        if (api.World is IServerWorldAccessor)
        {
            BlockEntity be = api.World.BlockAccessor.GetBlockEntity(vinePos);
            if (be is BlockEntityMelonVine)
            {
                ((BlockEntityMelonVine)be).CreatedFromParent(motherplantPos, facing, currentTotalHours);
            }
        }
    }

    private bool CanReplace(Block block)
    {
        if (block == null)
        {
            return true;
        }

        return block.Replaceable >= 6000 && !block.Code.GetName().Contains(melonBlockCode);
    }

    public static bool CanSupportMelon(ICoreAPI api, BlockPos pos)
    {
        Block underblock = api.World.BlockAccessor.GetBlock(pos, BlockLayersAccess.Fluid);
        if (underblock.IsLiquid()) return false;
        underblock = api.World.BlockAccessor.GetBlock(pos);
        return underblock.Replaceable <= 5000;
    }
}