using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;

namespace ForestGame.ComponentSystems;

public class ColliderSyncSystem : IComponentSystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor, Transform>(),
            (ref Actor actor, ref Transform transform) =>
            {
                actor.Collider = actor.Collider with { Position = transform.Position };
            });

        EcsManager.world.Query(new QueryDescription().WithAll<Solid, Transform>(),
            (ref Solid solid, ref Transform transform) =>
            {
                solid.Collider = solid.Collider with { Position = transform.Position };
            });
    }
}
