namespace BAIO.Core.Handlers;

using System;
using System.Threading;
using System.Threading.Tasks;

using Divine.Update;
using Divine.Zero.Log;

public class TaskHandler
{
    private bool isRunning;
    private static readonly TaskFactory Factory;

    public TaskHandler(Func<CancellationToken, Task> factory, bool restart = false)
    {
        this.TaskFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        this.Restart = restart;
    }

    static TaskHandler()
    {
        SynchronizationContext.SetSynchronizationContext(UpdateManager.SynchronizationContext);

        Factory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public static TaskHandler Run(Func<CancellationToken, Task> factory, bool restart = true, bool autostart = true)
    {
        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        var task = new TaskHandler(factory, restart);

        if (autostart)
        {
            task.RunAsync();
        }

        return task;
    }

    public bool IsRunning
    {
        get
        {
            return this.isRunning;
        }
    }

    public Task RunningTask { get; private set; }

    private bool Restart { get; }

    private Func<CancellationToken, Task> TaskFactory { get; }

    private CancellationTokenSource TokenSource { get; set; }

    public void Cancel(bool throwOnFirstException = false)
    {
        this.TokenSource?.Cancel(throwOnFirstException);
    }

    public void CancelAfter(TimeSpan delay)
    {
        this.TokenSource?.CancelAfter(delay);
    }

    public void CancelAfter(int millisecondsDelay)
    {
        this.TokenSource?.CancelAfter(millisecondsDelay);
    }

    public TaskHandler CreateCopy()
    {
        return new TaskHandler(this.TaskFactory, this.Restart);
    }

    public void RunAsync()
    {
        if (this.isRunning)
        {
            return;
        }

        this.isRunning = true;

        this.TokenSource = new CancellationTokenSource();
        this.RunningTask = Factory.StartNew(
            async () =>
            {
                try
                {
                    do
                    {
                        await this.TaskFactory(this.TokenSource.Token);
                        await Task.Delay(10, this.TokenSource.Token);
                    }
                    while (this.Restart && !this.TokenSource.IsCancellationRequested);
                }
                catch (TaskCanceledException)
                {
                    // canceled
                }
                catch (Exception e)
                {
                    LogManager.Error(e);
                }
                finally
                {
                    this.TokenSource = null;
                    this.RunningTask = null;

                    this.isRunning = false;
                }
            });
    }
}