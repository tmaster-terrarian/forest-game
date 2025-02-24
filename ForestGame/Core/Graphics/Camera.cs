using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.Core.Graphics;

public class Camera
{
    public Transform Transform;

    public Vector3 Forward => Vector3.Transform(Vector3.Forward, Transform.Rotation);

    public Vector3 Up => Vector3.Transform(Vector3.Up, Transform.Rotation);

    public Vector3 Right => Vector3.Transform(Vector3.Right, Transform.Rotation);

    private Point _lastMousePos;
    private float _pitch = 0;
    private float _yaw = 0;
    private float _sensitivity = 0.5f;
    private float _speed = 2f;
    private Vector3 _localVelocity;

    public Camera()
    {
        // Transform.Position = new(0, 0, 0);
        _lastMousePos = Input.MousePosition;
    }

    public void Update(GameTime gameTime)
    {
        var time = (float)gameTime.TotalGameTime.TotalSeconds * 60;

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

        if(Input.MousePosition != _lastMousePos)
        {
            var difference = Input.MousePosition - _lastMousePos;
            _yaw -= difference.X * gameTime.Delta() * _sensitivity;
            _pitch -= difference.Y * gameTime.Delta() * _sensitivity;
            _pitch = MathHelper.Clamp(_pitch, -MathHelper.ToRadians(89.99f), MathHelper.ToRadians(89.99f));
            Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, 0);
            _lastMousePos = new(RenderPipeline.Window.ClientBounds.Width / 2, RenderPipeline.Window.ClientBounds.Height / 2);
            Mouse.SetPosition(_lastMousePos.X, _lastMousePos.Y);
        }

        Vector2 inputDir = new(
            (Input.GetDown(Keys.D) ? 1 : 0) - (Input.GetDown(Keys.A) ? 1 : 0),
            (Input.GetDown(Keys.S) ? 1 : 0) - (Input.GetDown(Keys.W) ? 1 : 0)
        );
        if(inputDir != Vector2.Zero)
        {
            inputDir = Vector2.Normalize(inputDir);

            _localVelocity.X = MathUtil.Approach(_localVelocity.X, inputDir.X * gameTime.Delta() * _speed, 0.08f * gameTime.Delta());
            _localVelocity.Z = MathUtil.Approach(_localVelocity.Z, inputDir.Y * gameTime.Delta() * _speed, 0.08f * gameTime.Delta());
        }
        else
        {
            _localVelocity.X = MathUtil.Approach(_localVelocity.X, 0, 0.05f * gameTime.Delta());
            _localVelocity.Z = MathUtil.Approach(_localVelocity.Z, 0, 0.05f * gameTime.Delta());
        }

        Transform.Position += Vector3.Transform(_localVelocity * 60 * gameTime.Delta(), Quaternion.CreateFromYawPitchRoll(_yaw, 0, 0));
    }
}
