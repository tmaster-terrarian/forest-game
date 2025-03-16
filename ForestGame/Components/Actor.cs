using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.Components;

public struct Actor()
{
    public Vector3 Velocity { get; set; }
    public bool HasGravity { get; set; } = true;
    public CollisionInfo[] Collisions { get; set; } = [];
    public bool IsGrounded { get; set; }
}
