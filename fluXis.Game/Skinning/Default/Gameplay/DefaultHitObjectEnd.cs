using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Gameplay;

public partial class DefaultHitObjectEnd : Container
{
    private Box box;

    [BackgroundDependencyLoader]
    private void load()
    {
        CornerRadius = 10;
        Masking = true;
        Height = 42;
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Child = box = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }

    public void UpdateColor(int lane, int keyCount) => box.Colour = FluXisColors.GetLaneColor(lane, keyCount).Darken(.4f);
}