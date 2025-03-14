using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.ComponentSystems;

public class PlayerSystem : ISystem
{
    private Point _lastMousePos;

    private float _sensitivity = 0.5f;

    public void Update()
    {
        var camera = RenderPipeline.Camera;
        bool primaryPlayer = true;

        EcsManager.world.Query(new QueryDescription().WithAll<PlayerControlled, Motor>(),
            (Entity entity, ref PlayerControlled controller, ref Motor motor) =>
            {
                Vector3 inputDir = Vector3.Zero;

                if(Global.GameWindowFocused)
                {
                    if(primaryPlayer)
                    {
                        camera.Target = entity.Reference();
                        if(!Input.InputDisabled && Input.MousePosition != _lastMousePos)
                        {
                            var difference = Input.MousePosition - _lastMousePos;
                            motor.Yaw -= difference.X * (1f/144f) * _sensitivity;
                            motor.Pitch -= difference.Y * (1f/144f) * _sensitivity;
                            camera.Yaw = motor.Yaw;
                            camera.Pitch = motor.Pitch;
                            camera.Pitch = MathHelper.Clamp(camera.Pitch, -MathHelper.ToRadians(89.99f), MathHelper.ToRadians(89.99f));
                            camera.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(camera.Yaw, camera.Pitch, 0);
                            _lastMousePos = new(RenderPipeline.Window.ClientBounds.Width / 2, RenderPipeline.Window.ClientBounds.Height / 2);
                            Mouse.SetPosition(_lastMousePos.X, _lastMousePos.Y);
                        }
                    }

                    inputDir = new(
                        (Input.GetDown(Keys.D) ? 1 : 0) - (Input.GetDown(Keys.A) ? 1 : 0),
                        0,
                        (Input.GetDown(Keys.S) ? 1 : 0) - (Input.GetDown(Keys.W) ? 1 : 0)
                    );
                }

                // if(inputDir != Vector3.Zero)
                // {
                //     inputDir = Vector3.Normalize(inputDir);

                //     _localVelocity.X = MathUtil.Approach(_localVelocity.X, inputDir.X * _speed, 0.1f * Time.Delta);
                //     _localVelocity.Z = MathUtil.Approach(_localVelocity.Z, inputDir.Y * _speed, 0.1f * Time.Delta);
                // }
                // else
                // {
                //     _localVelocity.X = MathUtil.Approach(_localVelocity.X, 0, 0.08f * Time.Delta);
                //     _localVelocity.Z = MathUtil.Approach(_localVelocity.Z, 0, 0.08f * Time.Delta);
                // }

                motor.MovementDirection = Vector3.Transform(inputDir, Quaternion.CreateFromYawPitchRoll(camera.Yaw, 0, 0));

                primaryPlayer = false;
            }
        );
    }
}
