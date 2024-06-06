namespace SRCore;

[Flags]
public enum ProxyContext : byte
{
    None,
    Gateway,
    Agent,
    Client
}