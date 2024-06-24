using System.Diagnostics;
using SRGame.Client.Repository;
using SRGame.Mathematics;

namespace SRGame.Client.Entity.RefObject;

[DebuggerDisplay("ID = {Id}; Code = {CodeName}; Name = {ObjName}")]
public abstract class RefObjCommon : Entity<int>
{
    public TypeId TypeID => new(TypeID1, TypeID2, TypeID3, TypeID4);
    
    #region Fields

    private int _id = 0;
    public override int Id => _id;

    private bool _active;
    private string _codeName;
    private string _objName;
    private string _orgObjCodeName;
    private string _nameStrID;
    private string _descStrID;
    private byte _cashItem;
    private byte _bionic;
    private byte _typeID1;
    private byte _typeID2;
    private byte _typeID3;
    private byte _typeID4;
    private int _decayTime;
    private Country _country;
    private Rarity _rarity;
    private bool _canTrade;
    private bool _canSell;
    private bool _canBuy;
    private BorrowType _canBorrow;
    private DropType _canDrop;
    private bool _canPick;
    private bool _canRepair;
    private bool _canRevive;
    private UseType _canUse;
    private bool _canThrow;
    private int _price;
    private int _costRepair;
    private int _costRevive;
    private int _costBorrow;
    private int _keepingFee;
    private int _sellPrice;
    private ReqLevelType _reqLevelType1;
    private byte _reqLevel1;
    private ReqLevelType _reqLevelType2;
    private byte _reqLevel2;
    private ReqLevelType _reqLevelType3;
    private byte _reqLevel3;
    private ReqLevelType _reqLevelType4;
    private byte _reqLevel4;
    private int _maxContain;
    private short _regionID;
    private short _angle;
    private short _offsetZ;
    private short _offsetX;
    private short _offsetY;
    private short _speed1;
    private short _speed2;
    private int _scale;
    private short _bcHeight;
    private short _bcRadius;
    private int _eventID;
    private string _assocFileObj;
    private string _assocFileDrop;
    private string _assocFileIcon;
    private string _assocFile1;
    private string _assocFile2;

    [Translation(nameof(NameStrID))] public string Name { get; set; } = string.Empty;

    [Translation(nameof(DescStrID))] public string Description { get; set; } = string.Empty;

    public bool Active
    {
        get => _active;
        private set => _active = value;
    }

    public string CodeName
    {
        get => _codeName;
        private set => _codeName = value;
    }

    public string ObjName
    {
        get => _objName;
        private set => _objName = value;
    }

    public string OrgObjCodeName => _orgObjCodeName;

    public string NameStrID => _nameStrID;

    public string DescStrID => _descStrID;

    public byte CashItem => _cashItem;

    public byte Bionic => _bionic;

    public byte TypeID1 => _typeID1;

    public byte TypeID2 => _typeID2;

    public byte TypeID3 => _typeID3;

    public byte TypeID4 => _typeID4;

    public int DecayTime => _decayTime;

    public Country Country => _country;

    public Rarity Rarity => _rarity;

    public bool CanTrade => _canTrade;

    public bool CanSell => _canSell;

    public bool CanBuy => _canBuy;

    public BorrowType CanBorrow => _canBorrow;

    public DropType CanDrop => _canDrop;

    public bool CanPick => _canPick;

    public bool CanRepair => _canRepair;

    public bool CanRevive => _canRevive;

    public UseType CanUse => _canUse;

    public bool CanThrow => _canThrow;

    public int Price => _price;

    public int CostRepair => _costRepair;

    public int CostRevive => _costRevive;

    public int CostBorrow => _costBorrow;

    public int KeepingFee => _keepingFee;

    public int SellPrice => _sellPrice;

    public ReqLevelType ReqLevelType1 => _reqLevelType1;

    public byte ReqLevel1 => _reqLevel1;

    public ReqLevelType ReqLevelType2 => _reqLevelType2;

    public byte ReqLevel2 => _reqLevel2;

    public ReqLevelType ReqLevelType3 => _reqLevelType3;

    public byte ReqLevel3 => _reqLevel3;

    public ReqLevelType ReqLevelType4 => _reqLevelType4;

    public byte ReqLevel4 => _reqLevel4;

    public int MaxContain => _maxContain;

    public short RegionID => _regionID;

    public short Angle => _angle;

    public short OffsetZ => _offsetZ;

    public short OffsetX => _offsetX;
    public short OffsetY => _offsetY;
    public short Speed1 => _speed1;
    public short Speed2 => _speed2;
    public int Scale => _scale;
    public short BCHeight => _bcHeight;
    public short BCRadius => _bcRadius;
    public int EventId => _eventID;
    public string AssocFileObj => _assocFileObj;
    public string AssocFileDrop => _assocFileDrop;
    public string AssocFileIcon => _assocFileIcon;
    public string AssocFile1 => _assocFile1;
    public string AssocFile2 => _assocFile2;

    #endregion Fields

    public override bool Parse(EntityParser parser)
    {
        //Skip disabled
        if (!parser.TryParse(0, out _active) || _active == false)
            return false;

        //Skip invalid ID (PK)
        if (!parser.TryParse(1, out _id))
            return false;

        //Skip invalid CodeName
        if (!parser.TryParse(2, out _codeName))
            return false;

        parser.TryParse(3, out _objName);
        parser.TryParse(4, out _orgObjCodeName);
        parser.TryParse(5, out _nameStrID);
        parser.TryParse(6, out _descStrID);
        parser.TryParse(7, out _cashItem);
        parser.TryParse(8, out _bionic);
        parser.TryParse(9, out _typeID1);
        parser.TryParse(10, out _typeID2);
        parser.TryParse(11, out _typeID3);
        parser.TryParse(12, out _typeID4);
        parser.TryParse(13, out _decayTime);
        parser.TryParse(14, out _country);
        parser.TryParse(15, out _rarity);
        parser.TryParse(16, out _canTrade);
        parser.TryParse(17, out _canSell);
        parser.TryParse(18, out _canBuy);
        parser.TryParse(19, out _canBorrow);
        parser.TryParse(20, out _canDrop);
        parser.TryParse(21, out _canPick);
        parser.TryParse(22, out _canRepair);
        parser.TryParse(23, out _canRevive);
        parser.TryParse(24, out _canUse);
        parser.TryParse(25, out _canThrow);
        parser.TryParse(26, out _price);
        parser.TryParse(27, out _costRepair);
        parser.TryParse(28, out _costRevive);
        parser.TryParse(29, out _costBorrow);
        parser.TryParse(30, out _keepingFee);
        parser.TryParse(31, out _sellPrice);
        parser.TryParse(32, out _reqLevelType1);
        parser.TryParse(33, out _reqLevel1);
        parser.TryParse(34, out _reqLevelType2);
        parser.TryParse(35, out _reqLevel2);
        parser.TryParse(36, out _reqLevelType3);
        parser.TryParse(37, out _reqLevel3);
        parser.TryParse(38, out _reqLevelType4);
        parser.TryParse(39, out _reqLevel4);
        parser.TryParse(40, out _maxContain);
        parser.TryParse(41, out _regionID);
        parser.TryParse(42, out _angle);
        parser.TryParse(43, out _offsetX);
        parser.TryParse(44, out _offsetY);
        parser.TryParse(45, out _offsetZ);
        parser.TryParse(46, out _speed1);
        parser.TryParse(47, out _speed2);
        parser.TryParse(48, out _scale);
        parser.TryParse(49, out _bcHeight);
        parser.TryParse(50, out _bcRadius);
        parser.TryParse(51, out _eventID);
        parser.TryParse(52, out _assocFileObj);
        parser.TryParse(53, out _assocFileDrop);
        parser.TryParse(54, out _assocFileIcon);
        parser.TryParse(55, out _assocFile1);
        parser.TryParse(56, out _assocFile2);

        return true;
    }
}