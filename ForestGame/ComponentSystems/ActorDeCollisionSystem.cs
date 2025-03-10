using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class ActorDeCollisionSystem : IComponentSystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor, Transform>(), (Entity entity1, ref Actor actor1, ref Transform transform) =>
        {
            var actor = actor1;
            var bb1 = actor.Collider.BoundingBox(transform.Scale);
            var median1 = bb1.Median();
            EcsManager.world.Query(new QueryDescription().WithAll<Actor, Transform>(), (Entity entity2, ref Actor actor2, ref Transform transform2) =>
            {
                if (entity1 == entity2) return;
                var bb2 = actor2.Collider.BoundingBox(transform2.Scale);
                if (!bb1.Intersects(bb2)) return;

                var median2 = bb2.Median();
                var direction = median1 - median2;
                var amount = actor2.Collider.Size / 2f / direction.Length();
                direction = Vector3.Normalize(MathUtil.ProjectOnPlane(direction, Vector3.UnitY));
                actor.Velocity = MathUtil.ExpDecay(actor.Velocity, direction * amount * 2f, 12, Time.Delta);
            });
            entity1.Set(actor);
        });
    }
}
