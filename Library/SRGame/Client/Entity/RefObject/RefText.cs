using SRGame.Client.Repository;

namespace SRGame.Client.Entity.RefObject;

public class RefText : Entity<string>
{
    public override string Id => _nameStrId;
    private string _text;
    private string _nameStrId;
    private bool _active;
    
    public string Text
    {
        get => _text;
        set => _text = value;
    }
    
    public string NameStrId
    {
        get => _nameStrId;
        set => _nameStrId = value;
    }
    
    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    public override bool Parse(EntityParser parser)
    {
        if (!parser.TryParse(0, out _active) || !_active)
            return false;

        var nameStrIndex = 1;
        if (!parser.TryParse(nameStrIndex, out _nameStrId))
            return false;

        var languageTab = 8;
        var maxTabs = parser.GetColumnCount();

        //Try parse with the already set language tab
        parser.TryParse(languageTab, out _text);

        while (IsEmptyString(_text) && languageTab <= maxTabs)
        {
            parser.TryParse(languageTab, out _text);

            languageTab++;
        }

        // fix nullreferenceexception
        if (IsEmptyString(_text))
            Text = "0";

        return true;
    }

    private bool IsEmptyString(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return true;

        if (data == "0")
            return true;

        if (data.StartsWith("?"))
            return true;

        return false;
    }
}