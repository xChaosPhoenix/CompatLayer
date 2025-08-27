global using static CompatLayer.Constants;
using CompatLayer.Config;
using CompatLayer.Item;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Common;

namespace CompatLayer;

public class CompatLayerCore : ModSystem
{
    public static ILogger Logger { get; private set; }
    public static string Modid { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public static ConfigUniversal ConfigUniversal { get; set; }
    private static HarmonyLib.Harmony harmony;
    
    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        Api = api;
        Logger = Mod.Logger;
        Modid = Mod.Info.ModID;
        
        ConfigUniversal = ModConfig.ReadConfig<ConfigUniversal>(api, ConfigUniversalName);
        api.World.Config.SetBool("CompatLayer.ConfigWildcraftTreePatched", ConfigUniversal.ConfigWildcraftTreePatched);
        api.World.Config.SetBool("CompatLayer.ConfigWildcraftHerbPatched", ConfigUniversal.ConfigWildcraftHerbPatched);
        api.World.Config.SetBool("CompatLayer.ConfigWildcraftFruitPatched", ConfigUniversal.ConfigWildcraftFruitPatched);
        api.World.Config.SetBool("CompatLayer.ConfigAlchemyPatched", ConfigUniversal.ConfigAlchemyPatched);

        // TODO: IMPLEMENT CONFIG FOR WHAT HARMONY PATCHES TO ENABLE
        if (api.Side == EnumAppSide.Server)
        {
            harmony = new HarmonyLib.Harmony(Modid);
            harmony.PatchAll();
        }
        
        if (api.ModLoader.IsModEnabled("configlib"))
        {
            _ = new ConfigLibCompatibility(api);
        }
    }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        
        
        // Item Class Registration
        api.RegisterItemClass("ItemSeedCompat", typeof(ItemSeedCompat));
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
    }

    public override void Dispose()
    {
        harmony.UnpatchAll(Modid);
        Logger = null;
        Modid = null;
        Api = null;
        base.Dispose();
    }
}