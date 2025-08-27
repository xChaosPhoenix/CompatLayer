using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace CompatLayer.Item;

public class ItemSeedCompat : ItemPlantableSeed
{
    public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling)
    {
        if (blockSel == null)
        {
            base.OnHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handHandling);
            return;
        }
        
        IPlayer byPlayer = (byEntity is EntityPlayer) ? byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID) : null;
        Vintagestory.API.Common.BlockEntity be = byEntity.World.BlockAccessor.GetBlockEntity(blockSel.Position);
        
        if (be is BlockEntityFarmland)
        {
            // decide if we should continue based on if it can be planted here
            if (!Attributes["isCrop"].AsBool()) return;

            // so far wildcraft herbs is the only mod to implement crops growing on farmland. would have to change variant based on item code
            Block cropBlock = byEntity.World.GetBlock(AssetLocation.Create(("crop-" + itemslot.Itemstack.Collectible.Variant["herbseedlings"] + "-1"), "wildcraftherb"));
            if (cropBlock == null) return;

            if (((BlockEntityFarmland)be).TryPlant(cropBlock, itemslot, byEntity, blockSel))
            {
                byEntity.World.PlaySoundAt(new AssetLocation("game:sounds/block/plant"), blockSel.Position.X, blockSel.Position.Y, blockSel.Position.Z, byPlayer);

                ((byEntity as EntityPlayer)?.Player as IClientPlayer)?.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);

                if (byPlayer?.WorldData?.CurrentGameMode != EnumGameMode.Creative)
                {
                    itemslot.TakeOut(1);
                    itemslot.MarkDirty();
                }
                handHandling = EnumHandHandling.PreventDefault;
            }
        }
        else
        {
            if (Attributes["isCrop"].AsBool()) return;
            
            // find correct variant based on what kind of seed it is
            Block plantBlock = api.World.GetBlock((AssetLocation.Create(
                (Attributes["isHerb"].AsBool() ? ("seedling-" + Variant["herbseedlings"] + "-planted") : ("groundberryseedling-" + Variant["type"] + "-planted")),
                (Attributes["isHerb"].AsBool() ? "wildcraftherb" : "wildcraftfruit")
                )));
            if (plantBlock == null) return;
            
            blockSel = blockSel.Clone();
            blockSel.Position.Up();

            EnumBlockMaterial mat = api.World.BlockAccessor.GetBlock(blockSel.Position).BlockMaterial;
            
            if (!byEntity.Controls.Sneak || (mat != EnumBlockMaterial.Air && Attributes["waterplant"].AsBool() && mat != EnumBlockMaterial.Liquid))
            {
                base.OnHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handHandling);
                return;
            }

            string failureCode = "";
            if (!plantBlock?.TryPlaceBlock(api.World, byPlayer, itemslot.Itemstack, blockSel, ref failureCode) ?? true)
            {
                if (api is ICoreClientAPI capi && failureCode != null && failureCode != "__ignore__")
                {
                    capi.TriggerIngameError(this, failureCode, Lang.Get("placefailure-" + failureCode));
                }
            }
            else
            {
                byEntity.World.PlaySoundAt(new AssetLocation("sounds/block/plant"), blockSel.Position.X + 0.5f, blockSel.Position.Y, blockSel.Position.Z + 0.5f, byPlayer);

                ((byEntity as EntityPlayer)?.Player as IClientPlayer)?.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);

                if (byPlayer?.WorldData?.CurrentGameMode != EnumGameMode.Creative)
                {
                    itemslot.TakeOut(1);
                    itemslot.MarkDirty();
                }
            }
            handHandling = EnumHandHandling.PreventDefault;
        }
    }
    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        
        if(Attributes["isCrop"].AsBool())
        {
            Block cropBlock = world.GetBlock(AssetLocation.Create(("crop-" + inSlot.Itemstack.Collectible.Variant[(Attributes["isHerb"].AsBool() ? "herbseedlings" : "type")] + "-1"),"wildcraftherb"));
            if (cropBlock == null || cropBlock.CropProps == null) return;

            dsc.AppendLine(Lang.Get("soil-nutrition-requirement") + cropBlock.CropProps.RequiredNutrient);
            dsc.AppendLine(Lang.Get("soil-nutrition-consumption") + cropBlock.CropProps.NutrientConsumption);

            double totalDays = cropBlock.CropProps.TotalGrowthDays;
            if (totalDays > 0)
            {
                var defaultTimeInMonths = totalDays / 12;
                totalDays = defaultTimeInMonths * world.Calendar.DaysPerMonth;
            } else
            {
                totalDays = cropBlock.CropProps.TotalGrowthMonths * world.Calendar.DaysPerMonth;
            }

            totalDays /= api.World.Config.GetDecimal("cropGrowthRateMul", 1);

            // if crop
            dsc.AppendLine(Lang.Get("soil-growth-time") + " " + Lang.Get("count-days", Math.Round(totalDays, 1)));
            dsc.AppendLine(Lang.Get("crop-coldresistance", Math.Round(cropBlock.CropProps.ColdDamageBelow, 1)));
            dsc.AppendLine(Lang.Get("crop-heatresistance", Math.Round(cropBlock.CropProps.HeatDamageAbove, 1)));
            
            // if crop and waterplant
            if (Attributes["waterplant"].AsBool()) dsc.AppendLine(Lang.Get("plantable-in-water-or-land"));
            return;
        }
        if (Attributes["waterplant"].AsBool())
        {
            // if not crop but is waterplant
            dsc.AppendLine(Lang.Get("plantable-in-water-or-land"));
            return;
        }
        
        // if not a crop or waterplant
        dsc.AppendLine(Lang.Get("plantable-on-normal-soil"));
    }
}