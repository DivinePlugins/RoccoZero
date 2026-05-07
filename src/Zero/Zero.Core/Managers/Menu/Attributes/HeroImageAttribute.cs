namespace Ensage.SDK.Menu.Attributes
{
    using System;

    using Divine.Entity.Entities.Units.Heroes.Components;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class HeroImageAttribute : ImageAttribute
    {
        public HeroImageAttribute(HeroId heroId)
        {
            HeroId = heroId;
        }

        public HeroId HeroId { get; }
    }
}