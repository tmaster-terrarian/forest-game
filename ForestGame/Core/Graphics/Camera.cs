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

    private Transform _freeCamTransform = Transform.Identity;
    private Vector3 _freeCamVelocity;

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

        if(Global.Editor)
            FreeCam();

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

    private void FreeCam()
    {
        if(Global.EditorOpened)
        {
            if(Target.IsAlive() && Target.Entity.TryGet<Transform>(out var transform))
                _freeCamTransform.Position = transform.WorldPosition;
        }

        Target = EntityReference.Null;

        Vector3 targetVel = Vector3.Zero;

        Vector3 planar = new(
            (Input.GetDown(Keys.D) ? 1 : 0) - (Input.GetDown(Keys.A) ? 1 : 0),
            0,
            (Input.GetDown(Keys.S) ? 1 : 0) - (Input.GetDown(Keys.W) ? 1 : 0)
        );

        if(planar != Vector3.Zero)
        {
            planar.Normalize();
            planar = Vector3.Transform(planar, Quaternion.CreateFromYawPitchRoll(Yaw, 0, 0));
            targetVel = planar;
        }

        Vector3 vertical = new(0, (Input.GetDown(Keys.Space) ? 1 : 0) - (Input.GetDown(Keys.LeftShift) ? 1 : 0), 0);
        targetVel += vertical;

        _freeCamVelocity = MathUtil.Approach(
            _freeCamVelocity,
            !Global.LockMouse ? Vector3.Zero : targetVel,
            3 * Time.UnscaledDelta
        );

        _freeCamTransform.Position += _freeCamVelocity * 4 * Time.UnscaledDelta;

        Transform.Parent = _freeCamTransform;
    }
}
