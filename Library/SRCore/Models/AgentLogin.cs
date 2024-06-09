using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models
{
    internal class AgentLogin(IServiceProvider serviceProvider) : GameModel(serviceProvider)
    {
        [Reactive]
        public string Username { get; set; }

        [Reactive]
        public string Password { get; set; }

        [Reactive]
        public string AgentServerIp { get; set; }

        [Reactive]
        public ushort AgentServerPort { get; set; }

        [Reactive]
        public byte ContentId { get; set; }

        [Reactive]
        public ushort ShardId { get; set; }

        [Reactive]
        public uint Token { get; set; }

        public ushort LocalPort => _proxy.LocalPort;

        private readonly Proxy _proxy = serviceProvider.GetRequiredService<Proxy>();
    }

}
