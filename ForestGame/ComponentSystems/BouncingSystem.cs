using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class BouncingSystem : IComponentSystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor, Transform, Bouncy>(),
            (ref Actor actor, ref Transform transform, ref Bouncy bouncy) =>
            {
                if (transform.Position.Y < 0)
                {
                    transform.Position = transform.Position with { Y = -transform.Position.Y };
                    actor.Velocity = actor.Velocity with { Y = -actor.Velocity.Y };
                }

                transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI * Time.Delta);
            });
    }
}
