using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class MotorMovementSystem : ISystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor, Motor, Transform>(), (ref Actor actor, ref Motor motor, ref Transform transform) =>
        {
            var targetVelocity = Vector3.Zero;
            if (motor.MovementDirection != Vector3.Zero)
                targetVelocity = Vector3.Normalize(motor.MovementDirection) * motor.MaxSpeed;
            bool hasGravity = actor.HasGravity;
            if (hasGravity && !actor.IsGrounded)
            {
                targetVelocity *= 0.1f;
            }

            actor.Velocity += targetVelocity * Time.Delta * 6f;
        });
    }
}
