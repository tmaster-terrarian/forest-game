using Arch.Core.Extensions;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.Core;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "shaders";

        IsMouseVisible = false;

        IsFixedTimeStep = false;
    }

    protected override void Initialize()
    {
        Internals.Initialize(this);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        Internals.LoadContent(this);

        for(int i = 0; i < 30; i++)
        {
            var entity = Registry<Prototype>.Get(Registries.Prototypes.Teapot).Construct().Entity;
            var randomPos = MathUtil.RandomInsideUnitSphere() * 20;
            randomPos.Y = MathF.Abs(randomPos.Y);
            entity.Add(new Transform {
                Position = randomPos,
                Rotation = Quaternion.CreateFromYawPitchRoll(
                    Random.Shared.NextSingle() * MathHelper.TwoPi,
                    Random.Shared.NextSingle() * MathHelper.TwoPi,
                    Random.Shared.NextSingle() * MathHelper.TwoPi
                ),
                Scale = new Vector3(
                    MathUtil.RandomRange(0.5f, 2f),
                    MathUtil.RandomRange(0.5f, 2f),
                    MathUtil.RandomRange(0.5f, 2f)
                )
            });
            entity.Add<Components.Bouncy>(new(Random.Shared.NextSingle()));
        }

        var player = Registry<Prototype>.Get(Registries.Prototypes.Player).Construct();
        player.Entity.Set(new Transform { Position = Vector3.UnitY * 1 });
        RenderPipeline.Camera.Target = player;

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        Time.Update(gameTime);

        Input.InputDisabled = !IsActive;
        Input.RefreshKeyboardState();
        Input.RefreshMouseState();
        Input.RefreshGamePadState();
        Input.UpdateTypingInput(gameTime);

        if (Input.GetPressed(Buttons.Back) || Input.GetPressed(Keys.Escape))
            Exit();

        Internals.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Internals.Draw();
        base.Draw(gameTime);
    }
}
