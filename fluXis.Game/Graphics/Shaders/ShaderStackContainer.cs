using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace fluXis.Game.Graphics.Shaders;

public partial class ShaderStackContainer : CompositeDrawable
{
    private readonly List<ShaderContainer> shaders = new();

    // public IReadOnlyList<ShaderType> ShaderTypes => shaders.DistinctBy(x => x.Type).Select(x => x.Type).ToList();
    public IReadOnlyList<string> ShaderNames => shaders.DistinctBy(x => x.ShaderName).Select(x => x.ShaderName).ToList();

    public ShaderStackContainer()
    {
        RelativeSizeAxes = Axes.Both;
    }

    public void AddShader(ShaderContainer shader)
    {
        Logger.Log($"Adding shader {shader.GetType().Name} to stack", LoggingTarget.Runtime, LogLevel.Debug);

        if (shaders.Count == 0)
            InternalChild = shader;
        else
            shaders.Last().Add(shader);

        shaders.Add(shader);
        Logger.Log($"{shaders.Count} shaders now in stack", LoggingTarget.Runtime, LogLevel.Debug);
    }

    public ShaderStackContainer AddContent(Drawable[] content)
    {
        if (shaders.Count == 0)
            InternalChildren = content;
        else
            shaders.Last().AddRange(content);

        return this;
    }

    public IEnumerable<Drawable> RemoveContent()
    {
        IEnumerable<Drawable> children;

        if (shaders.Count == 0)
        {
            children = InternalChildren;
            ClearInternal(false);
        }
        else
        {
            var last = shaders.Last();
            children = last.Children.ToArray();
            last.Clear(false);
        }

        return children;
    }

    // Fetch shader by its container type
    public T GetShader<T>() where T : ShaderContainer
        => shaders.FirstOrDefault(s => s.GetType() == typeof(T)) as T;

    // Fetch shader by its ShaderName instead of ShaderType
    public ShaderContainer GetShader(string shaderName)
        => shaders.FirstOrDefault(s => s.ShaderName == shaderName);
}
