using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class ActorSystem : ISystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor, Transform>(), (Entity entity, ref Actor actor, ref Transform transform) =>
        {
            if (actor.HasGravity)
            {
                actor.Velocity += new Vector3(0, -9.81f * 2f, 0) * Time.Delta;
            }

            if (!entity.Has<Motor>())
            {
                var planarVel = MathUtil.ProjectOnPlane(actor.Velocity, Vector3.UnitY);
                var verticalVel = MathUtil.Project(actor.Velocity, Vector3.UnitY);

                planarVel = MathUtil.ExpDecay(planarVel, Vector3.Zero, 8, Time.Delta);
                actor.Velocity = planarVel + verticalVel;
            }

            transform.Position += actor.Velocity * Time.Delta;

            foreach (var collision in actor.Collisions)
            {
                if (collision.Normal == Vector3.UnitY)
                {
                    if (entity.TryGet<Bouncy>(out var bouncy))
                    {
                        if (bouncy.OriginalBounceVelocity is null)
                        {
                            actor.Velocity = actor.Velocity with { Y = MathF.Abs(actor.Velocity.Y) };
                            bouncy.OriginalBounceVelocity = actor.Velocity.Y;
                        }
                        else
                        {
                            actor.Velocity = actor.Velocity with { Y = bouncy.OriginalBounceVelocity.Value };
                        }
                        entity.Set(bouncy);
                    }
                    else
                    {
                        actor.Velocity = MathUtil.ProjectOnPlane(actor.Velocity, collision.Normal);
                    }
                }
                else
                {
                    actor.Velocity = MathUtil.ProjectOnPlane(actor.Velocity, collision.Normal);
                }
            }

            actor.Collisions = [];

            float checkBottom = transform.Position.Y;
            if (entity.TryGet(out Collider collider))
                checkBottom = collider.BoundingBox(transform.Scale).Min.Y;
            float diffBottom = transform.Position.Y - checkBottom;
            if (checkBottom <= 0 && actor.Velocity.Y <= 0)
            {
                transform.Position = transform.Position with { Y = diffBottom };
                actor.Collisions = [..actor.Collisions, new CollisionInfo(Vector3.UnitY)];
            }
        });
    }
}
