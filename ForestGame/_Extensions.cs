using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame;

public static class _Extensions
{
    public static Matrix ToXna(this Assimp.Matrix4x4 matrix4X4)
    {
        return new(
            matrix4X4.A1, matrix4X4.B1, matrix4X4.C1, matrix4X4.D1,
            matrix4X4.A2, matrix4X4.B2, matrix4X4.C2, matrix4X4.D2,
            matrix4X4.A3, matrix4X4.B3, matrix4X4.C3, matrix4X4.D3,
            matrix4X4.A4, matrix4X4.B4, matrix4X4.C4, matrix4X4.D4
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

    public static Color ToXna(this Assimp.Color4D color)
    {
        return new(color.R, color.G, color.B, color.A);
    }

    public static Color ToXna(this Assimp.Color3D color)
    {
        return new(color.R, color.G, color.B, 1f);
    }

    public static Vector3 ToXna(this Assimp.Vector3D vector)
    {
        return new(vector.X, vector.Y, vector.Z);
    }

    public static Assimp.Vector2D Truncate(this Assimp.Vector3D vector)
    {
        return new(vector.X, vector.Y);
    }

    public static Vector2 ToXna(this Assimp.Vector2D vector)
    {
        return new(vector.X, vector.Y);
    }

    public static Vector3 Median(this BoundingBox boundingBox)
    {
        return Vector3.Lerp(boundingBox.Min, boundingBox.Max, 0.5f);
    }

    public static TValue? AddOrGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue?> getDefault)
    {
        if(!dict.TryGetValue(key, out var value))
        {
            value = getDefault();
            dict.Add(key, value!);
        }

        return value;
    }

    /// <returns>true if the default value was added, false if a value was already present</returns>
    public static bool TryAddOrGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> getDefault, out TValue result)
    {
        result = dict.AddOrGet(key, getDefault)!;
        return !(result?.Equals(default(TValue)) ?? true);
    }

    public static BoundingBox MinkowskiDifference(this BoundingBox a, BoundingBox b)
    {
        float minkowskiFront = a.Max.Z - b.Min.Z;
        float minkowskiBack = a.Min.Z - b.Max.Z;
        float minkowskiRight = a.Max.X - b.Min.X;
        float minkowskiLeft = a.Min.X - b.Max.X;
        float minkowskiTop = a.Max.Y - b.Min.Y;
        float minkowskiBottom = a.Min.Y - b.Max.Y;

        return new BoundingBox(
            new Vector3(minkowskiLeft, minkowskiBottom, minkowskiBack),
            new Vector3(minkowskiRight, minkowskiTop, minkowskiFront)
        );
    }

    public static void DrawStringSpacesFix(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0, bool rtl = false)
    {
        var split = text.Split(' ');
        float x = 0;
        int spaceSize = MathUtil.RoundToInt(font.MeasureString("t").X);
        foreach(var word in split)
        {
            spriteBatch.DrawString(font, word, position + (Vector2.UnitX * x * scale.X), color, rotation, origin, scale, effects, layerDepth, rtl);
            x += font.MeasureString(word).X + spaceSize;
        }
    }

    public static void DrawStringSpacesFix(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale = 1, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0, bool rtl = false)
    {
        spriteBatch.DrawStringSpacesFix(font, text, position, color, rotation, origin, new Vector2(scale), effects, layerDepth, rtl);
    }

    public static void DrawStringSpacesFix(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color)
    {
        spriteBatch.DrawStringSpacesFix(font, text, position, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
    }
}
