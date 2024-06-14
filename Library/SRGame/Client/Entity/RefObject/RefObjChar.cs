namespace SRGame.Client.Entity.RefObject;

public class RefObjChar : RefObjCommon
{
    private byte _level;
    private Gender _charGender;
    private int _maxHealth;
    private int _maxMana;
    private byte _inventorySize;
    private byte _canStoreTID1;
    private byte _canStoreTID2;
    private byte _canStoreTID3;
    private byte _canStoreTID4;
    private byte _canBeVehicle;
    private byte _canControl;
    private byte _damagePortion;
    private short _maxPassenger;

    public byte Level
    {
        get => _level;
        set => _level = value;
    }

    public Gender CharGender
    {
        get => _charGender;
        set => _charGender = value;
    }

    public int MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    public int MaxMP
    {
        get => _maxMana;
        set => _maxMana = value;
    }

    public byte InventorySize
    {
        get => _inventorySize;
        set => _inventorySize = value;
    }

    public byte CanStoreTID1
    {
        get => _canStoreTID1;
        set => _canStoreTID1 = value;
    }

    public byte CanStoreTID2
    {
        get => _canStoreTID2;
        set => _canStoreTID2 = value;
    }

    public byte CanStoreTID3
    {
        get => _canStoreTID3;
        set => _canStoreTID3 = value;
    }

    public byte CanStoreTID4
    {
        get => _canStoreTID4;
        set => _canStoreTID4 = value;
    }

    public byte CanBeVehicle
    {
        get => _canBeVehicle;
        set => _canBeVehicle = value;
    }

    public byte CanControl
    {
        get => _canControl;
        set => _canControl = value;
    }

    public byte DamagePortion
    {
        get => _damagePortion;
        set => _damagePortion = value;
    }

    public short MaxPassenger
    {
        get => _maxPassenger;
        set => _maxPassenger = value;
    }

    public override bool Parse(EntityParser parser)
    {
        if (!base.Parse(parser))
            return false;

        parser.TryParse(57, out _level);
        parser.TryParse(58, out _charGender);
        parser.TryParse(59, out _maxHealth);
        parser.TryParse(60, out _maxMana);
        parser.TryParse(61, out _inventorySize);
        parser.TryParse(62, out _canStoreTID1);
        parser.TryParse(63, out _canStoreTID2);
        parser.TryParse(64, out _canStoreTID3);
        parser.TryParse(65, out _canStoreTID4);
        parser.TryParse(66, out _canBeVehicle);
        parser.TryParse(67, out _canControl);
        parser.TryParse(68, out _damagePortion);
        parser.TryParse(69, out _maxPassenger);

        return true;
    }
}