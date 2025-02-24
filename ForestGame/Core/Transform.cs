using Microsoft.Xna.Framework;

namespace ForestGame.Core;

public struct Transform()
{
    public static Transform Identity { get; } = new();

    public Vector3 Scale { get; set; } = Vector3.One;
    public Quaternion Rotation { get; set; } = Quaternion.Identity;
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Vector3 Origin { get; set; } = Vector3.Zero;

    public readonly Matrix Matrix
        => Matrix.CreateTranslation(Origin)
         * Matrix.CreateScale(Scale)
         * Matrix.CreateFromQuaternion(Rotation)
         * Matrix.CreateWorld(Position, Vector3.UnitZ, Vector3.Up);

    public static implicit operator Matrix(Transform transform)
    {
        return transform.Matrix;
    }
}
