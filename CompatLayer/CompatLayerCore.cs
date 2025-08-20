using CompatLayer.Config;
using CompatLayer.Item;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;

namespace CompatLayer;

public class CompatLayerCore : ModSystem
{
    public static ILogger Logger { get; private set; }
    public static string Modid { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public static ModConfig Config => ConfigLoader.Config;

    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        Api = api;
        Logger = Mod.Logger;
        Modid = Mod.Info.ModID;
    }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        
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
        Logger = null;
        Modid = null;
        Api = null;
        base.Dispose();
    }
}