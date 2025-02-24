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

    private Point _lastMousePos;
    private float _pitch = 0;
    private float _yaw = 0;
    private float _sensitivity = 0.5f;
    private float _speed = 0.02f;
    private Vector3 _localVelocity;

    private float[][] floats = [
        [0.1f, 0.3f, 0.7f, 0.7f, 0.3f],
        [0.3f, 0.7f, 0.9f, 0.9f, 0.7f],
        [0.7f, 0.9f, 1.0f, 1.0f, 0.9f],
        [0.3f, 0.7f, 0.9f, 1.0f, 0.9f],
        [0.1f, 0.3f, 0.7f, 0.9f, 0.7f]
    ];

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

        int tz = MathHelper.Clamp(MathUtil.FloorToInt(Transform.Position.Z - 0.5f), 0, 4);
        int tx = MathHelper.Clamp(MathUtil.FloorToInt(Transform.Position.X - 0.5f), 0, 4);
        int ntz = MathHelper.Clamp(MathUtil.FloorToInt(Transform.Position.Z - 0.5f + 1), 0, 4);
        int ntx = MathHelper.Clamp(MathUtil.FloorToInt(Transform.Position.X - 0.5f + 1), 0, 4);
        float fz = MathUtil.SmoothCos((Transform.Position.Z - 0.5f) % 1f, 0.9f);
        float fx = MathUtil.SmoothCos((Transform.Position.X - 0.5f) % 1f, 0.9f);

        float hz1 = MathHelper.Lerp(floats[tz][tx], floats[ntz][tx], fz);
        float hz2 = MathHelper.Lerp(floats[tz][ntx], floats[ntz][ntx], fz);
        float height = MathHelper.Lerp(hz1, hz2, fx);

        // float height = floats[tz][tx] + floats[ntz][tx] * fz + floats[tz][ntx] * fx + floats[ntz][ntx] * fz * fx;

        if(Input.MousePosition != _lastMousePos)
        {
            var difference = Input.MousePosition - _lastMousePos;
            _yaw -= difference.X * (1f/144f) * _sensitivity;
            _pitch -= difference.Y * (1f/144f) * _sensitivity;
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

            _localVelocity.X = MathUtil.Approach(_localVelocity.X, inputDir.X * _speed, 0.1f * gameTime.Delta());
            _localVelocity.Z = MathUtil.Approach(_localVelocity.Z, inputDir.Y * _speed, 0.1f * gameTime.Delta());
        }
        else
        {
            _localVelocity.X = MathUtil.Approach(_localVelocity.X, 0, 0.08f * gameTime.Delta());
            _localVelocity.Z = MathUtil.Approach(_localVelocity.Z, 0, 0.08f * gameTime.Delta());
        }

        Transform.Position += Vector3.Transform(_localVelocity * 60 * gameTime.Delta(), Quaternion.CreateFromYawPitchRoll(_yaw, 0, 0));

        Transform.Position = Transform.Position with { Y = 1.3f + height };
    }

    public void Draw(GraphicsDevice graphicsDevice)
    {
        for(int z = 0; z < 5; z++)
        {
            for(int x = 0; x < 5; x++)
            {
                GraphicsUtil.DrawQuad(
                    graphicsDevice,
                    RenderPipeline.WhiteTexture,
                    Color.Lerp(Color.Gray, Color.White, floats[z][x]),
                    Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(new(x + 0.5f, floats[z][x], z + 0.5f)),
                    1, 1,
                    Vector2.Zero, Vector2.One
                );
            }
        }
    }
}
