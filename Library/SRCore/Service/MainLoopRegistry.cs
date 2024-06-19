namespace SRCore.Service
{
    internal class MainLoopRegistry
    {
      private List<Action> _actions = new List<Action>();

      public void Register(Action action)
      {
            _actions.Add(action);
      }

      public void Run()
        {
            foreach (var action in _actions)
            {
                action();
            }
        }
    }
}
