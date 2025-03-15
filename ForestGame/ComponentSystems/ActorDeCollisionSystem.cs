using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class ActorDeCollisionSystem : ISystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor, Collider, Transform>(), (Entity entity1, ref Actor actor1, ref Collider collider1, ref Transform transform) =>
        {
            var actor = actor1;
            var bb1 = collider1.BoundingBox(transform.Scale);
            var median1 = bb1.Median();
            EcsManager.world.Query(new QueryDescription().WithAll<Actor, Collider, Transform>(), (Entity entity2, ref Actor actor2, ref Collider collider2, ref Transform transform2) =>
            {
                if (entity1 == entity2) return;
                var bb2 = collider2.BoundingBox(transform2.Scale);
                if (!bb1.Intersects(bb2)) return;

                var median2 = bb2.Median();
                var direction = median1 - median2;
                var amount = collider2.Size / direction.Length();
                direction = Vector3.Normalize(MathUtil.ProjectOnPlane(direction, Vector3.UnitY));
                Vector3 vel = MathUtil.ProjectOnPlane(actor.Velocity, Vector3.UnitY);
                vel = MathUtil.ExpDecay(vel, direction * amount * 2f, 12, Time.Delta);
                actor.Velocity = vel + MathUtil.Project(actor.Velocity, Vector3.UnitY);
            });
            entity1.Set(actor);
        });
    }
}
