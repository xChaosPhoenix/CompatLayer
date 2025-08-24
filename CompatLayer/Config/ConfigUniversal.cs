using Vintagestory.API.Common;

namespace CompatLayer.Config;

public class ConfigUniversal : IModConfig
{
    // Wildcraft trees values
    public bool ConfigWildcraftTreePatched { get; set; } = true;
    
    // Wildcraft herbs values
    public bool ConfigWildcraftHerbPatched { get; set; } = true;
    
    // Wildcraft fruit values
    public bool ConfigWildcraftFruitPatched { get; set; } = true;
    
    // Alchemy values
    public bool ConfigAlchemyPatched { get; set; } = true;

    public ConfigUniversal(ICoreAPI api, ConfigUniversal previousConfig = null)
    {
        if (previousConfig == null) return;
        
        // Wildcraft trees config
        ConfigWildcraftTreePatched = previousConfig.ConfigWildcraftTreePatched;
        if (!api.ModLoader.IsModEnabled("wildcrafttree")) ConfigWildcraftTreePatched = false; // Disable patch if not installed
        
        // Wildcraft herbs config
        ConfigWildcraftHerbPatched = previousConfig.ConfigWildcraftHerbPatched;
        if (!api.ModLoader.IsModEnabled("wildcraftherb")) ConfigWildcraftHerbPatched = false; // Disable patch if not installed
        
        // Wildcraft fruit config
        ConfigWildcraftFruitPatched = previousConfig.ConfigWildcraftFruitPatched;
        if (!api.ModLoader.IsModEnabled("wildcraftfruit")) ConfigWildcraftFruitPatched = false; // Disable patch if not installed
        
        // Alchemy config
        ConfigAlchemyPatched = previousConfig.ConfigAlchemyPatched;
        if (!api.ModLoader.IsModEnabled("alchemy")) ConfigAlchemyPatched = false; // Disable patch if not installed
    }
}