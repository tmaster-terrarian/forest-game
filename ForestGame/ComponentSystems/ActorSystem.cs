using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class ActorSystem : IComponentSystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor, Transform>(), (ref Actor actor, ref Transform transform) =>
        {
            if (actor.HasGravity)
            {
                actor.Velocity += new Vector3(0, -9.81f, 0) * Time.Delta;
            }
            transform.Position += actor.Velocity * Time.Delta;

            transform.Position = transform.Position with {
                Y = MathHelper.Max(transform.Position.Y, 0),
            };
        });
    }
}
