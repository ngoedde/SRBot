using System.Diagnostics;
using System.Text;

namespace SRNetwork.SilkroadSecurityApi;

public class Packet
{
    public int CodePage = 1252;
    private ushort m_opcode;
    private PacketWriter m_writer;
    private PacketReader m_reader;
    private bool m_encrypted;
    private bool m_massive;
    private bool m_locked;
    private byte[] m_reader_bytes;
    private object m_lock;

    public override string ToString()
    {
        return $"[{Opcode:X4}][Massive: {Massive}, Encrypted: {Encrypted}]";
    }

    public ushort Opcode
    {
        get { return m_opcode; }
    }
    public bool Encrypted
    {
        get { return m_encrypted; }
    }
    public bool Massive
    {
        get { return m_massive; }
    }

    public Packet(Packet rhs)
    {
        lock (rhs.m_lock)
        {
            m_lock = new object();

            m_opcode = rhs.m_opcode;
            m_encrypted = rhs.m_encrypted;
            m_massive = rhs.m_massive;

            m_locked = rhs.m_locked;
            if (!m_locked)
            {
                m_writer = new PacketWriter();
                m_reader = null;
                m_reader_bytes = null;
                m_writer.Write(rhs.m_writer.GetBytes());
            }
            else
            {
                m_writer = null;
                m_reader_bytes = rhs.m_reader_bytes;
                m_reader = new PacketReader(m_reader_bytes);
            }
        }
    }
    public Packet(ushort opcode)
    {
        m_lock = new object();
        m_opcode = opcode;
        m_encrypted = false;
        m_massive = false;
        m_writer = new PacketWriter();
        m_reader = null;
        m_reader_bytes = null;
    }
    public Packet(ushort opcode, bool encrypted)
    {
        m_lock = new object();
        m_opcode = opcode;
        m_encrypted = encrypted;
        m_massive = false;
        m_writer = new PacketWriter();
        m_reader = null;
        m_reader_bytes = null;
    }
    public Packet(ushort opcode, bool encrypted, bool massive)
    {
        if (encrypted && massive)
        {
            throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");
        }
        m_lock = new object();
        m_opcode = opcode;
        m_encrypted = encrypted;
        m_massive = massive;
        m_writer = new PacketWriter();
        m_reader = null;
        m_reader_bytes = null;
    }
    public Packet(ushort opcode, bool encrypted, bool massive, byte[] bytes)
    {
        if (encrypted && massive)
        {
            throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");
        }
        m_lock = new object();
        m_opcode = opcode;
        m_encrypted = encrypted;
        m_massive = massive;
        m_writer = new PacketWriter();
        m_writer.Write(bytes);
        m_reader = null;
        m_reader_bytes = null;
    }

    public Packet Reset()
    {
        if (m_reader != null)
        {
            m_reader.BaseStream.Position = 0;
        }

        if (m_writer != null)
        {
            m_writer.BaseStream.Position = 0;
        }
        
        m_lock = new object();
        
        return this;
    }
    
    public Packet(ushort opcode, bool encrypted, bool massive, byte[] bytes, int offset, int length)
    {
        if (encrypted && massive)
        {
            throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");
        }
        m_lock = new object();
        m_opcode = opcode;
        m_encrypted = encrypted;
        m_massive = massive;
        m_writer = new PacketWriter();
        m_writer.Write(bytes, offset, length);
        m_reader = null;
        m_reader_bytes = null;
    }

    public byte[] GetBytes()
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                return m_reader_bytes;
            }
            return m_writer.GetBytes();
        }
    }

    public void Lock()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                m_reader_bytes = m_writer.GetBytes();
                m_reader = new PacketReader(m_reader_bytes);
                m_writer.Close();
                m_writer = null;
                m_locked = true;
            }
        }
    }

    public long SeekRead(long offset, SeekOrigin orgin)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot SeekRead on an unlocked Packet.");
            }
            return m_reader.BaseStream.Seek(offset, orgin);
        }
    }

    public int RemainingRead()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot SeekRead on an unlocked Packet.");
            }
            return (int)(m_reader.BaseStream.Length - m_reader.BaseStream.Position);
        }
    }

    public byte ReadByte()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            return m_reader.ReadByte();
        }
    }
    public sbyte ReadSByte()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            return m_reader.ReadSByte();
        }
    }
    public ushort ReadUShort()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            return m_reader.ReadUInt16();
        }
    }
    public short ReadShort()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            return m_reader.ReadInt16();
        }
    }
    public uint ReadUInt()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            return m_reader.ReadUInt32();
        }
    }
    
    public int ReadInt()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            return m_reader.ReadInt32();
        }
    }
    public ulong ReadULong()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            return m_reader.ReadUInt64();
        }
    }
    public long ReadLong()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            return m_reader.ReadInt64();
        }
    }
    public float ReadSingle()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            return m_reader.ReadSingle();
        }
    }
    public double ReadDouble()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            return m_reader.ReadDouble();
        }
    }
    public string ReadString()
    {
        return this.ReadString(CodePage);
    }

    public bool ReadBool()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            
            return m_reader.ReadBoolean();
        }
    }
    
    public string ReadString(int codepage)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }

            ushort length = m_reader.ReadUInt16();
            byte[] bytes = m_reader.ReadBytes(length);

            return Encoding.GetEncoding(codepage).GetString(bytes);
        }
    }
    public string ReadUnicode()
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }

            ushort length = m_reader.ReadUInt16();
            byte[] bytes = m_reader.ReadBytes(length * 2);

            return Encoding.Unicode.GetString(bytes);
        }
    }

    public byte[] ReadByteArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            byte[] values = new byte[count];
            for (int x = 0; x < count; ++x)
            {
                values[x] = m_reader.ReadByte();
            }
            return values;
        }
    }
    public sbyte[] ReadSByteArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            sbyte[] values = new sbyte[count];
            for (int x = 0; x < count; ++x)
            {
                values[x] = m_reader.ReadSByte();
            }
            return values;
        }
    }
    
    
    public ushort[] ReadUShortArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            ushort[] values = new ushort[count];
            for (int x = 0; x < count; ++x)
            {
                values[x] = m_reader.ReadUInt16();
            }
            return values;
        }
    }
    public short[] ReadShortArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            short[] values = new short[count];
            for (int x = 0; x < count; ++x)
            {
                values[x] = m_reader.ReadInt16();
            }
            return values;
        }
    }
    public uint[] ReadUIntArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            uint[] values = new uint[count];
            for (int x = 0; x < count; ++x)
            {
                values[x] = m_reader.ReadUInt32();
            }
            return values;
        }
    }
    public int[] ReadIntArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            int[] values = new int[count];
            for (int x = 0; x < count; ++x)
            {
                values[x] = m_reader.ReadInt32();
            }
            return values;
        }
    }
    public ulong[] ReadULongArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            ulong[] values = new ulong[count];
            for (int x = 0; x < count; ++x)
            {
                values[x] = m_reader.ReadUInt64();
            }
            return values;
        }
    }
    public long[] ReadLongArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            long[] values = new long[count];
            for (int x = 0; x < count; ++x)
            {
                values[x] = m_reader.ReadInt64();
            }
            return values;
        }
    }
    public float[] ReadFloatArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            float[] values = new float[count];
            for (int x = 0; x < count; ++x)
            {
                values[x] = m_reader.ReadSingle();
            }
            return values;
        }
    }
    public double[] ReadDoubleArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            double[] values = new double[count];
            for (int x = 0; x < count; ++x)
            {
                values[x] = m_reader.ReadDouble();
            }
            return values;
        }
    }
    public string[] ReadAsciiArray(int count)
    {
        return this.ReadAsciiArray(CodePage);
    }
    public string[] ReadAsciiArray(int codepage, int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            string[] values = new string[count];
            for (int x = 0; x < count; ++x)
            {
                ushort length = m_reader.ReadUInt16();
                byte[] bytes = m_reader.ReadBytes(length);
                values[x] = Encoding.UTF7.GetString(bytes);
            }
            return values;
        }
    }
    public string[] ReadUnicodeArray(int count)
    {
        lock (m_lock)
        {
            if (!m_locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }
            string[] values = new string[count];
            for (int x = 0; x < count; ++x)
            {
                ushort length = m_reader.ReadUInt16();
                byte[] bytes = m_reader.ReadBytes(length * 2);
                values[x] = Encoding.Unicode.GetString(bytes);
            }
            return values;
        }
    }

    public long SeekWrite(long offset, SeekOrigin orgin)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot SeekWrite on a locked Packet.");
            }
            return m_writer.BaseStream.Seek(offset, orgin);
        }
    }

    public void WriteUInt8(byte value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(value);
        }
    }
    public void WriteByte(sbyte value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(value);
        }
    }
    public void WriteUShort(ushort value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(value);
        }
    }
    public void WriteInt16(short value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(value);
        }
    }
    public void WriteUInt(uint value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(value);
        }
    }
    public void WriteInt32(int value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(value);
        }
    }
    public void WriteUInt64(ulong value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(value);
        }
    }
    public void WriteInt64(long value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(value);
        }
    }
    public void WriteSingle(float value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(value);
        }
    }
    public void WriteDouble(double value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(value);
        }
    }
    public void WriteString(string value)
    {
        this.WriteString(value, CodePage);
    }
    public void WriteString(string value, int code_page)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }

            byte[] codepage_bytes = Encoding.GetEncoding(code_page).GetBytes(value);
            string utf7_value = Encoding.UTF7.GetString(codepage_bytes);
            byte[] bytes = Encoding.Default.GetBytes(utf7_value);

            m_writer.Write((ushort)bytes.Length);
            m_writer.Write(bytes);
        }
    }
    public void WriteUnicode(string value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }

            byte[] bytes = Encoding.Unicode.GetBytes(value);

            m_writer.Write((ushort)value.ToString().Length);
            m_writer.Write(bytes);
        }
    }

    public void WriteUInt8(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write((byte)(Convert.ToUInt64(value) & 0xFF));
        }
    }
    public void WriteByte(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write((sbyte)(Convert.ToInt64(value) & 0xFF));
        }
    }
    public void WriteUShort(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write((ushort)(Convert.ToUInt64(value) & 0xFFFF));
        }
    }
    public void WriteInt16(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write((ushort)(Convert.ToInt64(value) & 0xFFFF));
        }
    }
    public void WriteUInt(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write((uint)(Convert.ToUInt64(value) & 0xFFFFFFFF));
        }
    }
    public void WriteInt32(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write((int)(Convert.ToInt64(value) & 0xFFFFFFFF));
        }
    }
    public void WriteUInt64(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(Convert.ToUInt64(value));
        }
    }
    public void WriteInt64(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(Convert.ToInt64(value));
        }
    }
    public void WriteSingle(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(Convert.ToSingle(value));
        }
    }
    public void WriteDouble(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(Convert.ToDouble(value));
        }
    }
    public void WriteString(object value)
    {
        this.WriteString(value, CodePage);
    }
    public void WriteString(object value, int code_page)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }

            byte[] codepage_bytes = Encoding.GetEncoding(code_page).GetBytes(value.ToString());
            string utf7_value = Encoding.UTF7.GetString(codepage_bytes);
            byte[] bytes = Encoding.Default.GetBytes(utf7_value);

            m_writer.Write((ushort)bytes.Length);
            m_writer.Write(bytes);
        }
    }
    public void WriteUnicode(object value)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }

            byte[] bytes = Encoding.Unicode.GetBytes(value.ToString());

            m_writer.Write((ushort)value.ToString().Length);
            m_writer.Write(bytes);
        }
    }

    public void WriteUInt8Array(byte[] values)
    {
        if (m_locked)
        {
            throw new Exception("Cannot Write to a locked Packet.");
        }
        m_writer.Write(values);
    }
    public void WriteUInt8Array(byte[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }
    }
    public void WriteUInt16Array(ushort[] values)
    {
        this.WriteUInt16Array(values, 0, values.Length);
    }
    public void WriteUInt16Array(ushort[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }
    }
    public void WriteInt16Array(short[] values)
    {
        this.WriteInt16Array(values, 0, values.Length);
    }
    public void WriteInt16Array(short[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }
    }
    public void WriteUInt32Array(uint[] values)
    {
        this.WriteUInt32Array(values, 0, values.Length);
    }
    public void WriteUInt32Array(uint[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }
    }
    public void WriteInt32Array(int[] values)
    {
        this.WriteInt32Array(values, 0, values.Length);
    }
    public void WriteInt32Array(int[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }
    }
    public void WriteUInt64Array(ulong[] values)
    {
        this.WriteUInt64Array(values, 0, values.Length);
    }
    public void WriteUInt64Array(ulong[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }
    }
    public void WriteInt64Array(long[] values)
    {
        this.WriteInt64Array(values, 0, values.Length);
    }
    public void WriteInt64Array(long[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }
    }
    public void WriteSingleArray(float[] values)
    {
        this.WriteSingleArray(values, 0, values.Length);
    }
    public void WriteSingleArray(float[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }
    }
    public void WriteDoubleArray(double[] values)
    {
        this.WriteDoubleArray(values, 0, values.Length);
    }
    public void WriteDoubleArray(double[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }
    }
    public void WriteAsciiArray(string[] values, int codepage)
    {
        this.WriteAsciiArray(values, 0, values.Length, codepage);
    }
    public void WriteAsciiArray(string[] values, int index, int count, int codepage)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteString(values[x], codepage);
            }
        }
    }
    public void WriteAsciiArray(string[] values)
    {
        this.WriteAsciiArray(values, 0, values.Length, CodePage);
    }
    public void WriteAsciiArray(string[] values, int index, int count)
    {
        this.WriteAsciiArray(values, index, count, CodePage);
    }
    public void WriteUnicodeArray(string[] values)
    {
        this.WriteUnicodeArray(values, 0, values.Length);
    }
    public void WriteUnicodeArray(string[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteUnicode(values[x]);
            }
        }
    }

    public void WriteUInt8Array(object[] values)
    {
        this.WriteUInt8Array(values, 0, values.Length);
    }
    public void WriteUInt8Array(object[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteUInt8(values[x]);
            }
        }
    }
    public void WriteByteArray(byte[] values)
    {
        this.WriteByteArray(values, 0, values.Length);
    }
    public void WriteByteArray(byte[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteByte(values[x]);
            }
        }
    }
    public void WriteUInt16Array(object[] values)
    {
        this.WriteUInt16Array(values, 0, values.Length);
    }
    public void WriteUInt16Array(object[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteUShort(values[x]);
            }
        }
    }
    public void WriteInt16Array(object[] values)
    {
        this.WriteInt16Array(values, 0, values.Length);
    }
    public void WriteInt16Array(object[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteInt16(values[x]);
            }
        }
    }
    public void WriteUInt32Array(object[] values)
    {
        this.WriteUInt32Array(values, 0, values.Length);
    }
    public void WriteUInt32Array(object[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteUInt(values[x]);
            }
        }
    }
    public void WriteInt32Array(object[] values)
    {
        this.WriteInt32Array(values, 0, values.Length);
    }
    public void WriteInt32Array(object[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteInt32(values[x]);
            }
        }
    }
    public void WriteUInt64Array(object[] values)
    {
        this.WriteUInt64Array(values, 0, values.Length);
    }
    public void WriteUInt64Array(object[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteUInt64(values[x]);
            }
        }
    }
    public void WriteInt64Array(object[] values)
    {
        this.WriteInt64Array(values, 0, values.Length);
    }
    public void WriteInt64Array(object[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteInt64(values[x]);
            }
        }
    }
    public void WriteSingleArray(object[] values)
    {
        this.WriteSingleArray(values, 0, values.Length);
    }
    public void WriteSingleArray(object[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteSingle(values[x]);
            }
        }
    }
    public void WriteDoubleArray(object[] values)
    {
        this.WriteDoubleArray(values, 0, values.Length);
    }
    public void WriteDoubleArray(object[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteDouble(values[x]);
            }
        }
    }
    public void WriteAsciiArray(object[] values, int codepage)
    {
        this.WriteAsciiArray(values, 0, values.Length, codepage);
    }
    public void WriteAsciiArray(object[] values, int index, int count, int codepage)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteString(values[x].ToString(), codepage);
            }
        }
    }
    public void WriteAsciiArray(object[] values)
    {
        this.WriteAsciiArray(values, 0, values.Length, CodePage);
    }
    public void WriteAsciiArray(object[] values, int index, int count)
    {
        this.WriteAsciiArray(values, index, count, CodePage);
    }
    public void WriteUnicodeArray(object[] values)
    {
        this.WriteUnicodeArray(values, 0, values.Length);
    }
    public void WriteUnicodeArray(object[] values, int index, int count)
    {
        lock (m_lock)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            for (int x = index; x < index + count; ++x)
            {
                this.WriteUnicode(values[x].ToString());
            }
        }
    }
}
