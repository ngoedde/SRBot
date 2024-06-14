using SRGame.Client.Repository;

namespace SRGame.Client.Entity.RefObject;

public class RefSkillMastery : Entity<int>
{
    #region Fields

    private int _id;
    private string _name;
    private string _nameCode;
    private byte _groupNum;
    private string _description;
    private string _tabNameCode;
    private byte _tabID;
    private byte _skillToolTipType;
    private WeaponType _weaponType1;
    private WeaponType _weaponType2;
    private WeaponType _weaponType3;
    private string _icon;
    private string _focusIcon;

    public override int Id => _id;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public string NameCode
    {
        get => _nameCode;
        set => _nameCode = value;
    }

    public byte GroupNum
    {
        get => _groupNum;
        set => _groupNum = value;
    }

    public string Description
    {
        get => _description;
        set => _description = value;
    }

    public string TabNameCode
    {
        get => _tabNameCode;
        set => _tabNameCode = value;
    }

    public byte TabID
    {
        get => _tabID;
        set => _tabID = value;
    }

    public byte SkillToolTipType
    {
        get => _skillToolTipType;
        set => _skillToolTipType = value;
    }

    public WeaponType WeaponType1
    {
        get => _weaponType1;
        set => _weaponType1 = value;
    }

    public WeaponType WeaponType2
    {
        get => _weaponType2;
        set => _weaponType2 = value;
    }

    public WeaponType WeaponType3
    {
        get => _weaponType3;
        set => _weaponType3 = value;
    }

    public string Icon
    {
        get => _icon;
        set => _icon = value;
    }

    public string FocusIcon
    {
        get => _focusIcon;
        set => _focusIcon = value;
    }

    #endregion Fields

    #region IReferenceObj

    public override bool Parse(EntityParser parser)
    {
        parser.TryParse(0, out _id);
        parser.TryParse(1, out _name);
        parser.TryParse(2, out _nameCode);
        parser.TryParse(3, out _groupNum);
        parser.TryParse(4, out _description);
        parser.TryParse(5, out _tabNameCode);
        parser.TryParse(6, out _tabID);
        parser.TryParse(7, out _skillToolTipType);
        parser.TryParse(8, out _weaponType1);
        parser.TryParse(9, out _weaponType2);
        parser.TryParse(10, out _weaponType3);
        parser.TryParse(11, out _icon);
        parser.TryParse(12, out _focusIcon);

        return true;
    }

    #endregion IReferenceObj
}

//Mastery ID: 257
//Mastery Name - Do Not Use: 비천검법101
//MasteryNameCode: UIIT_STT_MASTERY_VI
//GroupNum: 10
//Mastery Description ID: UIIT_STT_MASTERY_VI_EXPLANATION
//Tab Name Code: UIIT_CTL_WEAPON_SKILL
//Type (TabID): 0
//SkillToolTipType: 0
//Weapon Type 1: 2
//Weapon Type 2: 3
//Weapon Type 3: 0
//Mastery Icon: icon\skillmastery\china\mastery_sword.ddj
//Mastery Focus Icon: icon\skillmastery\china\mastery_sword_focus.ddj