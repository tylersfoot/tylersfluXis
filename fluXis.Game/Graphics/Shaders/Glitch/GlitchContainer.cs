using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Glitch;

public partial class GlitchContainer : ShaderContainer
{
    private float blockSize;
    private float colorRate;

    public float BlockSize
    {
        get => blockSize;
        set
        {
            if (value == blockSize)
                return;

            blockSize = value;
            Invalidate(Invalidation.DrawNode);
        }
    }

    public float ColorRate
    {
        get => colorRate;
        set
        {
            if (value == colorRate)
                return;

            colorRate = value;
            Invalidate(Invalidation.DrawNode);
        }
    }

    protected override string FragmentShader => "Glitch";
    public override ShaderType Type => ShaderType.Glitch;
    protected override DrawNode CreateShaderDrawNode() => new GlitchContainerDrawNode(this, SharedData);
}