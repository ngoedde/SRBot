namespace SRPack.SRAdapter.Struct;

internal record SRPackHeader
{
    public const int HeaderSize = 256;
    public const string BlowfishChecksumDecoded = "Joymax Pak File";
    
    public bool Encrypted = false;
    public byte[] EncryptionChecksum = [];
    public byte[] Payload = new byte[205];
    public string Signature = string.Empty;
    public byte[] Version = [0,0,0,0];
}