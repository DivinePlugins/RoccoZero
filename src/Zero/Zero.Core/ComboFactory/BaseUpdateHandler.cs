using Divine.Update;

namespace Divine.Core.ComboFactory
{
    public abstract class BaseUpdateHandler : BaseUtils
    {
        private readonly int Timeout;

        public BaseUpdateHandler(int timeout = 0)
        {
            Timeout = timeout;
        }

        private UpdateHandler UpdateHandler;

        public virtual void Run()
        {
            if (UpdateHandler == null)
            {
                UpdateHandler = UpdateManager.CreateIngameUpdate(Timeout, Execute);
            }

            if (UpdateHandler.IsEnabled)
            {
                return;
            }

            UpdateHandler.IsEnabled = true;
        }

        public void Cancel()
        {
            if (!UpdateHandler.IsEnabled)
            {
                return;
            }

            UpdateHandler.IsEnabled = false;
        }

        public virtual void Dispose()
        {
            if (UpdateHandler == null)
            {
                return;
            }

            UpdateManager.DestroyIngameUpdate(Execute);
            UpdateHandler = null;
        }

        protected abstract void Execute();
    }
}
