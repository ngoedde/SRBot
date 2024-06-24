namespace SRCore.Service
{
    public class MainLoopRegistry
    {
        private List<Action<long>> _actions = new();

        public void Register(Action<long> action)
        {
            _actions.Add(action);
        }

        public void Run(long delta)
        {
            foreach (var action in _actions)
            {
                action(delta);
            }
        }
    }
}