namespace SRGame.Client;

public class DivisionInfo(byte contentId, Division[] divisions)
{
    public byte ContentId { get; } = contentId;

    public Division[] Divisions { get; } = divisions;
}