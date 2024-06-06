using System.Runtime.InteropServices;

namespace SRGame.Mathematics;


[StructLayout(LayoutKind.Explicit)]
public readonly struct TypeId
    : System.IEquatable<TypeId>
{
    [FieldOffset(0)]
    private readonly int _id;

    [FieldOffset(0)]
    private readonly byte _tid1;
    
    [FieldOffset(1)]
    private readonly byte _tid2;
    
    [FieldOffset(2)]
    private readonly byte _tid3;
    
    [FieldOffset(3)]
    private readonly byte _tid4;
    
    public byte T1 => _tid1;
    public byte T2 => _tid2;
    public byte T3 => _tid3;
    public byte T4 => _tid4;
    public int Id => _id;
    
    public bool Equals(TypeId other)
    {
        return _id == other._id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_id, _tid1, _tid2, _tid3, _tid4);
    }

    public static bool operator ==(TypeId left, TypeId right) => left.Equals(right);

    public static bool operator !=(TypeId left, TypeId right) => !(left == right);

    public static implicit operator int(TypeId value) => value._id;

    public override string ToString() => $"T1: {_tid1} - T2: {_tid2} - T3: {_tid3} - T4: {_tid4} - {_id}";

    public override bool Equals(object obj)
    {
        return obj is TypeId && Equals((TypeId)obj);
    }

    public TypeId(byte t1, byte t2, byte t3, byte t4)
    {
        _id =  (t1 << 1) | (t2 << 2) | (t3 << 3) | (t4 << 4);
        _tid1 = t1;
        _tid2 = t2;
        _tid3 = t3;
        _tid4 = t4;
    }
}