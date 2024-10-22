using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Graphics.Shaders.Glitch;
using fluXis.Game.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace fluXis.Game.Screens.Edit.Tabs.Design;

public partial class DesignShaderHandler : CompositeComponent
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public ShaderStackContainer ShaderStack { get; set; }

    protected override void Update()
    {
        base.Update();

        if (ShaderStack is null)
            return;

        var groups = map.MapEvents.ShaderEvents.GroupBy(x => x.Type);

        foreach (var group in groups)
            handleGroup(group.Key, group);
    }

    private void handleGroup(ShaderType type, IEnumerable<ShaderEvent> events)
    {
        var container = ShaderStack.GetShader(type);

        if (container == null)
            return;

        var current = events.LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (current == null)
        {
            container.Strength = 0;

            // Reset BlockSize and ColorRate if it's a GlitchContainer
            if (container is GlitchContainer glitchContainer)
            {
                glitchContainer.BlockSize = 0;
                glitchContainer.ColorRate = 0;
            }
            return;
        }

        var progress = (clock.CurrentTime - current.Time) / current.Duration;
        var endStrength = current.EndParameters.Strength;

        if (progress >= 1)
        {
            container.Strength = endStrength;

            // Set BlockSize and ColorRate if it's a GlitchContainer
            if (container is GlitchContainer glitchContainer)
            {
                glitchContainer.BlockSize = current.EndParameters.BlockSize;
                glitchContainer.ColorRate = current.EndParameters.ColorRate;
            }
            return;
        }

        var previous = events.LastOrDefault(e => e.Time < current.Time);
        var startStrength = current.UseStartParams ? current.StartParameters.Strength : previous?.EndParameters.Strength ?? 0;

        if (progress < 0)
        {
            container.Strength = startStrength;

            // Set initial BlockSize and ColorRate if it's a GlitchContainer
            if (container is GlitchContainer glitchContainer)
            {
                glitchContainer.BlockSize = current.StartParameters.BlockSize;
                glitchContainer.ColorRate = current.StartParameters.ColorRate;
            }
            return;
        }

        // Interpolate strength value
        container.Strength = Interpolation.ValueAt(clock.CurrentTime, startStrength, endStrength, current.Time, current.Time + current.Duration);

        // Interpolate BlockSize and ColorRate for GlitchContainer
        if (container is GlitchContainer glitch)
        {
            var startBlockSize = current.UseStartParams ? current.StartParameters.BlockSize : previous?.EndParameters.BlockSize ?? 0;
            var endBlockSize = current.EndParameters.BlockSize;
            glitch.BlockSize = Interpolation.ValueAt(clock.CurrentTime, startBlockSize, endBlockSize, current.Time, current.Time + current.Duration);

            var startColorRate = current.UseStartParams ? current.StartParameters.ColorRate : previous?.EndParameters.ColorRate ?? 0;
            var endColorRate = current.EndParameters.ColorRate;
            glitch.ColorRate = Interpolation.ValueAt(clock.CurrentTime, startColorRate, endColorRate, current.Time, current.Time + current.Duration);
        }
    }
}
