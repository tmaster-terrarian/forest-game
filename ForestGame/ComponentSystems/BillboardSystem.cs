using Arch.Core;
using ForestGame.Components;
using ForestGame.Core;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Vector3 = System.Numerics.Vector3;

namespace ForestGame.ComponentSystems;

public class BillboardSystem : ISystem
{
    public void Update()
    {
        var camTransform = RenderPipeline.Camera.Transform;
        var camPos = RenderPipeline.Camera.Transform.WorldPosition;
        var camForward = RenderPipeline.Camera.Transform.Matrix.Forward;
        EcsManager.world.Query(new QueryDescription().WithAll<Billboard, Transform>(),
            (ref Transform t) =>
            {
                var pos = t.WorldPosition;
                var lookatMatrix =
                    Matrix.CreateConstrainedBillboard(-pos, -camPos, Vector3.UnitY, null, null);
                t.Rotation = Quaternion.CreateFromRotationMatrix(lookatMatrix);
            });
    }
}
