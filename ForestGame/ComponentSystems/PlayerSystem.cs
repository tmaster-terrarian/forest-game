using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.ComponentSystems;

public class PlayerSystem : ISystem, ISystem.EditorUpdate
{
    private const float Sensitivity = 0.5f;

    public void Update()
    {
        var camera = RenderPipeline.Camera;
        bool primaryPlayer = true;

        EcsManager.world.Query(new QueryDescription().WithAll<PlayerControlled, Motor, Actor>(),
            (Entity entity, ref PlayerControlled controller, ref Motor motor, ref Actor actor) =>
            {
                Vector3 inputDir = Vector3.Zero;
                Point lastMousePos = controller.LastMousePos;
                Point currentMousePos = Input.MousePosition;

                if(Global.GameWindowFocused)
                {
                    if(primaryPlayer)
                    {
                        camera.Target = entity.Reference();
                        if(Global.LockMouse && ((currentMousePos != lastMousePos && !Input.InputDisabled) || Internals._focusClickedChanged))
                        {
                            var difference = Internals._focusClickedChanged ? Point.Zero : currentMousePos - lastMousePos;
                            camera.Yaw -= difference.X * (1f/144f) * Sensitivity;
                            camera.Pitch -= difference.Y * (1f/144f) * Sensitivity;
                            camera.Pitch = MathHelper.Clamp(camera.Pitch, -MathHelper.ToRadians(89.99f), MathHelper.ToRadians(89f));
                            camera.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(camera.Yaw, camera.Pitch, 0);
                            currentMousePos = new(RenderPipeline.Window.ClientBounds.Width / 2, RenderPipeline.Window.ClientBounds.Height / 2);
                            Mouse.SetPosition(currentMousePos.X, currentMousePos.Y);

                            if(!Global.Editor)
                            {
                                motor.Yaw = camera.Yaw;
                                motor.Pitch = camera.Pitch;
                            }
                        }
                    }

                    if(Global.PlayerCanMove)
                    {
                        inputDir = new(
                            (Input.GetDown(Keys.D) ? 1 : 0) - (Input.GetDown(Keys.A) ? 1 : 0),
                            0,
                            (Input.GetDown(Keys.S) ? 1 : 0) - (Input.GetDown(Keys.W) ? 1 : 0)
                        );
                    }
                }

                if(Input.InputDisabled && !Internals._focusClickedChanged)
                {
                    currentMousePos = new(RenderPipeline.Window.ClientBounds.Width / 2, RenderPipeline.Window.ClientBounds.Height / 2);
                }

                controller.LastMousePos = currentMousePos;

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

                if(!Global.Editor)
                {
                    motor.MovementDirection = Vector3.Transform(inputDir, Quaternion.CreateFromYawPitchRoll(camera.Yaw, 0, 0));
                    bool hasBouncy = entity.Has<Bouncy>();
                    if (!actor.IsGrounded && !hasBouncy)
                        motor.MovementDirection = Vector3.Zero;
                }

                primaryPlayer = false;
            }
        );
    }
}
