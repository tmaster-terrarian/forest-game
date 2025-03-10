using System;
using Microsoft.Xna.Framework;

namespace ForestGame;

public static class MathUtil
{
    public static float Approach(float value, float target, float rate)
    {
        if(value < target)
            return MathHelper.Min(value + rate, target);
        else
            return MathHelper.Max(value - rate, target);
    }

    public static float Approach(ref float value, float target, float rate)
    {
        if(value < target)
            value = MathHelper.Min(value + rate, target);
        else
            value = MathHelper.Max(value - rate, target);
        return value;
    }

    public static int Approach(int value, int target, int rate)
    {
        if(value < target)
            return MathHelper.Min(value + rate, target);
        else
            return MathHelper.Max(value - rate, target);
    }

    public static int Approach(ref int value, int target, int rate)
    {
        if(value < target)
            value = MathHelper.Min(value + rate, target);
        else
            value = MathHelper.Max(value - rate, target);
        return value;
    }

    public static float ApproachNotExceeding(float value, float target, float rate)
    {
        if(Math.Abs(value) < Math.Abs(target))
            value = Approach(value, target, rate);
        return value;
    }

    public static Vector3 ApproachNotExceeding(Vector3 value, Vector3 target, float rate)
    {
        return new(
            ApproachNotExceeding(value.X, target.X, rate),
            ApproachNotExceeding(value.Y, target.Y, rate),
            ApproachNotExceeding(value.Z, target.Z, rate)
        );
    }

    public static float MoveTo(float value, float target, float accel, float decel)
    {
        if(Math.Abs(value) < Math.Abs(target))
            value = Approach(value, target, accel);
        if(Math.Abs(value) > Math.Abs(target))
            value = Approach(value, target, decel);
        return value;
    }

    public static Vector3 MoveTo(Vector3 value, Vector3 target, float accel, float decel)
    {
        return new(
            MoveTo(value.X, target.X, accel, decel),
            MoveTo(value.Y, target.Y, accel, decel),
            MoveTo(value.Z, target.Z, accel, decel)
        );
    }

    public static Vector2 MoveTo(Vector2 value, Vector2 target, float accel, float decel)
    {
        return new(
            MoveTo(value.X, target.X, accel, decel),
            MoveTo(value.Y, target.Y, accel, decel)
        );
    }

    public static float Sqr(float value)
    {
        return value*value;
    }

    public static float Sqrt(float value)
    {
        return MathF.Sqrt(value);
    }

    public static int Sqr(int value)
    {
        return value*value;
    }

    public static float Snap(float value, float interval)
    {
        return MathF.Floor(value / interval) * interval;
    }

    public static float Snap(ref float value, float interval)
    {
        value = MathF.Floor(value / interval) * interval;
        return value;
    }

    public static Vector2 Snap(Vector2 value, Vector2 interval)
    {
        value.X = Snap(value.X, interval.X);
        value.Y = Snap(value.Y, interval.Y);
        return value;
    }

    public static Vector2 Snap(ref Vector2 value, Vector2 interval)
    {
        value.X = Snap(value.X, interval.X);
        value.Y = Snap(value.Y, interval.Y);
        return value;
    }

    public static Vector2 Snap(Vector2 value, float interval)
    {
        value.X = Snap(value.X, interval);
        value.Y = Snap(value.Y, interval);
        return value;
    }

    public static Vector2 Snap(ref Vector2 value, float interval)
    {
        value.X = Snap(value.X, interval);
        value.Y = Snap(value.Y, interval);
        return value;
    }

    public static float SnapCeiling(float value, float interval)
    {
        return MathF.Ceiling(value / interval) * interval;
    }

    public static float SnapCeiling(ref float value, float interval)
    {
        value = MathF.Ceiling(value / interval) * interval;
        return value;
    }

    public static Vector2 SnapCeiling(Vector2 value, Vector2 interval)
    {
        value.X = SnapCeiling(value.X, interval.X);
        value.Y = SnapCeiling(value.Y, interval.Y);
        return value;
    }

    public static Vector2 SnapCeiling(ref Vector2 value, Vector2 interval)
    {
        value.X = SnapCeiling(value.X, interval.X);
        value.Y = SnapCeiling(value.Y, interval.Y);
        return value;
    }

    public static Vector2 SnapCeiling(Vector2 value, float interval)
    {
        value.X = SnapCeiling(value.X, interval);
        value.Y = SnapCeiling(value.Y, interval);
        return value;
    }

    public static Vector2 SnapCeiling(ref Vector2 value, float interval)
    {
        value.X = SnapCeiling(value.X, interval);
        value.Y = SnapCeiling(value.Y, interval);
        return value;
    }

    public static int AbsMax(int value, int max)
    {
        return MathHelper.Max(Math.Abs(value), Math.Abs(max)) * Math.Sign(value);
    }

    public static int AbsMin(int value, int min)
    {
        return MathHelper.Min(Math.Abs(value), Math.Abs(min)) * Math.Sign(value);
    }

    public static int RoundToInt(float value)
    {
        return (int)Math.Round(value);
    }

    public static int CeilToInt(float value)
    {
        return (int)Math.Ceiling(value);
    }

    public static int FloorToInt(float value)
    {
        return (int)Math.Floor(value);
    }

    public static int ClampToInt(float value, int a, int b)
    {
        return (int)Math.Clamp(value, a, b);
    }

    public static float InverseLerp(float a, float b, float t)
    {
        return (t - a)/(b - a);
    }

    public static float InverseLerp01(float a, float b, float t)
    {
        return Clamp01((t - a)/(b - a));
    }

    public static float Clamp01(float value)
    {
        return Math.Clamp(value, 0, 1);
    }

    /// <summary>
    /// Exponential decay function
    /// </summary>
    /// <param name="a">Start value</param>
    /// <param name="b">Destination value</param>
    /// <param name="decay">Useful range approx. 1 to 25, from slow to fast. 16 is a good default.</param>
    /// <param name="dt">Delta Time (in seconds)</param>
    /// <returns></returns>
    public static float ExpDecay(float a, float b, float decay, float dt)
    {
        return b+(a-b)*MathF.Exp(-decay*dt);
    }

    public static Vector2 ExpDecay(Vector2 a, Vector2 b, float decay, float dt)
    {
        return b+(a-b)*MathF.Exp(-decay*dt);
    }

    public static Vector3 ExpDecay(Vector3 a, Vector3 b, float decay, float dt)
    {
        return b+(a-b)*MathF.Exp(-decay*dt);
    }

    public static Quaternion ExpDecay(Quaternion a, Quaternion b, float decay, float dt)
    {
        return Quaternion.Slerp(a, b, 1 - MathF.Exp(-decay * dt));
    }

    public static int Sign(float a)
    {
        return MathF.Sign(a);
    }

    public static bool Approximately(float a, float b, float threshold)
    {
        return MathF.Abs(a - b) < threshold;
    }

    public static float SmoothCos(float value, float exp)
    {
        return MathF.Pow(-0.5f * MathF.Cos(value * MathF.PI) + 0.5f, exp);
    }

    public static float SmoothCosClamp(float value, float exp)
    {
        return SmoothCos(Clamp01(value), exp);
    }

    public static float RandomRange(float min, float max)
    {
        return Random.Shared.NextSingle() * (max - min) + min;
    }

    public static Vector3 RandomInsideUnitSphere() {
        var u = Random.Shared.NextSingle();
        var v = Random.Shared.NextSingle();
        var theta = u * 2.0f * MathF.PI;
        var phi = MathF.Acos(2.0f * v - 1.0f);
        var r = MathF.Cbrt(Random.Shared.NextSingle());
        var sinTheta = MathF.Sin(theta);
        var cosTheta = MathF.Cos(theta);
        var sinPhi = MathF.Sin(phi);
        var cosPhi = MathF.Cos(phi);
        var x = r * sinPhi * cosTheta;
        var y = r * sinPhi * sinTheta;
        var z = r * cosPhi;
        return new Vector3(x, y, z);
    }

    public static Vector3 Project(Vector3 vector, Vector3 onNormal)
    {
        if (vector == Vector3.Zero) return Vector3.Zero;
        float num = Vector3.Dot(onNormal, onNormal);
        if (num < float.Epsilon)
            return Vector3.Zero;
        return onNormal * Vector3.Dot(vector, onNormal) / num;
    }

    public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
    {
        if (vector == Vector3.Zero) return Vector3.Zero;
        return vector - Project(vector, planeNormal);
    }

    public static Vector3 RandomVector3(float min, float max)
    {
        return new(
            Random.Shared.NextSingle() * (max - min) + min,
            Random.Shared.NextSingle() * (max - min) + min,
            Random.Shared.NextSingle() * (max - min) + min
        );
    }

    public static Vector3 RandomVector3(Vector3 min, Vector3 max)
    {
        return new(
            Random.Shared.NextSingle() * (max.X - min.X) + min.X,
            Random.Shared.NextSingle() * (max.Y - min.Y) + min.Y,
            Random.Shared.NextSingle() * (max.Z - min.Z) + min.Z
        );
    }
}
