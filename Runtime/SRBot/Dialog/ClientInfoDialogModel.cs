using System.ComponentModel;
using ReactiveUI.Fody.Helpers;
using SRGame.Client;

namespace SRBot.Dialog;

public class ClientInfoDialogModel(ClientInfoManager clientInfoManager, EntityManager entityManager)
{
    [property: Category("DivisionInfo"), DisplayName("Division Info")]
    [Reactive] public DivisionInfo DivisionInfo => clientInfoManager.DivisionInfo;
    
    [property: Category("Version"), DisplayName("Division Info")]
    [Reactive] public uint Version => clientInfoManager.Version;
    
    [property: Category("Gateway"), DisplayName("Gateway port")]
    [Reactive] public ushort GatewayPort => clientInfoManager.GatewayPort;
    
    [property: Category("Entities"), DisplayName("Item Count")]
    [Reactive] public int ItemCount => entityManager.ItemRepository.Entities.Count;
    
    [property: Category("Entities"), DisplayName("Skill Count")]
    [Reactive] public int SkillCount => entityManager.SkillRepository.Entities.Count;
    
    [property: Category("Entities"), DisplayName("Translation Count")]
    [Reactive] public int TranslationCount => entityManager.TranslationRepository.Entities.Count;
    
    [property: Category("Entities"), DisplayName("Character Count")]
    [Reactive] public int CharacterCount => entityManager.CharacterRepository.Entities.Count;
    
    [property: Category("Entities"), DisplayName("Skill Mastery Count")]
    [Reactive] public int SkillMasteryCount => entityManager.SkillMasteryRepository.Entities.Count;
    
    [Reactive] public string Path => clientInfoManager.Path;
}