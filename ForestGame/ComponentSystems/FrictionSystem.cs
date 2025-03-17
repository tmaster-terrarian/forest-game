using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class FrictionSystem : ISystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor>(), (ref Actor actor) =>
        {
            // Air Friction
            actor.Velocity = MathUtil.ExpDecay(actor.Velocity, Vector3.Zero, 0.5f, Time.Delta);

            // Ground Checking
            actor.IsGrounded = false;
            if (actor.Collisions.Any((info) => Vector3.Dot(info.Normal, Vector3.UnitY) > 0.9f))
                actor.IsGrounded = true;

            // Ground Friction
            if (actor.IsGrounded)
            {
                Vector3 planarVel = MathUtil.ProjectOnPlane(actor.Velocity, Vector3.UnitY);
                Vector3 verticalVel = MathUtil.Project(actor.Velocity, Vector3.UnitY);
                planarVel = MathUtil.ExpDecay(planarVel, Vector3.Zero, 6f, Time.Delta);
                actor.Velocity = planarVel + verticalVel;
            }
        });
    }
}
