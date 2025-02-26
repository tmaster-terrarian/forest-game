using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Components;

public record struct Sprite(Texture2D Texture, Rectangle? SourceRect, Color Color, SpriteEffects Effects)
{
    public readonly void Draw(SpriteBatch spriteBatch, Entity entity)
    {
        if(!entity.TryGet(out Transform transform))
            transform = Transform.Identity;

        var rot = Vector3.Transform(Vector3.UnitX, transform.Rotation);

        spriteBatch.Draw(
            Texture,
            new Vector2(transform.Position.X, transform.Position.Y),
            SourceRect,
            Color,
            MathF.Atan2(rot.Y, rot.X),
            new Vector2(transform.Origin.X, transform.Origin.Y),
            new Vector2(transform.Scale.X, transform.Scale.Y),
            Effects,
            0
        );
    }

    public readonly void Draw(GraphicsDevice graphicsDevice, Matrix world, Entity entity)
    {
        if(!entity.TryGet(out Transform transform))
            transform = Transform.Identity;

        var rot = Vector3.Transform(Vector3.UnitX, transform.Rotation);

        var srcRect = SourceRect ?? new(0, 0, Texture.Width, Texture.Height);

        GraphicsUtil.DrawQuad(
            graphicsDevice,
            Texture,
            Color,
            Matrix.CreateTranslation(0.5f, -0.5f, 0.5f) * transform * world,
            RenderPipeline.EffectBasicDiffuse,
            1,
            1,
            srcRect.Location.ToVector2(),
            (srcRect.Location + srcRect.Size).ToVector2()
        );
    }
}
