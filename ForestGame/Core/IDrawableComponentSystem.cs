using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core;

public interface IDrawableComponentSystem
{
    public void GetDrawables(GraphicsDevice graphicsDevice);
}
