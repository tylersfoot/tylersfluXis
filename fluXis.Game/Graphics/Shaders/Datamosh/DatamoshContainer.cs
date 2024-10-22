using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Datamosh;

public partial class DatamoshContainer : ShaderContainer
{
    protected override string FragmentShader => "Datamosh";
    public override ShaderType Type => ShaderType.Datamosh;
    protected override DrawNode CreateShaderDrawNode() => new DatamoshContainerDrawNode(this, SharedData);
}
