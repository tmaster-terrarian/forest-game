using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class OrientationArrows
{
    private GraphicsDevice GraphicsDevice { get; set; }
    private BasicEffect BasicEffect { get; set; }

    public OrientationArrows(GraphicsDevice graphicsDevice)
    {
        GraphicsDevice = graphicsDevice;
        BasicEffect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            Projection = RenderPipeline.ProjectionMatrix,
            View = RenderPipeline.ViewMatrix
        };
    }

}
