using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;

namespace ForestGame.ComponentSystems;

public class ColliderSyncSystem : ISystem, ISystem.EditorUpdate
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Collider, Transform>(),
            (Entity entity, ref Collider collider, ref Transform transform) => {
                entity.Set(collider with { Position = transform.WorldPosition });
            }
        );
    }
}
