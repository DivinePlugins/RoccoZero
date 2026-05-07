namespace Ensage.SDK.Orbwalker.Modes
{
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Core.Handlers;

    using Ensage.SDK.Service;

    public abstract class OrbwalkingModeAsync : OrbwalkingMode
    {
        protected OrbwalkingModeAsync(IServiceContext context)
            : base(context)
        {
        }

        protected TaskHandler Handler { get; set; }

        public override void Execute()
        {
            if (this.Handler == null)
            {
                this.Handler = TaskHandler.Run(this.ExecuteAsync, false, false);
            }

            this.Handler.RunAsync();
        }

        public abstract Task ExecuteAsync(CancellationToken token);

        protected void Cancel()
        {
            this.Handler?.Cancel();
        }
    }
}