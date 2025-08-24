using ConfigLib;
using ImGuiNET;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace CompatLayer.Config;

public class ConfigLibCompatibility
{
    
    public ConfigLibCompatibility(ICoreAPI api)
    {
        api.ModLoader.GetModSystem<ConfigLibModSystem>().RegisterCustomConfig(Lang.Get(SettingsPrefix + "compatlayer-universal"), (id, buttons) => EditConfigUniversal(id, buttons, api));
    }

    private void EditConfigUniversal(string id, ControlButtons buttons, ICoreAPI api)
    {
        if(buttons.Save) ModConfig.WriteConfig(api, ConfigUniversalName, CompatLayerCore.ConfigUniversal);
        if(buttons.Restore) CompatLayerCore.ConfigUniversal = ModConfig.ReadConfig<ConfigUniversal>(api, ConfigUniversalName);
        if(buttons.Defaults) CompatLayerCore.ConfigUniversal = new(api);
        
        if(CompatLayerCore.ConfigUniversal != null) BuildSettingsUniversal(CompatLayerCore.ConfigUniversal, id, api);
    }

    private void BuildSettingsUniversal(ConfigUniversal config, string id, ICoreAPI api)
    {
        // Wildcraft trees menu
        if (api.ModLoader.IsModEnabled("wildcrafttree"))
        {
            if (ImGui.CollapsingHeader(Lang.Get(SettingsPrefix + "wildcrafttree-header") + $"##universal-wildcrafttreeSetting-{id}"))
            {
                ImGui.Indent();
                config.ConfigWildcraftTreePatched = OnCheckBox(id, config.ConfigWildcraftTreePatched, nameof(config.ConfigWildcraftTreePatched));
                ImGui.Unindent();
            }
        }
        
        // Wildcraft herbs menu
        if (api.ModLoader.IsModEnabled("wildcraftherb"))
        {
            if (ImGui.CollapsingHeader(Lang.Get(SettingsPrefix + "wildcraftherb-header") + $"##universal-wildcraftherbSetting-{id}"))
            {
                ImGui.Indent();
                config.ConfigWildcraftHerbPatched = OnCheckBox(id, config.ConfigWildcraftHerbPatched, nameof(config.ConfigWildcraftHerbPatched));
                ImGui.Unindent();
            }
        }
        
        // Wildcraft fruit menu
        if (api.ModLoader.IsModEnabled("wildcraftfruit"))
        {
            if (ImGui.CollapsingHeader(Lang.Get(SettingsPrefix + "wildcraftfruit-header") + $"##universal-wildcraftfruitSetting-{id}"))
            {
                ImGui.Indent();
                config.ConfigWildcraftFruitPatched = OnCheckBox(id, config.ConfigWildcraftFruitPatched, nameof(config.ConfigWildcraftFruitPatched));
                ImGui.Unindent();
            }
        }
        
        // Alchemy menu
        if (api.ModLoader.IsModEnabled("alchemy"))
        {
            if (ImGui.CollapsingHeader(Lang.Get(SettingsPrefix + "alchemy-header") + $"##universal-alchemySetting-{id}"))
            {
                ImGui.Indent();
                config.ConfigAlchemyPatched = OnCheckBox(id, config.ConfigAlchemyPatched, nameof(config.ConfigAlchemyPatched));
                ImGui.Unindent();
            }
        }
    }

    private bool OnCheckBox(string id, bool value, string name)
    {
        bool newValue = value;
        ImGui.Checkbox(Lang.Get(SettingsPrefix + name) + $"##{name}-{id}", ref newValue);
        return newValue;
    }
}