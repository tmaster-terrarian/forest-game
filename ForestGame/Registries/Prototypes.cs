using ForestGame.Core;
using Microsoft.Xna.Framework;

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
                    Collider = new Collider(Vector3.Zero, new Vector3(0.5f, 1.5f, 0.5f), Vector3.UnitY * 0.75f)
                },
                new Components.Motor {
                    MaxSpeed = 2.5f,
                }
            },
        });

        Registry.Register<Prototype>(Teapot, new() {
            Components = {
                Transform.Identity,
                new Components.Actor
                {
                    Collider = new Collider(Vector3.Zero, Vector3.One * 1.5f, Vector3.UnitY * 0.75f),
                },
                new Components.AspectIdentity(Aspects.Teapot),
            },
        });
    }
}
