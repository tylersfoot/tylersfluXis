using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Pixelate;

public partial class PixelateContainer : ShaderContainer
{
    public float Strength { get; private set; }

    public override void ApplyShaderParameters(ShaderEvent shaderEvent)
    {
        if (shaderEvent.StartParameters.TryGetValue("Strength", out var strengthParam) && strengthParam is SliderParameter strengthSlider)
        {
            Strength = strengthSlider.Value;
        }

        Invalidate(Invalidation.DrawNode);
    }

    protected override string FragmentShader => "Pixelate";
    public override string ShaderName => "Pixelate";
    protected override DrawNode CreateShaderDrawNode() => new PixelateContainerDrawNode(this, SharedData);
}