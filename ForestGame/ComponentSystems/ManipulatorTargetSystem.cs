using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;

namespace ForestGame.ComponentSystems;

public class ManipulatorTargetSystem : ISystem
{
    private EntityReference _manipulatorHolder = EntityReference.Null;

    public void Update()
    {
        if(!_manipulatorHolder.IsAlive())
        {
            EcsManager.world.Query(new QueryDescription().WithAll<ManipulatorData, PlayerControlled>(),
                (Entity entity) => {
                    if(_manipulatorHolder.IsAlive())
                        return;

                    _manipulatorHolder = entity.Reference();
                }
            );
            if(!_manipulatorHolder.IsAlive())
                return;
        }

        float closest = float.MaxValue;
        var reference = EntityReference.Null;
        Ray ray = new(RenderPipeline.Camera.Transform.WorldPosition, RenderPipeline.Camera.Transform.Matrix.Forward);

        EcsManager.world.Query(new QueryDescription().WithAll<Collider, Transform>().WithNone<PlayerControlled>(),
            (Entity entity, ref Collider collider, ref Transform transform) => {
                if(ray.Intersects(collider.BoundingBox(transform.Scale)) is float hit)
                {
                    if(hit >= closest)
                        return;
                    closest = hit;
                    reference = entity.Reference();
                }
            }
        );

        ref ManipulatorData data = ref _manipulatorHolder.Entity.Get<ManipulatorData>();

        ManipulatorRenderSystem renderSystem = (ManipulatorRenderSystem)Registry<ISystem>.Get(Registries.ComponentSystems.ManipulatorRender)!;

        if(reference != data.TargetEntity)
            renderSystem.HighlightFadeAmount = 0.1f;
        data.TargetEntity = reference;
    }
}
