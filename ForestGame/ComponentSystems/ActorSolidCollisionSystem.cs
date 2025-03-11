using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

/// <summary>
/// Made with the help of <see href="https://spader.zone/minkowski/">"2D Collisions with Minkowski Differences"</see>
/// </summary>
public class ActorSolidCollisionSystem : IComponentSystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Actor, Transform>(),
            (Entity entity, ref Actor actor, ref Collider collider, ref Transform transform) =>
        {
            var actorMutable = actor;
            var bb = collider.BoundingBox(transform.Scale);
            var transformMutable = transform;
            EcsManager.world.Query(new QueryDescription().WithAll<Solid, Collider>(), (Entity solidEntity, ref Solid solid, ref Collider solidCollider) =>
            {
                var solidBb = solidCollider.BoundingBox(Vector3.One);
                if (solidEntity.TryGet<Transform>(out var solidTransform))
                {
                    solidBb = solidCollider.BoundingBox(solidTransform.Scale);
                }

                if (!bb.Intersects(solidBb)) return;

                var minkowskiDiff = bb.MinkowskiDifference(solidBb);
                if (minkowskiDiff.Contains(Vector3.Zero) == ContainmentType.Disjoint) return;

                Vector3 penetration = Vector3.Zero;
                float min = float.MaxValue;
                if (MathF.Abs(minkowskiDiff.Min.X) < min) {
                    min = MathF.Abs(minkowskiDiff.Min.X);
                    penetration = new Vector3(minkowskiDiff.Min.X, 0f, 0f);
                }
                if (MathF.Abs(minkowskiDiff.Max.X) < min) {
                    min = MathF.Abs(minkowskiDiff.Max.X);
                    penetration = new Vector3(minkowskiDiff.Max.X, 0f, 0f);
                }
                if (MathF.Abs(minkowskiDiff.Min.Y) < min) {
                    min = MathF.Abs(minkowskiDiff.Min.Y);
                    penetration = new Vector3(0f, minkowskiDiff.Min.Y, 0f);
                }
                if (MathF.Abs(minkowskiDiff.Max.Y) < min) {
                    min = MathF.Abs(minkowskiDiff.Max.Y);
                    penetration = new Vector3(0f, minkowskiDiff.Max.Y, 0f);
                }
                if (MathF.Abs(minkowskiDiff.Min.Z) < min) {
                    min = MathF.Abs(minkowskiDiff.Min.Z);
                    penetration = new Vector3(0f, 0f, minkowskiDiff.Min.Z);
                }
                if (MathF.Abs(minkowskiDiff.Max.Z) < min) {
                    min = MathF.Abs(minkowskiDiff.Max.Z);
                    penetration = new Vector3(0f, 0f, minkowskiDiff.Max.Z);
                }

                if (actorMutable.Velocity == Vector3.Zero || penetration == Vector3.Zero) return;
                transformMutable.Position -= penetration;
                Vector3 normal = Vector3.Normalize(-penetration);
                actorMutable.Collisions = [..actorMutable.Collisions, new CollisionInfo(normal)];
            });

            entity.Set(actorMutable);
            entity.Set(transformMutable);
        });
    }
}
