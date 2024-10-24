using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.ColorShift;

public partial class ColorShiftContainer : ShaderContainer
{
    public float Degrees { get; private set; }

    public override void ApplyShaderParameters(ShaderEvent shaderEvent)
    {
        if (shaderEvent.StartParameters.TryGetValue("Degrees", out var degreesParam) && degreesParam is SliderParameter degreesSlider)
        {
            Degrees = degreesSlider.Value;
        }

        Invalidate(Invalidation.DrawNode);
    }

    protected override string FragmentShader => "ColorShift";
    public override string ShaderName => "ColorShift";
    protected override DrawNode CreateShaderDrawNode() => new ColorShiftContainerDrawNode(this, SharedData);
}