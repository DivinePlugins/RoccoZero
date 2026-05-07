using System.Threading;
using System.Threading.Tasks;

using Divine.Core.Handlers;

namespace Divine.Core.ComboFactory
{
    public abstract class BaseTaskHandler : BaseUtils
    {
        private TaskHandler TaskHandler;

        private bool isRunning;

        public void RunAsync(bool restart = true)
        {
            if (isRunning && restart)
            {
                return;
            }

            if (TaskHandler == null)
            {
                TaskHandler = TaskHandler.Run(ExecuteAsync, restart, false);
            }

            TaskHandler.RunAsync();
            isRunning = true;
        }

        public void Cancel()
        {
            if (!isRunning)
            {
                return;
            }

            TaskHandler?.Cancel();
            isRunning = false;
        }

        public virtual void Dispose()
        {
            Cancel();
        }

        protected abstract Task ExecuteAsync(CancellationToken token);
    }
}
