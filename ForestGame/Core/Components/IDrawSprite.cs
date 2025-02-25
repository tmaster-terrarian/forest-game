using Arch.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.Core.Components;

public interface IDrawSprite
{
    public void Draw(Entity entity, GameTime gameTime);
}
