using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.Core.Graphics;

public class Camera
{
    public Transform Transform;

    public Vector3 Forward => Vector3.Transform(Vector3.Forward, Transform.Rotation);

    public Vector3 Up => Vector3.Transform(Vector3.Up, Transform.Rotation);

    public Vector3 Right => Vector3.Transform(Vector3.Right, Transform.Rotation);

    public EntityReference Target;

    public float Yaw { get; set; }
    public float Pitch { get; set; }

    private float[][] floats = [
        [0.0f, 0.3f, 0.7f, 0.7f, 0.3f],
        [0.3f, 0.7f, 0.9f, 0.9f, 0.7f],
        [0.7f, 0.9f, 1.0f, 1.0f, 0.9f],
        [0.3f, 0.7f, 0.9f, 1.0f, 0.9f],
        [0.0f, 0.3f, 0.7f, 0.9f, 0.7f]
    ];

    public void Update()
    {
        // var time = Time.Elapsed * 60;
        //
        // Transform.Position = new Vector3(
        //     2.5f * MathF.Cos(time * 0.01f),
        //     2.5f * MathF.Sin(time * 0.01f),
        //     2.5f * MathF.Sin(time * 0.01f)
        // );
        //
        // float xz = Vector2.Distance(new Vector2(Transform.Position.X, Transform.Position.Z), new Vector2(0, 0));
        // float y = Transform.Position.Y - 0;
        //
        // float yaw = MathF.Atan2(Transform.Position.X - 0, Transform.Position.Z - 0);
        // float pitch = MathF.Atan2(y, xz);
        //
        // Transform.Rotation = Quaternion.CreateFromYawPitchRoll(
        //     MathHelper.WrapAngle(yaw),
        //     -MathHelper.WrapAngle(pitch),
        //     0
        // );

        if(!Target.IsAlive())
            return;

        if(Target.Entity.TryGet<Transform>(out var transform))
            Transform.Parent = transform;

        // int tz = MathHelper.Clamp(MathUtil.FloorToInt(Transform.Position.Z - 0.5f), 0, 4);
        // int tx = MathHelper.Clamp(MathUtil.FloorToInt(Transform.Position.X - 0.5f), 0, 4);
        // int ntz = MathHelper.Clamp(MathUtil.FloorToInt(Transform.Position.Z - 0.5f + 1), 0, 4);
        // int ntx = MathHelper.Clamp(MathUtil.FloorToInt(Transform.Position.X - 0.5f + 1), 0, 4);
        // float fz = MathUtil.SmoothCos((Transform.Position.Z - 0.5f) % 1f, 0.9f);
        // float fx = MathUtil.SmoothCos((Transform.Position.X - 0.5f) % 1f, 0.9f);

        // float hz1 = MathHelper.Lerp(floats[tz][tx], floats[ntz][tx], fz);
        // float hz2 = MathHelper.Lerp(floats[tz][ntx], floats[ntz][ntx], fz);
        // float height = MathHelper.Lerp(hz1, hz2, fx);

        // Transform.Position = Transform.Position with { Y = 1.3f + height };

        Transform.Position = Transform.Position with { Y = 1.3f };
    }

    public void Draw(GraphicsDevice graphicsDevice)
    {
        // for(int z = 0; z < 5; z++)
        // {
        //     for(int x = 0; x < 5; x++)
        //     {
        //         GraphicsUtil.DrawQuad(
        //             graphicsDevice,
        //             RenderPipeline.WhiteTexture,
        //             Color.Lerp(Color.Gray, Color.White, floats[z][x]),
        //             Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(new(x + 0.5f, floats[z][x], z + 0.5f)),
        //             RenderPipeline.EffectLit,
        //             1, 1,
        //             Vector2.Zero, Vector2.One
        //         );
        //     }
        // }

        // VertexPositionColorNormalTexture[] point = [
        //     new(Vector3.Zero, Color.Red, Vector3.UnitY, Vector2.Zero),
        // ];
        // graphicsDevice.DrawUserPrimitives(
        //     PrimitiveType.PointList, point, 0, 1,
        //     VertexPositionColorNormalTexture.VertexDeclaration
        // );
    }
}
