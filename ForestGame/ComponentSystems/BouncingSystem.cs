using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;

namespace ForestGame.ComponentSystems;

public class BouncingSystem : IComponentSystem
{
    public void Update()
    {
        EcsManager.world.Query(new QueryDescription().WithAll<Transform, Bouncy>(),
            (ref Transform transform, ref Bouncy bouncy) =>
            {
                transform.Position = transform.Position with
                {
                    Y = MathF.Abs(MathF.Sin((Time.Elapsed + bouncy.RandomSeed * 120) * bouncy.BounceSpeed)) * 5f
                };
            });
    }
}
