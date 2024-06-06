namespace SRGame.Client.Entity.RefObject;

public class RefSkillMastery : Entity<int>
{
    #region Fields

    private int _id;
    public override int Id => _id;

    public string Name;
    public string NameCode;

    public byte GroupNum;
    public string Description;
    public string TabNameCode;
    public byte TabID;

    public byte SkillToolTipType;

    public WeaponType WeaponType1;
    public WeaponType WeaponType2;
    public WeaponType WeaponType3;
    public string Icon;
    public string FocusIcon;

    #endregion Fields

    #region IReferenceObj

    public override bool Parse(EntityParser parser)
    {
        parser.TryParse(0, out _id);
        parser.TryParse(1, out Name);
        // if(Game.ClientType >= GameClientType.Chinese)
        //     parser.TryParse(3, out NameCode);
        parser.TryParse(2, out NameCode);

        parser.TryParse(3, out GroupNum);
        parser.TryParse(4, out Description);
        parser.TryParse(5, out TabNameCode);
        parser.TryParse(6, out TabID);
        parser.TryParse(7, out SkillToolTipType);
        parser.TryParse(8, out WeaponType1);
        parser.TryParse(9, out WeaponType2);
        parser.TryParse(10, out WeaponType3);
        parser.TryParse(11, out Icon);
        parser.TryParse(12, out FocusIcon);

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