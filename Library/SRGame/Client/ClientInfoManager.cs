using System.Net;
using System.Net.Sockets;
using System.Text;
using SRPack.SRAdapter.Utils;

namespace SRGame.Client;

public class ClientInfoManager(ClientFileSystem fileSystem)
{
    public delegate void LoadedEventArgs();

    public event LoadedEventArgs? Loaded;

    public DivisionInfo DivisionInfo { get; private set; } = new(0, []);

    public ushort GatewayPort { get; private set; }

    public uint Version { get; private set; }

    public string Path => fileSystem.Path;

    public async Task LoadAsync()
    {
        DivisionInfo = await LoadDivisionInfoAsync();
        GatewayPort = await LoadGatewayPort();
        Version = await LoadVersion();

        OnLoaded();
    }

    private async Task<DivisionInfo> LoadDivisionInfoAsync()
    {
        var divisionFile = await fileSystem.ReadFileStream(AssetPack.Media, "divisioninfo.txt");
        using var reader = new BinaryReader(divisionFile);

        var locale = reader.ReadByte();
        var divisionCount = reader.ReadByte();
        var divisions = new Division[divisionCount];

        for (var iDivision = 0; iDivision < divisionCount; iDivision++)
        {
            var name = reader.ReadStringFixed();
            reader.ReadByte(); //NOP

            var gatewayCount = reader.ReadByte();
            var gateways = new string[gatewayCount];

            for (var iGateway = 0; iGateway < gatewayCount; iGateway++)
            {
                gateways[iGateway] = reader.ReadStringFixed();
                reader.ReadByte(); //NOP
            }

            divisions[iDivision] = new Division(name, gateways);
        }

        return new DivisionInfo(locale, divisions);
    }

    private async Task<ushort> LoadGatewayPort()
    {
        var gateportFile = await fileSystem.ReadFileText(AssetPack.Media, "gateport.txt");

        return ushort.Parse(gateportFile);
    }

    private async Task<uint> LoadVersion()
    {
        var versionFile = await fileSystem.ReadFileStream(AssetPack.Media, "SV.T");

        using var reader = new BinaryReader(versionFile);

        var versionBufferLength = reader.ReadInt32();
        var versionBuffer = reader.ReadBytes(versionBufferLength);

        var blowfish = new Blowfish();
        blowfish.Initialize("SILKROADVERSION"u8.ToArray(), 0, 8);

        var decodedVersionBuffer = blowfish.Decode(versionBuffer);
        return decodedVersionBuffer == null
            ? 0
            : uint.Parse(Encoding.ASCII.GetString(decodedVersionBuffer, 0, 4));
    }

    public IPEndPoint GetGatewayEndPoint()
    {
        var division = DivisionInfo.Divisions[0].GatewayServers[0];

        return ToIPEndPoint(division, GatewayPort);
    }

    private static IPEndPoint ToIPEndPoint(string? hostOrIP, ushort port)
    {
        ArgumentNullException.ThrowIfNull(hostOrIP);

        if (!IPAddress.TryParse(hostOrIP, out IPAddress? address))
            address = Array.Find(Dns.GetHostEntry(hostOrIP).AddressList,
                p => p.AddressFamily == AddressFamily.InterNetwork);

        return new IPEndPoint(address!, port);
    }

    protected virtual void OnLoaded()
    {
        Loaded?.Invoke();
    }
}