using Microsoft.Xna.Framework;

namespace ForestGame.Core;

public struct CollisionInfo(Vector3 normal)
{
    public Vector3 Normal = normal;
}
