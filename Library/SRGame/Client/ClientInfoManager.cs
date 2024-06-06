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
        var divisionFile = await fileSystem.ReadFileBytes(AssetPack.Media, "divisioninfo.txt");
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
        var versionFile = await fileSystem.ReadFileBytes(AssetPack.Media, "SV.T");

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

    protected virtual void OnLoaded()
    {
        Loaded?.Invoke();
    }
}