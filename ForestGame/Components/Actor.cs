using Microsoft.Xna.Framework;

namespace ForestGame.Components;

public record struct Actor(Vector3 Velocity, bool HasGravity)
{
    public Actor() : this(Vector3.Zero, true) { }
}
