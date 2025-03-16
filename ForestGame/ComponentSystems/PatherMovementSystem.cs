using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class PatherMovementSystem : ISystem
{
    public void Update()
    {
        EcsManager.world.Query(
            new QueryDescription()
                .WithAll<Pather, Motor, Transform>(),
            (ref Pather pather, ref Motor motor, ref Transform transform) =>
            {
                var direction = Vector3.Normalize(pather.TargetPosition - transform.WorldPosition);
                if (!pather.IsPathing) direction = Vector3.Zero;
                motor.MovementDirection = direction;
            }
        );
    }
}
