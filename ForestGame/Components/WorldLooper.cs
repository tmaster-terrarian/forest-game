using Microsoft.Xna.Framework;

namespace ForestGame.Components;

public record struct WorldLooper(Vector3 WorldSize)
{
    public Vector3 GetPosInWorld(Vector3 pos)
    {
        return new Vector3(pos.X % WorldSize.X, pos.Y % WorldSize.Y, pos.Z % WorldSize.Z);
    }
}
