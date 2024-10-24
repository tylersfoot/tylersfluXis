using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Glitch;

public partial class GlitchContainer : ShaderContainer
{
    public float Strength { get; private set; }
    public float BlockSize { get; private set; }
    public float ColorRate { get; private set; }

    public override void ApplyShaderParameters(ShaderEvent shaderEvent)
    {
        if (shaderEvent.StartParameters.TryGetValue("Strength", out var strengthParam) && strengthParam is SliderParameter strengthSlider)
        {
            Strength = strengthSlider.Value;
        }
        
        if (shaderEvent.StartParameters.TryGetValue("BlockSize", out var blockSizeParam) && blockSizeParam is SliderParameter blockSizeSlider)
        {
            BlockSize = blockSizeSlider.Value;
        }

        if (shaderEvent.StartParameters.TryGetValue("ColorRate", out var colorRateParam) && colorRateParam is SliderParameter colorRateSlider)
        {
            ColorRate = colorRateSlider.Value;
        }

        Invalidate(Invalidation.DrawNode);
    }

    protected override string FragmentShader => "Glitch";
    public override string ShaderName => "Glitch";

    protected override DrawNode CreateShaderDrawNode() => new GlitchContainerDrawNode(this, SharedData);
}