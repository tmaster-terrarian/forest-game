using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class RandomPatherSystem : ISystem
{
    public void Update()
    {
        EcsManager.world.Query(
            new QueryDescription()
                .WithAll<RandomPather, Pather, Transform>(),
            (ref RandomPather randomMovement, ref Pather pather, ref Transform transform) =>
            {
                bool patherUpdateIntervalPassed = pather.LastPathTime < Time.Elapsed - pather.PathUpdateInterval;
                bool patherAtDestination = Vector3.Distance(transform.WorldPosition, pather.TargetPosition) < 1f;
                if (!patherAtDestination && !patherUpdateIntervalPassed)
                {
                    return;
                }

                if (patherAtDestination)
                {
                    pather.IsPathing = false;
                }

                if (patherUpdateIntervalPassed || pather.ForcePathUpdateOnDestinationReached)
                {
                    pather.TargetPosition = transform.WorldPosition + MathUtil.RandomInsideUnitSphere() * 5;
                    pather.LastPathTime = Time.Elapsed;
                    pather.IsPathing = true;
                }
            });
    }
}
