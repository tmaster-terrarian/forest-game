using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public abstract class Renderer
{
    public RenderPipeline.EffectPass EffectPass { get; set; }
    public RenderPipeline.RenderPass RenderPass { get; set; }

    public string? MainTexturePath { get; set; }

    public bool Enabled { get; set; } = true;

    public abstract void Draw(GraphicsDevice graphicsDevice, EffectConfig effectConfig);
}
