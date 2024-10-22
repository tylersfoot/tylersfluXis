using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.Shaders.Datamosh;

public partial class DatamoshContainer
{
    private class DatamoshContainerDrawNode : ShaderDrawNode<DatamoshContainer>
    {
        private float strength;
        private IUniformBuffer<DatamoshParameters> datamoshParametersBuffer;

        public DatamoshContainerDrawNode(DatamoshContainer source, BufferedDrawNodeSharedData sharedData)
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
            datamoshParametersBuffer ??= renderer.CreateUniformBuffer<DatamoshParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                datamoshParametersBuffer.Data = datamoshParametersBuffer.Data with
                {
                    TexSize = current.Size,
                    Strength = strength
                };

                Shader.BindUniformBlock("m_DatamoshParameters", datamoshParametersBuffer);
                Shader.Bind();
                renderer.DrawFrameBuffer(current, new RectangleF(0, 0, current.Texture.Width, current.Texture.Height), ColourInfo.SingleColour(Color4.White));
                Shader.Unbind();
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            datamoshParametersBuffer?.Dispose();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct DatamoshParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Strength;
            private readonly UniformPadding8 pad1;
            private readonly UniformPadding12 pad2;
        }
    }
}
