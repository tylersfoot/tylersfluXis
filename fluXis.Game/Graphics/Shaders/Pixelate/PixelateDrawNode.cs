using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.Shaders.Pixelate;

public partial class PixelateContainer
{
    private class PixelateContainerDrawNode : ShaderDrawNode<PixelateContainer>
    {
        private float strength;
        private IUniformBuffer<PixelateParameters> pixelateParametersBuffer;

        public PixelateContainerDrawNode(PixelateContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            strength = Source.Strength;
        }

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);

            if (strength > 0)
                drawFrameBuffer(renderer, strength);
        }

        private void drawFrameBuffer(IRenderer renderer, float strength)
        {
            pixelateParametersBuffer ??= renderer.CreateUniformBuffer<PixelateParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                pixelateParametersBuffer.Data = pixelateParametersBuffer.Data with
                {
                    TexSize = current.Size,
                    Strength = strength
                };

                Shader.BindUniformBlock("m_PixelateParameters", pixelateParametersBuffer);
                Shader.Bind();
                renderer.DrawFrameBuffer(current, new RectangleF(0, 0, current.Texture.Width, current.Texture.Height), ColourInfo.SingleColour(Color4.White));
                Shader.Unbind();
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            pixelateParametersBuffer?.Dispose();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct PixelateParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Strength;
            private readonly UniformPadding8 pad1;
            private readonly UniformPadding12 pad2;
        }
    }
}
