namespace SRCore;

[Flags]
public enum ProxyContext : byte
{
    None = 0,
    Gateway = 2,
    Agent = 4,
    Client = 8
}