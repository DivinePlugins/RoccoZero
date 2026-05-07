using System;
using System.Collections.Generic;

namespace Divine.Core.Managers.Unit
{
    internal static class TypeHelper
    {
        private static readonly Dictionary<Type, IType> Types = new Dictionary<Type, IType>
        {
            { typeof(Ally), new Ally() },
            { typeof(Enemy), new Enemy() },
            { typeof(Neutral), new Neutral() },
            { typeof(Controllable), new Controllable() },
            { typeof(NoIllusion), new NoIllusion() }
        };

        public static IType GetType<T>()
        {
            if (typeof(T).Name == "IType")
            {
                throw new BannedTypeException("Please Do Not Use IType");
            }

            return Types[typeof(T)];
        }
    }
}