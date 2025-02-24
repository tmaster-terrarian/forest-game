using Microsoft.Xna.Framework;

namespace ForestGame.Core.Graphics;

public class Camera
{
    public Transform Transform;

    public Vector3 Forward => Vector3.Transform(Vector3.Forward, Transform.Rotation);

    public Vector3 Up => Vector3.Transform(Vector3.Up, Transform.Rotation);

    public Camera()
    {
        // Transform.Position = new(0, 1, 2);
    }

    public void Update(GameTime gameTime)
    {
        var time = (float)gameTime.TotalGameTime.TotalSeconds * 60;

        Transform.Position = new Vector3(
            2.5f * MathF.Cos(time * 0.01f),
            2.5f * MathF.Sin(time * 0.01f),
            2.5f * MathF.Sin(time * 0.01f)
        );

        float xz = Vector2.Distance(new Vector2(Transform.Position.X, Transform.Position.Z), new Vector2(0, 0));
        float y = Transform.Position.Y - 0;

        float yaw = MathF.Atan2(y, xz);
        float pitch = MathF.Atan2(Transform.Position.X - 0, Transform.Position.Z - 0);

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(
            MathHelper.WrapAngle(pitch),
            -MathHelper.WrapAngle(yaw),
            0
        );
    }
}
