using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public interface IModel
{
    public void Draw(GraphicsDevice graphicsDevice, Matrix world, Effect effect);

    public static abstract IModel Load(string path);
}
