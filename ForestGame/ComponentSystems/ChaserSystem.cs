using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class ChaserSystem : ISystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Chaser, Transform>(), (Entity entity, ref Chaser chaser, ref Transform transform) =>
        {
            if (!chaser.Target.IsAlive()) return;
            if (!chaser.Target.Entity.TryGet(out Transform targetTransform)) return;
            if (entity.TryGet(out Motor motor))
            {
                motor.MovementDirection = Vector3.Normalize(targetTransform.Position - transform.Position);
                entity.Set(motor);
            }
        });
    }
}
