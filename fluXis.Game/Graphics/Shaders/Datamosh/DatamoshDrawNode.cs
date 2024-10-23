using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.Shaders.Datamosh;

public partial class DatamoshContainer
{
    private class DatamoshContainerDrawNode : ShaderDrawNode<DatamoshContainer>
    {
        private float strength;
        private IUniformBuffer<DatamoshParameters> parametersBuffer;

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
            parametersBuffer ??= renderer.CreateUniformBuffer<DatamoshParameters>();

            // Get the current and previous frame buffers
            IFrameBuffer previous = SharedData.CurrentEffectBuffer;  // This holds the last drawn frame
            IFrameBuffer current = SharedData.GetNextEffectBuffer(); // This will be the target for the merged result

            // We are now rendering into 'current' which will hold the merged result
            using (BindFrameBuffer(current))
            {
                renderer.SetBlend(BlendingParameters.None);

                // Draw the current frame to the left half
                renderer.DrawFrameBuffer(previous, new RectangleF(0, 0, current.Texture.Width / 2, current.Texture.Height), ColourInfo.SingleColour(Color4.White));

                // Draw the previous frame to the right half
                renderer.DrawFrameBuffer(current, new RectangleF(current.Texture.Width / 2, 0, current.Texture.Width / 2, current.Texture.Height), ColourInfo.SingleColour(Color4.White));
            }

            // Now, render the merged buffer
            IFrameBuffer target = SharedData.GetNextEffectBuffer();
            using (BindFrameBuffer(target))
            {
                // Prepare and bind the uniform data for the shader
                parametersBuffer.Data = parametersBuffer.Data with
                {
                    TexSize = current.Size,
                    Strength = strength
                };

                Shader.BindUniformBlock("m_DatamoshParameters", parametersBuffer);
                Shader.Bind();

                // Draw the merged texture into the target framebuffer (final output to the screen)
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
        private record struct DatamoshParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Strength;
            private readonly UniformPadding8 pad1;
            private readonly UniformPadding12 pad2;
        }
    }
}
