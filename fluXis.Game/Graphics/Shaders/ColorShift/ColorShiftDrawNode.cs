using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;
using fluXis.Game.Graphics.Shaders;
using System.Linq;

namespace fluXis.Game.Graphics.Shaders.ColorShift;

public partial class ColorShiftContainer
{
    private class ColorShiftContainerDrawNode : ShaderDrawNode<ColorShiftContainer>
    {
        private IUniformBuffer<ColorShiftParameters> colorShiftParametersBuffer;

        public ColorShiftContainerDrawNode(ColorShiftContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            var shaderParams = ShaderSettings.Shaders[Source.Type.ToString()].Parameters;

            if (shaderParams.TryGetValue("Degrees", out var parameter) && parameter is SliderParameter slider)
            {
                sliderValue = slider.Value;
            }
        }

        private float sliderValue = 0;

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);

            if (sliderValue > 0)
                drawFrameBuffer(renderer, sliderValue);
        }

        private void drawFrameBuffer(IRenderer renderer, float value)
        {
            colorShiftParametersBuffer ??= renderer.CreateUniformBuffer<ColorShiftParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                colorShiftParametersBuffer.Data = colorShiftParametersBuffer.Data with
                {
                    TexSize = current.Size,
                    Strength = value
                };

                Shader.BindUniformBlock("m_ColorShiftParameters", colorShiftParametersBuffer);
                Shader.Bind();
                renderer.DrawFrameBuffer(current, new RectangleF(0, 0, current.Texture.Width, current.Texture.Height), ColourInfo.SingleColour(Color4.White));
                Shader.Unbind();
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            colorShiftParametersBuffer?.Dispose();
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct ColorShiftParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Strength;
            private readonly UniformPadding8 pad1;
            private readonly UniformPadding12 pad2;
        }
    }
}



// namespace fluXis.Game.Graphics.Shaders.ColorShift;

// public partial class ColorShiftContainer
// {
//     private class ColorShiftContainerDrawNode : ShaderDrawNode<ColorShiftContainer>
//     {
//         private float strength;
//         private IUniformBuffer<ColorShiftParameters> colorShiftParametersBuffer;

//         public ColorShiftContainerDrawNode(ColorShiftContainer source, BufferedDrawNodeSharedData sharedData)
//             : base(source, sharedData)
//         {
//         }

//         public override void ApplyState()
//         {
//             base.ApplyState();

//             strength = Source.Strength;
//         }

//         protected override void PopulateContents(IRenderer renderer)
//         {
//             base.PopulateContents(renderer);

//             if (strength > 0)
//                 drawFrameBuffer(renderer, strength);
//         }

//         private void drawFrameBuffer(IRenderer renderer, float strength)
//         {
//             colorShiftParametersBuffer ??= renderer.CreateUniformBuffer<ColorShiftParameters>();

//             IFrameBuffer current = SharedData.CurrentEffectBuffer;
//             IFrameBuffer target = SharedData.GetNextEffectBuffer();

//             renderer.SetBlend(BlendingParameters.None);

//             using (BindFrameBuffer(target))
//             {
//                 colorShiftParametersBuffer.Data = colorShiftParametersBuffer.Data with
//                 {
//                     TexSize = current.Size,
//                     Strength = strength
//                 };

//                 Shader.BindUniformBlock("m_ColorShiftParameters", colorShiftParametersBuffer);
//                 Shader.Bind();
//                 renderer.DrawFrameBuffer(current, new RectangleF(0, 0, current.Texture.Width, current.Texture.Height), ColourInfo.SingleColour(Color4.White));
//                 Shader.Unbind();
//             }
//         }

//         protected override void Dispose(bool isDisposing)
//         {
//             base.Dispose(isDisposing);
//             colorShiftParametersBuffer?.Dispose();
//         }

//         [StructLayout(LayoutKind.Sequential, Pack = 1)]
//         private record struct ColorShiftParameters
//         {
//             public UniformVector2 TexSize;
//             public UniformFloat Strength;
//             private readonly UniformPadding8 pad1;
//             private readonly UniformPadding12 pad2;
//         }
//     }
// }
