namespace BAIO.Core;

using System.Collections.Generic;

public class MultiSleeper
{
    public MultiSleeper()
    {
        this.LastSleepTickDictionary = new Dictionary<object, float>();
    }

    public Dictionary<object, float> LastSleepTickDictionary { get; set; }

    public void Reset(object id)
    {
        if (!this.LastSleepTickDictionary.ContainsKey(id))
        {
            return;
        }

        this.LastSleepTickDictionary[id] = 0;
    }

    public void Sleep(float duration, object id, bool extendCurrentSleep = false)
    {
        if (!this.LastSleepTickDictionary.ContainsKey(id))
        {
            this.LastSleepTickDictionary.Add(id, Utils.TickCount + duration);
            return;
        }

        if (extendCurrentSleep && this.LastSleepTickDictionary[id] > Utils.TickCount)
        {
            this.LastSleepTickDictionary[id] += duration;
            return;
        }

        this.LastSleepTickDictionary[id] = Utils.TickCount + duration;
    }

    public bool Sleeping(object id)
    {
        float lastSleepTick;
        return this.LastSleepTickDictionary.TryGetValue(id, out lastSleepTick) && Utils.TickCount < lastSleepTick;
    }
}