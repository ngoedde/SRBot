using System;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using SRCore.Models;

namespace SRBot.Page.World;

public class WorldPageModel : PageModel
{
    private readonly IServiceProvider _serviceProvider;
    public Player Player => _serviceProvider.GetRequiredService<Player>();
    public Spawn Spawn => _serviceProvider.GetRequiredService<Spawn>();

    public WorldPageModel(IServiceProvider serviceProvider) : base("srbot_page_world", "World", 3,
        MaterialIconKind.Map)
    {
        _serviceProvider = serviceProvider;
    }
}