using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.ComponentSystems;

public class ModelGraphicsSystem : IComponentSystem, IDrawableComponentSystem
{
    public void Update() { }

    public void GetDrawables(GraphicsDevice graphicsDevice, Action<(Aspect aspect, Transform transform)> consumer)
    {
        EcsManager.world.Query(new QueryDescription().WithAll<AspectIdentity>(), (Entity entity, ref AspectIdentity aspectId) =>
        {
            Aspect? aspect = Registry<Aspect>.Get(aspectId.Id);
            if(aspect is null)
                return;

            bool hasTransform = entity.TryGet<Transform>(out var transform);

            consumer?.Invoke(new(aspect, hasTransform ? transform : Transform.Identity));
        });
    }
}
