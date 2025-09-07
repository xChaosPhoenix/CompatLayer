using Vintagestory.API.Common;

namespace CompatLayer.Config;

public class ConfigUniversal : IModConfig
{
    // Wildcraft trees values
    
    // Wildcraft herbs values
    
    // Wildcraft fruit values
    
    // Alchemy values

    public ConfigUniversal(ICoreAPI api, ConfigUniversal previousConfig = null)
    {
        if (previousConfig == null) return;
    }
}