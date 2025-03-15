using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;

namespace ForestGame.Stages;

public class TestStage : Stage
{
    protected override void Start()
    {
        base.Start();

        var player = Registry<Prototype>.Get(Registries.Prototypes.Player).Construct();
        player.Entity.Set(new Transform { Position = Vector3.UnitY * 10 });
        // player.Entity.Add(new Bouncy());
        RenderPipeline.Camera.Target = player;

        for(int i = 0; i < 30; i++)
        {
            var entity = Registry<Prototype>.Get(Registries.Prototypes.Teapot).Construct().Entity;
            var randomPos = MathUtil.RandomInsideUnitSphere() * 80;
            randomPos.Y = MathF.Abs(randomPos.Y);
            entity.Set<Transform>(new() {
                Position = randomPos,
                Scale = MathUtil.SquashScale(MathUtil.RandomRange(0.8f, 1.5f)) * MathUtil.RandomRange(0.5f, 2f),
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtil.RandomRange(-MathHelper.Pi, MathHelper.Pi))
            });

            // entity.Add<Components.Bouncy>(new(Random.Shared.NextSingle()));
            // entity.Remove<Components.Bouncy>();
        }

        for(int i = 0; i < 30; i++)
        {
            var entity = Registry<Prototype>.Get(Registries.Prototypes.StreetLamp).Construct().Entity;
            var randomPos = MathUtil.RandomInsideUnitSphere() * 80;
            randomPos.Y = MathF.Abs(randomPos.Y) * 0;
            entity.Set<Transform>(new() {
                Position = randomPos,
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtil.RandomRange(-MathHelper.Pi, MathHelper.Pi))
            });
        }

        // EcsManager.world.Create(
        //     new Solid(),
        //     new Collider(new(-3.5f, 0.5f, -3.5f), Vector3.One, Vector3.Zero)
        // );

        EcsManager.world.Create(
            new WorldLooper(new Vector3(80, 0, 80))
        );

        EcsManager.world.Create(
            new Transform {
                Position = new(4.5f, 0.5f, 4.5f),
            },
            new AspectIdentity(Registries.Aspects.Icosphere)
        );
    }
}
