using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.ColorShift;

public partial class ColorShiftContainer : ShaderContainer
{
    protected override string FragmentShader => "ColorShift";
    public override ShaderType Type => ShaderType.ColorShift;
    protected override DrawNode CreateShaderDrawNode() => new ColorShiftContainerDrawNode(this, SharedData);
}
