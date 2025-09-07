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
                ImGui.Text(Lang.Get(SettingsPrefix + "wildcrafttree-info"));
                ImGui.Unindent();
            }
        }
        
        // Wildcraft herbs menu
        if (api.ModLoader.IsModEnabled("wildcraftherb"))
        {
            if (ImGui.CollapsingHeader(Lang.Get(SettingsPrefix + "wildcraftherb-header") + $"##universal-wildcraftherbSetting-{id}"))
            {
                ImGui.Indent();
                ImGui.Text(Lang.Get(SettingsPrefix + "wildcraftherb-info"));
                ImGui.Unindent();
            }
        }
        
        // Wildcraft fruit menu
        if (api.ModLoader.IsModEnabled("wildcraftfruit"))
        {
            if (ImGui.CollapsingHeader(Lang.Get(SettingsPrefix + "wildcraftfruit-header") + $"##universal-wildcraftfruitSetting-{id}"))
            {
                ImGui.Indent();
                ImGui.Text(Lang.Get(SettingsPrefix + "wildcraftfruit-info"));
                ImGui.Unindent();
            }
        }
        
        // Alchemy menu
        if (api.ModLoader.IsModEnabled("alchemy"))
        {
            if (ImGui.CollapsingHeader(Lang.Get(SettingsPrefix + "alchemy-header") + $"##universal-alchemySetting-{id}"))
            {
                ImGui.Indent();
                ImGui.Text(Lang.Get(SettingsPrefix + "alchemy-info"));
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