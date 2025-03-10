using ForestGame.Core;

namespace ForestGame.Registries;

public static class Prototypes
{
    public const string Player = "player";
    public const string Teapot = "teapot";

    public static void Initialize()
    {
        Registry.Register<Prototype>(Player, new() {
            Components = {
                Transform.Identity,
                new Components.PlayerControlled(),
                new Components.Actor {
                    HasGravity = true,
                },
                new Components.Motor {
                    Acceleration = 2f,
                    Deceleration = 2f,
                    MaxSpeed = 5f,
                },
                new Components.Bouncy()
            },
        });

        Registry.Register<Prototype>(Teapot, new() {
            Components = {
                Transform.Identity,
                new Components.Actor(),
                new Components.AspectIdentity(Aspects.Teapot),
            },
        });
    }
}
