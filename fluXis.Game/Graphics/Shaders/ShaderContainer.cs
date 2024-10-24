using fluXis.Game.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Layout;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;

namespace fluXis.Game.Graphics.Shaders;

public abstract partial class ShaderContainer : Container, IBufferedDrawable
{
    protected virtual string VertexShader => VertexShaderDescriptor.TEXTURE_2;
    protected abstract string FragmentShader { get; }
    // public abstract ShaderType Type { get; }
    public abstract string ShaderName { get; }
    protected abstract DrawNode CreateShaderDrawNode();

    // Parameters dictionary for storing and applying dynamic parameters (like Strength, BlockSize, etc.)
    protected Dictionary<string, ShaderParameter> Parameters { get; private set; } = new();

    /// <summary>
    /// Applies shader parameters based on the provided ShaderEvent.
    /// </summary>
    /// <param name="shaderEvent">The ShaderEvent containing the parameters to apply.</param>
    public virtual void ApplyShaderParameters(ShaderEvent shaderEvent)
    {
        // Set parameters from the shader event
        foreach (var param in shaderEvent.EndParameters)
        {
            if (Parameters.ContainsKey(param.Key))
            {
                if (param.Value is SliderParameter slider)
                {
                    if (Parameters[param.Key] is SliderParameter containerSlider)
                    {
                        containerSlider.Value = slider.Value;
                        Invalidate(Invalidation.DrawNode); // Ensure the draw node is updated when parameters change
                    }
                }
                // Add other parameter types here if necessary (like CheckboxParameter, DropdownParameter, etc.)
            }
        }
    }

    /// <summary>
    /// Initializes shader parameters based on the shader's settings from ShaderSettings.
    /// </summary>
    public void InitializeShaderParameters()
    {
        if (ShaderSettings.Shaders.TryGetValue(ShaderName, out var shaderInfo))
        {
            // Initialize shader parameters based on ShaderSettings
            foreach (var param in shaderInfo.Parameters)
            {
                Parameters[param.Key] = param.Value.Clone(); // Create a deep copy of the parameter
            }
        }
    }

    public bool DrawOriginal { get; set; }
    public ColourInfo EffectColour { get; set; } = Color4.White;
    public BlendingParameters EffectBlending { get; set; } = BlendingParameters.Inherit;
    public EffectPlacement EffectPlacement { get; set; }
    public Color4 BackgroundColour { get; set; } = new(0, 0, 0, 0);
    public Vector2 FrameBufferScale { get; set; } = Vector2.One;

    public BlendingParameters DrawEffectBlending
    {
        get
        {
            BlendingParameters blending = EffectBlending;

            blending.CopyFromParent(Blending);
            blending.ApplyDefaultToInherited();

            return blending;
        }
    }

    protected override bool RequiresChildrenUpdate => base.RequiresChildrenUpdate && childrenUpdateVersion != updateVersion;

    public IShader TextureShader { get; private set; }
    public DrawColourInfo? FrameBufferDrawColour => base.DrawColourInfo;

    protected BufferedDrawNodeSharedData SharedData { get; } = new(2, null, false, true);
    private IShader shader { get; set; }

    private long updateVersion;
    private long childrenUpdateVersion = -1;

    [BackgroundDependencyLoader]
    private void load(ShaderManager shaders)
    {
        TextureShader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE);
        shader = shaders.Load(VertexShader, FragmentShader);

        // Initialize shader parameters during loading
        InitializeShaderParameters();
    }

    protected void ForceRedraw() => Invalidate(Invalidation.DrawNode);

    protected override DrawNode CreateDrawNode() => CreateShaderDrawNode();

    public override bool UpdateSubTreeMasking()
    {
        bool result = base.UpdateSubTreeMasking();

        childrenUpdateVersion = updateVersion;
        return result;
    }

    protected override RectangleF ComputeChildMaskingBounds() => ScreenSpaceDrawQuad.AABBFloat;

    protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
    {
        bool result = base.OnInvalidate(invalidation, source);

        if ((invalidation & Invalidation.DrawNode) <= 0) return result;

        updateVersion++;
        return true;
    }

    protected override void Update()
    {
        base.Update();
        ForceRedraw();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        SharedData.Dispose();
    }
}


// using fluXis.Game.Map.Structures.Events;
// using osu.Framework.Allocation;
// using osu.Framework.Graphics;
// using osu.Framework.Graphics.Colour;
// using osu.Framework.Graphics.Containers;
// using osu.Framework.Graphics.Primitives;
// using osu.Framework.Graphics.Shaders;
// using osu.Framework.Layout;
// using osuTK;
// using osuTK.Graphics;

// namespace fluXis.Game.Graphics.Shaders;

// public abstract partial class ShaderContainer : Container, IBufferedDrawable
// {
//     protected virtual string VertexShader => VertexShaderDescriptor.TEXTURE_2;
//     protected abstract string FragmentShader { get; }
//     public abstract ShaderType Type { get; }
//     protected abstract DrawNode CreateShaderDrawNode();

//     private float strength;

//     /// <summary>
//     /// The strength of the mosaic effect. From 0 to 1.
//     /// <br/>
//     /// 0 means its full resolution, 1 means its 1x1 pixel.
//     /// </summary>
//     public float Strength
//     {
//         get => strength;
//         set
//         {
//             if (value == strength)
//                 return;

//             strength = value;
//             Invalidate(Invalidation.DrawNode);
//         }
//     }

//     public bool DrawOriginal { get; set; }
//     public ColourInfo EffectColour { get; set; } = Color4.White;
//     public BlendingParameters EffectBlending { get; set; } = BlendingParameters.Inherit;
//     public EffectPlacement EffectPlacement { get; set; }
//     public Color4 BackgroundColour { get; set; } = new(0, 0, 0, 0);
//     public Vector2 FrameBufferScale { get; set; } = Vector2.One;

//     public BlendingParameters DrawEffectBlending
//     {
//         get
//         {
//             BlendingParameters blending = EffectBlending;

//             blending.CopyFromParent(Blending);
//             blending.ApplyDefaultToInherited();

//             return blending;
//         }
//     }

//     protected override bool RequiresChildrenUpdate => base.RequiresChildrenUpdate && childrenUpdateVersion != updateVersion;

//     public IShader TextureShader { get; private set; }
//     public DrawColourInfo? FrameBufferDrawColour => base.DrawColourInfo;

//     protected BufferedDrawNodeSharedData SharedData { get; } = new(2, null, false, true);
//     private IShader shader { get; set; }

//     private long updateVersion;
//     private long childrenUpdateVersion = -1;

//     [BackgroundDependencyLoader]
//     private void load(ShaderManager shaders)
//     {
//         TextureShader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE);
//         shader = shaders.Load(VertexShader, FragmentShader);
//     }

//     protected void ForceRedraw() => Invalidate(Invalidation.DrawNode);

//     protected override DrawNode CreateDrawNode() => CreateShaderDrawNode();

//     public override bool UpdateSubTreeMasking()
//     {
//         bool result = base.UpdateSubTreeMasking();

//         childrenUpdateVersion = updateVersion;
//         return result;
//     }

//     protected override RectangleF ComputeChildMaskingBounds() => ScreenSpaceDrawQuad.AABBFloat;

//     protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
//     {
//         bool result = base.OnInvalidate(invalidation, source);

//         if ((invalidation & Invalidation.DrawNode) <= 0) return result;

//         updateVersion++;
//         return true;
//     }

//     protected override void Update()
//     {
//         base.Update();
//         ForceRedraw();
//     }

//     protected override void Dispose(bool isDisposing)
//     {
//         base.Dispose(isDisposing);
//         SharedData.Dispose();
//     }
// }
