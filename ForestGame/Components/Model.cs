using Arch.Core;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Components;

public record struct Model<M>(string ModelPath) : IDrawModel where M : class, IModel
{
    public M? ModelInstance { get; private set; }

    public readonly M GetModel() => ContentLoader.Load<M>(ModelPath)!;

    public void Draw(Entity entity, GraphicsDevice graphicsDevice, Matrix world, Effect effect)
    {
        ModelInstance ??= GetModel();
        ModelInstance.Draw(graphicsDevice, world, effect);
    }

    public void UnloadModel()
    {
        ModelInstance = default;
    }
}
