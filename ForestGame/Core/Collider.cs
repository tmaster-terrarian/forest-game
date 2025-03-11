using Microsoft.Xna.Framework;

namespace ForestGame.Core;

public record struct Collider(Vector3 Position, Vector3 Size, Vector3 Offset)
{
    public Collider() : this(Vector3.Zero, Vector3.One, Vector3.Zero) { }

    public BoundingBox BoundingBox(Vector3 scale) =>
        new(Position + (Offset - Size / 2) * scale, Position + (Offset + Size / 2) * scale);
}
