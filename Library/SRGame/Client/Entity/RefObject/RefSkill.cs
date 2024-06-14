using SRGame.Client.Repository;

namespace SRGame.Client.Entity.RefObject;

public class RefSkill : Entity<int>
{
    private const int PARAM_COUNT = 50;

    private const int EFFECT_TRANSFER = 1701213281; //efta
    private const int DURATION = 1685418593; //dura

    #region Fields

    private int _id;
    private byte _service;
    private int _groupID;
    private string _basicCode;
    private string _basicName;
    private string _codeName;
    private string _basicGroup;
    private int _basicOriginal;
    private byte _basicLevel;
    private byte _basicActivity;
    private uint _chainCode;
    private int _basicRecycleCost;
    private int _actionPreparingTime;
    private int _actionCastingTime;
    private int _actionDuration;
    private int _actionReuseDelay;
    private int _actionCoolTime;
    private int _actionFlyingSpeed;
    private byte _actionInterruptable;
    private int _actionOverlap;
    private int _actionAutoAttackType;
    private int _actionInTown;
    private short _actionRange;
    private bool _targetRequired;
    private bool _targetTypeAnimal;
    private bool _targetTypeLand;
    private bool _targetTypeBuilding;
    private bool _targetGroupSelf;
    private bool _targetGroupAlly;
    private bool _targetGroupParty;
    private bool _targetGroupEnemyMonster;
    private bool _targetGroupEnemyPlayer;
    private bool _targetGroupNeutral;
    private bool _targetGroupDontCare;
    private bool _targetEtcSelectDeadBody;
    private int _requiredMastery1;
    private int _requiredMastery2;
    private byte _requiredMasteryLevel1;
    private byte _requiredMasteryLevel2;
    private short _requiredStrength;
    private short _requiredIntelligence;
    private int _requiredLearnSkill1;
    private int _requiredLearnSkill2;
    private int _requiredLearnSkill3;
    private byte _requiredLearnSkillLevel1;
    private byte _requiredLearnSkillLevel2;
    private byte _requiredLearnSkillLevel3;
    private int _requiredLearnSkillPoint;
    private byte _requiredRace;
    private byte _requiredRestriction1;
    private byte _requiredRestriction2;
    private WeaponType _requiredWeapon1;
    private WeaponType _requiredWeapon2;
    private short _consumeHealth;
    private short _consumeMana;
    private short _consumeHealthRatio;
    private short _consumeManaRatio;
    private byte _consumeHwan;
    private byte _uiSkillTab;
    private byte _uiSkillPage;
    private byte _uiSkillColumn;
    private byte _uiSkillRow;
    private string _uiIconFile;
    private string _uiSkillName;
    private string _uiSkillToolTip;
    private string _uiSkillToolTipDesc;
    private string _uiSkillStudyDesc;
    private short _aiAttackChance;
    private byte _aiSkillType;
    private List<int> _params = new(PARAM_COUNT);

    public override int Id => _id;

    [Translation(nameof(UISkillName))]
    public string Name { get; internal set; }
    
    [Translation(nameof(UISkillStudyDesc))]
    public string Description { get; internal set; }
    
    [Translation(nameof(UISkillToolTip))]
    public string TooltipTitle { get; internal set; }
    
    [Translation(nameof(UISkillToolTipDesc))]
    public string TooltipDescription { get; internal set; }
    
    public byte Service
    {
        get => _service;
        set => _service = value;
    }

    public int GroupID
    {
        get => _groupID;
        set => _groupID = value;
    }

    public string BasicCode
    {
        get => _basicCode;
        set => _basicCode = value;
    }

    public string BasicName
    {
        get => _basicName;
        set => _basicName = value;
    }

    public string CodeName
    {
        get => _codeName;
        set => _codeName = value;
    }

    public string BasicGroup
    {
        get => _basicGroup;
        set => _basicGroup = value;
    }

    public int BasicOriginal
    {
        get => _basicOriginal;
        set => _basicOriginal = value;
    }

    public byte BasicLevel
    {
        get => _basicLevel;
        set => _basicLevel = value;
    }

    public byte BasicActivity
    {
        get => _basicActivity;
        set => _basicActivity = value;
    }

    public uint ChainCode
    {
        get => _chainCode;
        set => _chainCode = value;
    }

    public int BasicRecycleCost
    {
        get => _basicRecycleCost;
        set => _basicRecycleCost = value;
    }

    public int ActionPreparingTime
    {
        get => _actionPreparingTime;
        set => _actionPreparingTime = value;
    }

    public int ActionCastingTime
    {
        get => _actionCastingTime;
        set => _actionCastingTime = value;
    }

    public int ActionDuration
    {
        get => _actionDuration;
        set => _actionDuration = value;
    }

    public int ActionReuseDelay
    {
        get => _actionReuseDelay;
        set => _actionReuseDelay = value;
    }

    public int ActionCoolTime
    {
        get => _actionCoolTime;
        set => _actionCoolTime = value;
    }

    public int ActionFlyingSpeed
    {
        get => _actionFlyingSpeed;
        set => _actionFlyingSpeed = value;
    }

    public byte ActionInterruptable
    {
        get => _actionInterruptable;
        set => _actionInterruptable = value;
    }

    public int ActionOverlap
    {
        get => _actionOverlap;
        set => _actionOverlap = value;
    }

    public int ActionAutoAttackType
    {
        get => _actionAutoAttackType;
        set => _actionAutoAttackType = value;
    }

    public int ActionInTown
    {
        get => _actionInTown;
        set => _actionInTown = value;
    }

    public short ActionRange
    {
        get => _actionRange;
        set => _actionRange = value;
    }

    public bool TargetRequired
    {
        get => _targetRequired;
        set => _targetRequired = value;
    }

    public bool TargetTypeAnimal
    {
        get => _targetTypeAnimal;
        set => _targetTypeAnimal = value;
    }

    public bool TargetTypeLand
    {
        get => _targetTypeLand;
        set => _targetTypeLand = value;
    }

    public bool TargetTypeBuilding
    {
        get => _targetTypeBuilding;
        set => _targetTypeBuilding = value;
    }

    public bool TargetGroupSelf
    {
        get => _targetGroupSelf;
        set => _targetGroupSelf = value;
    }

    public bool TargetGroupAlly
    {
        get => _targetGroupAlly;
        set => _targetGroupAlly = value;
    }

    public bool TargetGroupParty
    {
        get => _targetGroupParty;
        set => _targetGroupParty = value;
    }

    public bool TargetGroupEnemyMonster
    {
        get => _targetGroupEnemyMonster;
        set => _targetGroupEnemyMonster = value;
    }

    public bool TargetGroupEnemyPlayer
    {
        get => _targetGroupEnemyPlayer;
        set => _targetGroupEnemyPlayer = value;
    }

    public bool TargetGroupNeutral
    {
        get => _targetGroupNeutral;
        set => _targetGroupNeutral = value;
    }

    public bool TargetGroupDontCare
    {
        get => _targetGroupDontCare;
        set => _targetGroupDontCare = value;
    }

    public bool TargetEtcSelectDeadBody
    {
        get => _targetEtcSelectDeadBody;
        set => _targetEtcSelectDeadBody = value;
    }

    public int RequiredMastery1
    {
        get => _requiredMastery1;
        set => _requiredMastery1 = value;
    }

    public int RequiredMastery2
    {
        get => _requiredMastery2;
        set => _requiredMastery2 = value;
    }

    public byte RequiredMasteryLevel1
    {
        get => _requiredMasteryLevel1;
        set => _requiredMasteryLevel1 = value;
    }

    public byte RequiredMasteryLevel2
    {
        get => _requiredMasteryLevel2;
        set => _requiredMasteryLevel2 = value;
    }

    public short RequiredStrength
    {
        get => _requiredStrength;
        set => _requiredStrength = value;
    }

    public short RequiredIntelligence
    {
        get => _requiredIntelligence;
        set => _requiredIntelligence = value;
    }

    public int RequiredLearnSkill1
    {
        get => _requiredLearnSkill1;
        set => _requiredLearnSkill1 = value;
    }

    public int RequiredLearnSkill2
    {
        get => _requiredLearnSkill2;
        set => _requiredLearnSkill2 = value;
    }

    public int RequiredLearnSkill3
    {
        get => _requiredLearnSkill3;
        set => _requiredLearnSkill3 = value;
    }

    public byte RequiredLearnSkillLevel1
    {
        get => _requiredLearnSkillLevel1;
        set => _requiredLearnSkillLevel1 = value;
    }

    public byte RequiredLearnSkillLevel2
    {
        get => _requiredLearnSkillLevel2;
        set => _requiredLearnSkillLevel2 = value;
    }

    public byte RequiredLearnSkillLevel3
    {
        get => _requiredLearnSkillLevel3;
        set => _requiredLearnSkillLevel3 = value;
    }

    public int RequiredLearnSkillPoint
    {
        get => _requiredLearnSkillPoint;
        set => _requiredLearnSkillPoint = value;
    }

    public byte RequiredRace
    {
        get => _requiredRace;
        set => _requiredRace = value;
    }

    public byte RequiredRestriction1
    {
        get => _requiredRestriction1;
        set => _requiredRestriction1 = value;
    }

    public byte RequiredRestriction2
    {
        get => _requiredRestriction2;
        set => _requiredRestriction2 = value;
    }

    public WeaponType RequiredWeapon1
    {
        get => _requiredWeapon1;
        set => _requiredWeapon1 = value;
    }

    public WeaponType RequiredWeapon2
    {
        get => _requiredWeapon2;
        set => _requiredWeapon2 = value;
    }

    public short ConsumeHealth
    {
        get => _consumeHealth;
        set => _consumeHealth = value;
    }

    public short ConsumeMana
    {
        get => _consumeMana;
        set => _consumeMana = value;
    }

    public short ConsumeHealthRatio
    {
        get => _consumeHealthRatio;
        set => _consumeHealthRatio = value;
    }

    public short ConsumeManaRatio
    {
        get => _consumeManaRatio;
        set => _consumeManaRatio = value;
    }

    public byte ConsumeHwan
    {
        get => _consumeHwan;
        set => _consumeHwan = value;
    }

    public byte UISkillTab
    {
        get => _uiSkillTab;
        set => _uiSkillTab = value;
    }

    public byte UISkillPage
    {
        get => _uiSkillPage;
        set => _uiSkillPage = value;
    }

    public byte UISkillColumn
    {
        get => _uiSkillColumn;
        set => _uiSkillColumn = value;
    }

    public byte UISkillRow
    {
        get => _uiSkillRow;
        set => _uiSkillRow = value;
    }

    public string UIIconFile
    {
        get => _uiIconFile;
        set => _uiIconFile = value;
    }

    public string UISkillName
    {
        get => _uiSkillName;
        set => _uiSkillName = value;
    }

    public string UISkillToolTip
    {
        get => _uiSkillToolTip;
        set => _uiSkillToolTip = value;
    }

    public string UISkillToolTipDesc
    {
        get => _uiSkillToolTipDesc;
        set => _uiSkillToolTipDesc = value;
    }

    public string UISkillStudyDesc
    {
        get => _uiSkillStudyDesc;
        set => _uiSkillStudyDesc = value;
    }

    public short AIAttackChance
    {
        get => _aiAttackChance;
        set => _aiAttackChance = value;
    }

    public byte AISkillType
    {
        get => _aiSkillType;
        set => _aiSkillType = value;
    }

    public List<int> Params
    {
        get => _params;
        set => _params = value;
    }
    #endregion Fields

    public override bool Parse(EntityParser parser)
    {
        //Skip disabled
        if (!parser.TryParse(0, out _service) || Service == 0)
            return false;

        //Skip invalid ID (PK)
        if (!parser.TryParse(1, out _id))
            return false;

        //Skip invalid group (MSKILL, HSKILL, TSKILL, GSKILL, PSKILL, P2SKILL) to save memory
        if (!parser.TryParse(2, out _groupID) /*|| GroupID == 0*/)
            return false;

        parser.TryParse(3, out _basicCode);
        parser.TryParse(4, out _basicName);
        parser.TryParse(5, out _basicGroup);
        parser.TryParse(6, out _basicOriginal);
        parser.TryParse(7, out _basicLevel);
        parser.TryParse(8, out _basicActivity);
        parser.TryParse(9, out _chainCode);
        parser.TryParse(10, out _basicRecycleCost);
        parser.TryParse(11, out _actionPreparingTime);
        parser.TryParse(12, out _actionCastingTime);
        parser.TryParse(13, out _actionDuration);
        parser.TryParse(14, out _actionReuseDelay);
        parser.TryParse(15, out _actionCoolTime);
        parser.TryParse(16, out _actionFlyingSpeed);
        parser.TryParse(17, out _actionInterruptable);
        parser.TryParse(18, out _actionOverlap);
        parser.TryParse(19, out _actionAutoAttackType);
        parser.TryParse(20, out _actionInTown);
        parser.TryParse(21, out _actionRange);
        parser.TryParse(22, out _targetRequired);
        parser.TryParse(23, out _targetTypeAnimal);
        parser.TryParse(24, out _targetTypeLand);
        parser.TryParse(25, out _targetTypeBuilding);
        parser.TryParse(26, out _targetGroupSelf);
        parser.TryParse(27, out _targetGroupAlly);
        parser.TryParse(28, out _targetGroupParty);
        parser.TryParse(29, out _targetGroupEnemyMonster);
        parser.TryParse(30, out _targetGroupEnemyPlayer);
        parser.TryParse(31, out _targetGroupNeutral);
        parser.TryParse(32, out _targetGroupDontCare);
        parser.TryParse(33, out _targetEtcSelectDeadBody);
        parser.TryParse(34, out _requiredMastery1);
        parser.TryParse(35, out _requiredMastery2);
        parser.TryParse(36, out _requiredMasteryLevel1);
        parser.TryParse(37, out _requiredMasteryLevel2);
        parser.TryParse(38, out _requiredStrength);
        parser.TryParse(39, out _requiredIntelligence);
        parser.TryParse(40, out _requiredLearnSkill1);
        parser.TryParse(41, out _requiredLearnSkill2);
        parser.TryParse(42, out _requiredLearnSkill3);
        parser.TryParse(43, out _requiredLearnSkillLevel1);
        parser.TryParse(44, out _requiredLearnSkillLevel2);
        parser.TryParse(45, out _requiredLearnSkillLevel3);
        parser.TryParse(46, out _requiredLearnSkillPoint);
        parser.TryParse(47, out _requiredRace);
        parser.TryParse(48, out _requiredRestriction1);
        parser.TryParse(49, out _requiredRestriction2);
        parser.TryParse(50, out _requiredWeapon1);
        parser.TryParse(51, out _requiredWeapon2);
        parser.TryParse(52, out _consumeHealth);
        parser.TryParse(53, out _consumeMana);
        parser.TryParse(54, out _consumeHealthRatio);
        parser.TryParse(55, out _consumeManaRatio);
        parser.TryParse(56, out _consumeHwan);
        parser.TryParse(57, out _uiSkillTab);
        parser.TryParse(58, out _uiSkillPage);
        parser.TryParse(59, out _uiSkillColumn);
        parser.TryParse(60, out _uiSkillRow);
        parser.TryParse(61, out _uiIconFile);
        parser.TryParse(62, out _uiSkillName);
        parser.TryParse(63, out _uiSkillToolTip);
        parser.TryParse(64, out _uiSkillToolTipDesc);
        parser.TryParse(65, out _uiSkillStudyDesc);
        parser.TryParse(66, out _aiAttackChance);
        parser.TryParse(67, out _aiSkillType);

        for (var i = 0; i < PARAM_COUNT; i++)
            if (parser.TryParse(68 + i, out int paramValue))
                _params.Add(paramValue);

        return true;
    }
}

//Params:
//6386804 = att
//  Param1: Type
//          Types:
//             5 = Phy. atk. pwr.
//             6 =
//  Param2: Physical percentage
//  Param3: Min
//  Param4: Max
//  Param5: Magical percentage?

//1734702198 = getv -> "getVariable"
//  Param1: Name

//1936028790 = setv -> "setVariable"
//  Param1: Name
//  Param2: Value
//  Param3: Value2

//Variables:
//1296122196 = MAAT -> ""
//1160860481 = E1SA -> ""
//1380992085 = RPDU -> "Rouge poison damage up"
//1380996181 = RPTU -> "Rouge poison time up
//to be continued...