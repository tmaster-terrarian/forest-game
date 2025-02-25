using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core;

public abstract class ComponentSystem(QueryDescription query)
{
    public QueryDescription Query { get; } = query;

    public virtual void Update(GameTime gameTime)
    {
        
    }

    public virtual void Draw(GraphicsDevice graphicsDevice, GameTime gameTime)
    {
        
    }
}
