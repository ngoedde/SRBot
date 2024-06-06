namespace SRGame.Client;

public class Division(string name, string[] gatewayServers)
{
    public string Name { get; } = name;

    public string[] GatewayServers { get; } = gatewayServers;
}