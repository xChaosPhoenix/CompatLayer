using System;
using Vintagestory.API.Common;

namespace CompatLayer.Config;

public class ConfigLoader : ModSystem
{
    private const string ConfigName = "CompatLayer.json";
    public static ModConfig Config { get; private set; }

    public override void StartPre(ICoreAPI api)
    {
        try
        {
            Config = api.LoadModConfig<ModConfig>(ConfigName);
            if (Config == null)
            {
                Config = new ModConfig();
                Mod.Logger.VerboseDebug("Config file not found, creating a new one...");
            }

            api.StoreModConfig(Config, ConfigName);
        }
        catch (Exception e)
        {
            Mod.Logger.Error("Failed to load config, you probably made a typo: {0}", e);
            Config = new ModConfig();
        }
    }

    public override void Dispose()
    {
        Config = null;
        base.Dispose();
    }

    public override void Start(ICoreAPI api)
    {
        // Properties can be used in json patches like this
        // "condition": { "when": "CompatLayer_ExampleProperty", "isValue": "true" }
        api.World.Config.SetBool("CompatLayer_ExampleProperty", Config.ExampleConfigSetting);
    }
}