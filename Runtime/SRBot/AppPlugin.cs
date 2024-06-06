using System.Threading.Tasks;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using SRCore;

namespace SRBot;

public abstract class AppPlugin
{
    public string TechnicalName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public MaterialIconKind Icon { get; init; } = MaterialIconKind.ErrorOutline;

    public virtual void Shutdown(Kernel kernel)
    {
        
    }
    
    public virtual void Initialize(Kernel kernel)
    {
    }
    
    public virtual void BuildServices(IServiceCollection services)
    {
    }
}