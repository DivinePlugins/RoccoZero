using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Divine.Core.Managers.Orbwalker.Modes;

namespace Divine.Core.Managers.Orbwalker
{
    internal class OrbwalkingMode
    {
        public Dictionary<string, IMode> Modes { get; set; } = new Dictionary<string, IMode>();

        public OrbwalkingMode()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(e => !e.IsAbstract && typeof(IMode).IsAssignableFrom(e)))
            {
                var mode = (IMode)Activator.CreateInstance(type);
                Modes[mode.Name] = mode;
            }
        }
    }
}
