using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Renderers;

public class PatherRenderer : Renderer
{
    public override void Draw(GraphicsDevice graphicsDevice, EffectConfig effectConfig)
    {
        effectConfig.VertexColorIntensity?.SetValue(1);
        effectConfig.LightIntensity?.SetValue(0);

        foreach(var effectPass in effectConfig.Effect.CurrentTechnique.Passes)
        {
            effectPass.Apply();

            EcsManager.world.Query(
                new QueryDescription().WithAll<Pather, Motor, Transform>(),
                (ref Pather pather, ref Motor motor, ref Transform transform) =>
                {
                    var difference = pather.TargetPosition - transform.WorldPosition;
                    var direction = Vector3.Normalize(difference);
                    if (!pather.IsPathing)
                        return;

                    GraphicsUtil.DrawVector(
                        graphicsDevice,
                        transform.WorldPosition,
                        direction,
                        Color.Blue,
                        difference.Length()
                    );
                }
            );
        }
    }
}
