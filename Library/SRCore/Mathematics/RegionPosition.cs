using System.Numerics;
using System.Runtime.InteropServices;

namespace SRCore.Mathematics;

[StructLayout(LayoutKind.Auto)]
public readonly struct RegionPosition
{
    private readonly RegionId _regionId;
    private readonly Vector3 _offset;

    public RegionId RegionId => _regionId;
    public Vector3 Offset => _offset;

    public RegionPosition(RegionId regionId, Vector3 vector3)
    {
        _regionId = regionId;
        _offset = vector3;
    }
}