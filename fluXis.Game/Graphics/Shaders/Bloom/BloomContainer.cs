using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Graphics.Shaders.Bloom;

public partial class BloomContainer : ShaderContainer
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

    protected override string FragmentShader => "Bloom";
    public override string ShaderName => "Bloom";

    public BloomContainer()
    {
        DrawOriginal = true;
        EffectBlending = BlendingParameters.Additive;
        EffectPlacement = EffectPlacement.InFront;
    }

    protected override DrawNode CreateShaderDrawNode() => new BloomContainerDrawNode(this, SharedData);
}
