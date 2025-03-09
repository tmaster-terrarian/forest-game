using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Components;

public interface IDrawModel : IRequiresTransform, IRequiresEffect
{
    public void Draw(Entity entity, GraphicsDevice graphicsDevice, Matrix world, Effect effect);
}
