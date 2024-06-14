namespace SRGame.Client.Entity.RefObject;

public class RefText : Entity<string>
{
    public override string Id => NameStrId;

    private const int LANG_OFFSET = 2;
    private const int LANG_COUNT = 14;
    public string Text;
    public string NameStrId;
    private bool Active;

    //private readonly string[] _data = new string[LANG_COUNT];

    // languageFlag
    // 0 - Korean
    // 1 - Chinese
    // 2 - Taiwan
    // 3 - Japan
    // 4 - English
    // 5 - Vietnam
    // 7 - Turkey
    // 8 - Thailand
    // 9 - Russia
    // 10 - Spain
    // 11 - Arabic

    //public string Lang0 => _data[OFFSET + 0];
    //public string Lang1 => _data[OFFSET + 1];
    //public string Lang2 => _data[OFFSET + 2];
    //public string Lang3 => _data[OFFSET + 3];
    //public string Lang4 => _data[OFFSET + 4];
    //public string Lang5 => _data[OFFSET + 5];
    //public string Lang6 => _data[OFFSET + 6];
    //public string Lang7 => _data[OFFSET + 7];
    //public string Lang8 => _data[OFFSET + 8];

    //public string this[int index] => _data[index];

    public override bool Parse(EntityParser parser)
    {
        if (!parser.TryParse(0, out Active) || !Active)
            return false;

        var nameStrIndex = 1;
        if (!parser.TryParse(nameStrIndex, out NameStrId))
            return false;

        var languageTab = 8;
        var maxTabs = parser.GetColumnCount();

        //Try parse with the already set language tab
        parser.TryParse(languageTab, out Text);

        while (IsEmptyString(Text) && languageTab <= maxTabs)
        {
            parser.TryParse(languageTab, out Text);

            languageTab++;
        }

        // fix nullreferenceexception
        if (IsEmptyString(Text))
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