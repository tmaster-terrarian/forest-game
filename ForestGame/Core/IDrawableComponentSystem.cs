using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core;

public interface IDrawableComponentSystem : IComponentSystem
{
    public void GetDrawables(GraphicsDevice graphicsDevice, Action<(Aspect aspect, Transform transform)> consumer);
}
