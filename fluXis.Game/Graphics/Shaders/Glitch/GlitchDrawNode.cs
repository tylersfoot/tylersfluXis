using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.Shaders.Glitch;

public partial class GlitchContainer
{
    private class GlitchContainerDrawNode : ShaderDrawNode<GlitchContainer>
    {
        private float strength;
        private float blockSize;
        private float colorRate;
        private IUniformBuffer<GlitchParameters> parametersBuffer;

        public GlitchContainerDrawNode(GlitchContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            strength = Source.Strength / 10f;
            blockSize = Source.BlockSize;
            colorRate = Source.ColorRate / 20f;
        }

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);

            if (strength > 0)
                drawFrameBuffer(renderer, strength, blockSize, colorRate);
        }

        private void drawFrameBuffer(IRenderer renderer, float strength, float blockSize, float colorRate)
        {
            parametersBuffer ??= renderer.CreateUniformBuffer<GlitchParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                parametersBuffer.Data = new GlitchParameters
                {
                    TexSize = current.Size,
                    Strength = strength,
                    Time = (float)Source.Time.Current % 10000f,
                    BlockSize = blockSize,
                    ColorRate = colorRate
                };

                Shader.BindUniformBlock("m_GlitchParameters", parametersBuffer);
                Shader.Bind();
                renderer.DrawFrameBuffer(current, new RectangleF(0, 0, current.Texture.Width, current.Texture.Height), ColourInfo.SingleColour(Color4.White));
                Shader.Unbind();
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            parametersBuffer?.Dispose();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct GlitchParameters
        {
            public UniformVector2 TexSize; // Texture size
            public UniformFloat Strength; // Vibration strength
            public UniformFloat Time; // Time value
            public UniformFloat BlockSize; // Vibration block size
            public UniformFloat ColorRate; // Color separation rate (0.0 to 1.0)
            private readonly UniformPadding8 padding; // Add this to align to 32 bytes
        }
    }
}