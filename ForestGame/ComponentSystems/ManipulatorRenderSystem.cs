using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.ComponentSystems;

public class ManipulatorRenderSystem : ISystem, IDrawableSystem
{
    private const float Range = 16;
    public bool Enabled { get; set; } = false;

    public float HighlightFadeAmount { get; set; } = 0;

    private float _fadeAmount = 0;
    private bool scanning;
    private float scanProgress;

    private EntityReference _manipulatorHolder = EntityReference.Null;

    public void DoScan()
    {
        scanning = true;
        scanProgress = 0;
    }

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
        }

        if(_manipulatorHolder.IsAlive() && Input.GetPressed(Keys.O))
            DoScan();

        if(!Enabled)
            _fadeAmount = MathUtil.ExpDecay(_fadeAmount, 0, 6, Time.Delta);
        else
            _fadeAmount = MathUtil.ExpDecay(_fadeAmount, 1, 10, Time.Delta);

        if(scanning)
            scanProgress = MathUtil.ExpDecay(scanProgress, 1, 1.5f, Time.Delta);

        if(MathUtil.Approximately(scanProgress, 1, 0.01f))
        {
            scanning = false;
            scanProgress = 0;
        }

        HighlightFadeAmount = MathUtil.ExpDecay(HighlightFadeAmount, 1, 10, Time.Delta);
    }

    public void GetDrawables(GraphicsDevice graphicsDevice)
    {
        var aspect = GetAspect();
        if(aspect is null)
            return;

        EcsManager.world.Query(new QueryDescription().WithAll<Collider, Transform>().WithNone<ManipulatorData>(),
            (Entity entity, ref Collider collider, ref Transform transform) => {
                float opacity = _fadeAmount;
                if(scanning)
                {
                    var scanDistance = scanProgress * Range;
                    var objDistance = (transform.WorldPosition - RenderPipeline.Camera.Transform.WorldPosition).Length();
                    var scanInfluence = 2f - MathHelper.Clamp(objDistance - scanDistance, 0, 2);
                    scanInfluence += MathHelper.Min(0, objDistance - scanDistance) * 0.1f;
                    scanInfluence -= MathHelper.Max(0, objDistance - scanDistance) * 0.25f;
                    if(Math.Sign(objDistance - scanDistance) == -1)
                        scanInfluence = MathHelper.Max(0.5f, scanInfluence);
                    scanInfluence *= 1 - MathHelper.Max(0, scanDistance - (Range - 1));
                    opacity = scanInfluence;
                }

                EntityReference target = EntityReference.Null;

                if(_manipulatorHolder.IsAlive())
                    target = _manipulatorHolder.Entity.Get<ManipulatorData>().TargetEntity;

                aspect.Submit(
                    transform,
                    collider,
                    entity == target ? HighlightFadeAmount + MathHelper.Max(0, opacity) : 0,
                    opacity,
                    scanning || entity == target,
                    entity.Has<Solid>()
                );
            }
        );
    }

    private static BoundingBoxAspect? GetAspect()
        => Registry<Aspect>.Get(Registries.Aspects.BoundingBox) as BoundingBoxAspect;
}
