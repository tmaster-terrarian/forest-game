using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core;

public interface IDrawableSystem
{
    public void GetDrawables(GraphicsDevice graphicsDevice);
}
