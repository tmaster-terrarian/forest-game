using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;

namespace ForestGame.ComponentSystems;

public class ColliderSyncSystem : IComponentSystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Collider, Transform>(),
            (ref Collider collider, ref Transform transform) => {
                collider = collider with { Position = transform.Position };
            }
        );
    }
}
