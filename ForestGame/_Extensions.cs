using Microsoft.Xna.Framework;

namespace ForestGame;

public static class _Extensions
{
    public static Matrix ToXna(this Assimp.Matrix4x4 matrix4X4)
    {
        return new(
            matrix4X4.A1, matrix4X4.A2, matrix4X4.A3, matrix4X4.A4,
            matrix4X4.B1, matrix4X4.B2, matrix4X4.B3, matrix4X4.B4,
            matrix4X4.C1, matrix4X4.C2, matrix4X4.C3, matrix4X4.C4,
            matrix4X4.D1, matrix4X4.D2, matrix4X4.D3, matrix4X4.D4
        );
    }

    public static Assimp.Matrix4x4 ToAssimp(this Matrix matrix4X4)
    {
        return new(
            matrix4X4.M11, matrix4X4.M12, matrix4X4.M13, matrix4X4.M14,
            matrix4X4.M21, matrix4X4.M22, matrix4X4.M23, matrix4X4.M24,
            matrix4X4.M31, matrix4X4.M32, matrix4X4.M33, matrix4X4.M34,
            matrix4X4.M41, matrix4X4.M42, matrix4X4.M43, matrix4X4.M44
        );
    }
}
