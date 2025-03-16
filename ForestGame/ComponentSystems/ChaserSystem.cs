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
        EcsManager.world.Query(
            new QueryDescription()
                .WithAll<Chaser, Transform>()
                .WithAny<Motor>(),
            (Entity entity, ref Chaser chaser, ref Transform transform) =>
        {
            if (!chaser.Target.IsAlive()) return;
            if (!chaser.Target.Entity.TryGet(out Transform targetTransform)) return;

            Vector3 pos = transform.WorldPosition;
            Vector3 targetPos = targetTransform.WorldPosition;
            if (entity.TryGet(out Motor motor))
            {
                UpdateMotor(entity, motor, pos, targetPos);
            }
        });
    }

    private void UpdateMotor(Entity entity, Motor motor, Vector3 position, Vector3 targetPosition)
    {
        Vector3 direction = MathUtil.ProjectOnPlane(Vector3.Normalize(targetPosition - position), Vector3.UnitY);
        motor.MovementDirection = direction;
        entity.Set(motor);
    }
}
