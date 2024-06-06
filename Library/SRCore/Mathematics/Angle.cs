using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRCore.Mathematics;

[StructLayout(LayoutKind.Auto)]
public readonly struct Angle
{
    public static readonly Angle Zero = new Angle(0.0f);

    private readonly float _radians;

    public float Radians => _radians;
    public float Degrees => _radians * MathHelper.RadToDegree;

    public Angle(float radians)
    {
        _radians = radians;
    }

    public Angle(float angleValue, AngleType angleType)
    {
        _radians = angleType switch
        {
            AngleType.Radians => angleValue,
            AngleType.Degrees => angleValue * MathHelper.DegreeToRad,
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    /// Is between -π and π
    /// </summary>
    public bool IsNormalized => _radians is >= -MathHelper.PI and <= MathHelper.PI;

    /// <summary>
    /// Is between 0 and 𝜏 (2π)
    /// </summary>
    public bool IsNormalizedPositive => _radians is >= 0 and <= MathHelper.Tau;

    /// <summary>
    /// Normalizes the Angle to be between -π and π
    /// </summary>
    public static Angle Normalize(Angle angle)
    {
        // https://github.com/sharpdx/SharpDX/blob/master/Source/SharpDX.Mathematics/AngleSingle.cs#L146
        var value = MathF.IEEERemainder(angle._radians, MathHelper.TwoPI);
        switch (value)
        {
            case <= -MathHelper.PI:
                value += MathHelper.PI;
                break;
            case > MathHelper.PI:
                value -= MathHelper.PI;
                break;
        }

        return new Angle(value);
    }

    /// <summary>
    /// Normalizes the Angle to be between 0 and 𝜏 (2π)
    /// </summary>
    public static Angle NormalizePositive(Angle angle)
    {
        // https://github.com/sharpdx/SharpDX/blob/master/Source/SharpDX.Mathematics/AngleSingle.cs#L161
        var value = angle._radians % MathHelper.Tau;
        if (value < 0.0)
            value += MathHelper.Tau;

        return new Angle(value);
    }
}
