using Arch.Core;
using ForestGame.Core;
using ForestGame.Core.Components;

namespace ForestGame.ComponentSystems;

public class PhysicsSystem : IComponentSystem
{
    private static readonly QueryDescription _velocityQuery = new QueryDescription().WithAll<Velocity, Transform>();

    public void Update()
    {
        EcsManager.world.Query(_velocityQuery, (Entity entity, ref Velocity vel, ref Transform transform) =>
        {
            transform.Position += vel.Delta * Time.Delta;
        });
    }
}
