using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.Components;

public record struct Actor(Vector3 Velocity, bool HasGravity, Collider Collider, List<CollisionInfo> Collisions)
{
    public Actor() : this(Vector3.Zero, true, new Collider(), []) { }
}
