using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class MotorMovementSystem : IComponentSystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor, Motor, Transform>(), (ref Actor actor, ref Motor motor, ref Transform transform) =>
        {
            var planarVel = MathUtil.ProjectOnPlane(actor.Velocity, transform.Matrix.Up);
            var verticalVel = MathUtil.Project(actor.Velocity, transform.Matrix.Up);

            var targetVel = Vector3.Zero;
            if (motor.MovementDirection != Vector3.Zero)
                targetVel = Vector3.Normalize(motor.MovementDirection) * motor.MaxSpeed;

            planarVel = MathUtil.ExpDecay(planarVel, targetVel, 12, Time.Delta);

            actor.Velocity = planarVel + verticalVel;
        });
    }
}
