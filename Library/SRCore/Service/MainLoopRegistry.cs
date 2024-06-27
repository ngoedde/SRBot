namespace SRCore.Service
{
    public class MainLoopRegistry
    {
        private List<Func<long, Task>> _actions = new();

        public void Register(Func<long, Task> action)
        {
            _actions.Add(action);
        }

        public async Task Run(long delta)
        {
            foreach (var action in _actions)
            {
                await action(delta);
            }
        }
    }
}