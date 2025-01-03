using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools;

public class SelectTool : ChartingTool
{
    public override string Name => "Select";
    public override string Description => "Select and move objects";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = FontAwesome6.Solid.ArrowPointer };
    public override PlacementBlueprint CreateBlueprint() => null;
}
