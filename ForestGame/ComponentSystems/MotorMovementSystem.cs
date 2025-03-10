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
            var planarVel = MathUtil.ProjectOnPlane(actor.Velocity, Vector3.UnitY);
            var verticalVel = MathUtil.Project(actor.Velocity, Vector3.UnitY);

            var targetVel = Vector3.Zero;
            if (motor.MovementDirection != Vector3.Zero)
                targetVel = Vector3.Normalize(motor.MovementDirection) * motor.MaxSpeed;

            planarVel = MathUtil.ExpDecay(planarVel, targetVel, 12, 1f / 60f);

            actor.Velocity = planarVel + verticalVel;
        });
    }
}
