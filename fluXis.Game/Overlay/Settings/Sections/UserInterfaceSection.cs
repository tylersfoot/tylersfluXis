using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class UserInterfaceSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.LayerGroup;
    public override string Title => "UI";

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        Add(new SettingsToggle(config.GetBindable<bool>(FluXisSetting.MainMenuVisualizer), "Main Menu Visualizer"));
    }
}