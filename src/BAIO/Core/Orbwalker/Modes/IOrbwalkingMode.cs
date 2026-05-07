namespace Ensage.SDK.Orbwalker.Modes
{
    public interface IOrbwalkingMode
    {
        bool CanExecute { get; }

        void Execute();

        void Activate();

        void Deactivate();
    }
}