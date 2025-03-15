using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class WorldLoopingSystem : ISystem
{
    public void Update()
    {
        Vector3 worldSize = Vector3.One * float.MaxValue;
        EcsManager.world.Query(new QueryDescription().WithAll<WorldLooper>(), (Entity entity, ref WorldLooper worldLooperGet) =>
        {
            worldSize = worldLooperGet.WorldSize;

            Vector3 playerPos = Vector3.Zero;
            EcsManager.world.Query(new QueryDescription().WithAll<PlayerControlled, Transform>(),
                (ref PlayerControlled playerControlled, ref Transform transform) =>
                {
                    playerPos = transform.Position;
                    playerPos = MathUtil.Mod(playerPos, worldSize) with { Y = playerPos.Y };
                    transform.Position = playerPos;
                });

            EcsManager.world.Query(new QueryDescription().WithAll<Transform>().WithAny<Actor, Solid>().WithNone<PlayerControlled>(), (ref Transform transform) =>
            {
                Vector3 pos = transform.Position;
                Vector3 diff = pos - playerPos;
                if (MathF.Abs(diff.X) > worldSize.X / 2f)
                {
                    float direction = -MathF.Sign(diff.X);
                    transform.Position += Vector3.UnitX * (worldSize.X * direction);
                }
                if (MathF.Abs(diff.Z) > worldSize.Z / 2f)
                {
                    float direction = -MathF.Sign(diff.Z);
                    transform.Position += Vector3.UnitZ * (worldSize.Z * direction);
                }
            });
        });
    }
}
