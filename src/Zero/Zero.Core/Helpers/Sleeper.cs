using System.Threading.Tasks;

using Divine.Game;

namespace Divine.Core.Helpers
{
    public sealed class Sleeper
    {
        private bool isSleeping;

        private float sleepTime;

        public bool Sleeping
        {
            get
            {
                if (!isSleeping)
                {
                    return false;
                }

                isSleeping = GameManager.RawGameTime * 1000 < sleepTime;
                return isSleeping;
            }
        }

        public void Reset()
        {
            sleepTime = 0;
            isSleeping = false;
        }

        public void Sleep(float milliseconds)
        {
            sleepTime = GameManager.RawGameTime * 1000 + milliseconds;
            isSleeping = true;
        }

        private bool isDelay;

        public async void DelaySleep(int delay, float milliseconds)
        {
            if (isDelay)
            {
                return;
            }

            isDelay = true;

            await Task.Delay(delay);

            sleepTime = GameManager.RawGameTime * 1000 + milliseconds;
            isSleeping = true;
            isDelay = false;
        }
    }
}
