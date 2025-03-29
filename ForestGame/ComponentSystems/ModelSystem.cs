using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.ComponentSystems;

public class ModelGraphicsSystem : ISystem, ISystem.Drawable
{
    public void Update() { }

    public void GetDrawables(GraphicsDevice graphicsDevice)
    {
        EcsManager.world.Query(new QueryDescription().WithAll<AspectIdentity>(),
            (Entity entity, ref AspectIdentity aspectId) => {
                Aspect? aspect = Registry<Aspect>.Get(aspectId.Id);
                if(aspect is null)
                    return;

                if(!entity.TryGet<Transform>(out var transform))
                    aspect.Submit(Transform.Identity);
                else
                    aspect.Submit(transform);
            }
        );
    }
}
