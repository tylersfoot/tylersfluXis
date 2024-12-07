using System;
using System.Collections.Generic;
using fluXis.Game.Import;
using fluXis.Game.Overlay.Settings.UI;
using fluXis.Game.Plugins;
using osu.Framework.Platform;

namespace fluXis.Import.Meow;

public class MeowPlugin : Plugin
{
    public override string Name => "Hell yeah";
    public override string Author => "tylersfoot";
    public override Version Version => new(1, 0, 0);

    private MeowPluginConfig config;

    protected override MapImporter CreateImporter() => new MeowImport(config);
    public override void CreateConfig(Storage storage) => config = new MeowPluginConfig(storage);

    public override List<SettingsItem> CreateSettings() => new()
    {
        new SettingsTextBox
        {
            Label = "Meow Directory",
            Description = "The directory where Meow is installed.",
            Bindable = config.GetBindable<string>(MeowPluginSetting.GameLocation)
        }
    };
}
