using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class TextAspect : Aspect
{
    public string Text { get; set; }
    public Color Color { get; set; }

    /// <summary>
    /// origin in uv cooridinates (0 to 1)
    /// </summary>
    public Vector2 Origin { get; set; }

    protected override void Draw(Transform transform, GraphicsDevice graphicsDevice, Effect effect)
    {
        GraphicsUtil.DrawText(RenderPipeline.SpriteBatch, Text ?? "null", transform, Color, Origin);
    }
}
