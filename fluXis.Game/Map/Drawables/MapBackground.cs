using fluXis.Game.Database.Maps;
using fluXis.Game.Skinning;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Map.Drawables;

public partial class MapBackground : Sprite
{
    [CanBeNull]
    private RealmMap map;

    [Resolved]
    private SkinManager skinManager { get; set; }

    [CanBeNull]
    public RealmMap Map
    {
        get => map;
        set
        {
            map = value;

            if (IsLoaded)
                setTexture();
        }
    }

    private bool cropped { get; }

    public MapBackground([CanBeNull] RealmMap map, bool cropped = false)
    {
        this.map = map;
        this.cropped = cropped;

        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load() => setTexture();

    private void setTexture()
    {
        var custom = cropped ? Map?.GetPanelBackground() : Map?.GetBackground();
        Texture = custom ?? skinManager.GetDefaultBackground();
    }
}
