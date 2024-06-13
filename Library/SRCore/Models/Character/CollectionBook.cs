using System.Collections.ObjectModel;
using SRCore.Models.CollectionBook;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.Character;

public class CollectionBook : ObservableCollection<Theme>
{
    public static CollectionBook FromPacket(Packet packet)
    {
        var collectionBookCount = packet.ReadUInt();
        var result = new CollectionBook();

        for (var i = 0; i < collectionBookCount; i++)
        {
            var theme = new Theme
            {
                Index = packet.ReadUInt(),
                StartedDateTime = packet.ReadUInt(),
                Pages = packet.ReadUInt(),
            };
            result.Add(theme);
        }

        return result;
    }
}