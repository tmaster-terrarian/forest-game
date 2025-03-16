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
            var vel = actor.Velocity;
            var targetVelocity = vel;
            if (motor.MovementDirection != Vector3.Zero)
                targetVelocity = Vector3.Normalize(motor.MovementDirection) * motor.MaxSpeed;

            vel = MathUtil.ExpDecay(vel, targetVelocity, 12, Time.Delta);

            actor.Velocity = vel;
        });
    }
}
