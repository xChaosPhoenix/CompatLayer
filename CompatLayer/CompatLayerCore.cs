global using static CompatLayer.Constants;
using CompatLayer.Config;
using CompatLayer.Item;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace CompatLayer;

public class CompatLayerCore : ModSystem
{
    public static ILogger Logger { get; private set; }
    public static string Modid { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public static ConfigUniversal ConfigUniversal { get; set; }
    private static HarmonyLib.Harmony harmony;
    
    /* IMPORTANT
     * TODO: FIND MISSING TEXTURE ISSUES (VSMODDB COMMENT AKSYL)
     * TODO: FIX / UPDATE EXPANDED FOODS COMPATABILITY PATCH FOR WILDCRAFT FRUIT
     *
     * LESS IMPORTANT
     * TODO: IMPLEMENT CONFIG FOR WHAT HARMONY PATCHES TO ENABLE
     * TODO: FIND BETTER SOLUTION TO HERBARIUM PATCHES OTHER THAN USING HARMONY
     * TODO: VERIFY MEMORY LEAK ISSUE SOLVED
     */
    
    
    
    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        Api = api;
        Logger = Mod.Logger;
        Modid = Mod.Info.ModID;
        
        ConfigUniversal = ModConfig.ReadConfig<ConfigUniversal>(api, ConfigUniversalName);
        
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