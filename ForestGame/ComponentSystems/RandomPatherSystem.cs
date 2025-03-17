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
                .WithAll<RandomPather, Pather, Transform, Actor>(),
            (ref RandomPather randomPather, ref Pather pather, ref Transform transform, ref Actor actor) =>
            {
                if (pather.LastPathTime == 0)
                    SetFirstRandomTime(ref pather);
                bool patherUpdateIntervalPassed = pather.LastPathTime < Time.Elapsed - pather.PathUpdateInterval;
                bool patherAtDestination =
                    MathUtil.WillOvershoot(transform.WorldPosition, pather.TargetPosition, actor.Velocity);
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
                    pather.TargetPosition = transform.WorldPosition +
                                            MathUtil.RandomVector3(-randomPather.Range, randomPather.Range);
                    SetNewPatherTime(ref pather);
                    pather.IsPathing = true;
                }
            });
    }

    private void SetFirstRandomTime(ref Pather pather)
    {
        pather.LastPathTime = Time.Elapsed - Random.Shared.NextSingle() * pather.PathUpdateInterval;
    }

    private void SetNewPatherTime(ref Pather pather)
    {
        pather.LastPathTime =
            Time.Elapsed - Random.Shared.NextSingle() * pather.PathUpdateIntervalRandomRange;
    }
}
