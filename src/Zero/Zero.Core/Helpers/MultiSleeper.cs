using System.Collections.Generic;

namespace Divine.Core.Helpers
{
    public sealed class ZMultiSleeper
    {
        private readonly Dictionary<uint, ZSleeper> sleepers = new Dictionary<uint, ZSleeper>();

        public ZSleeper this[uint handle]
        {
            get
            {
                if (!sleepers.TryGetValue(handle, out var sleeper))
                {
                    sleeper = new ZSleeper();
                    sleepers[handle] = sleeper;
                }

                return sleeper;
            }
        }

        public void Reset(uint id)
        {
            this[id].Reset();
        }

        public void Sleep(uint id, float milliseconds)
        {
            this[id].Sleep(milliseconds);
        }

        public void DelaySleep(uint id, int delay, float milliseconds)
        {
            this[id].DelaySleep(delay, milliseconds);
        }

        public bool Sleeping(uint id)
        {
            return this[id].Sleeping;
        }
    }

    public static class ZMultiSleeper<T>
    {
        private static readonly Dictionary<T, ZSleeper> sleepers = new Dictionary<T, ZSleeper>();

        public static ZSleeper Sleeper(T handle)
        {
            if (!sleepers.TryGetValue(handle, out var sleeper))
            {
                sleeper = new ZSleeper();
                sleepers[handle] = sleeper;
            }

            return sleeper;
        }

        public static void Reset(T id)
        {
            Sleeper(id).Reset();
        }

        public static void Sleep(T id, float milliseconds)
        {
            Sleeper(id).Sleep(milliseconds);
        }

        public static void DelaySleep(T id, int delay, float milliseconds)
        {
            Sleeper(id).DelaySleep(delay, milliseconds);
        }

        public static bool Sleeping(T id)
        {
            return Sleeper(id).Sleeping;
        }
    }
}
