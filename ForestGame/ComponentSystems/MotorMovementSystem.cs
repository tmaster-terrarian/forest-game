using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class MotorMovementSystem : IComponentSystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor, Motor>(), (ref Actor actor, ref Motor motor) =>
        {
            // if (actor.HasGravity)
            // {
            //     actor.Velocity += new Vector3(0, -9.81f, 0) * Time.Delta;
            // }

            var targetVel = Vector3.Normalize(motor.MovementDirection) * motor.MaxSpeed;
            // if(motor.MovementDirection.X == 0)
            //     targetVel.X = actor.Velocity.X;
            if(motor.MovementDirection.Y == 0)
                targetVel.Y = actor.Velocity.Y;
            // if(motor.MovementDirection.Z == 0)
            //     targetVel.Z = actor.Velocity.Z;

            actor.Velocity = MathUtil.MoveTo(
                actor.Velocity,
                targetVel,
                motor.Acceleration * Time.Delta,
                motor.Deceleration * Time.Delta
            );
        });
    }
}
