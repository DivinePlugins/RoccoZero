namespace BAIO.Interfaces
{
    using System;

    using Divine.Entity.Entities.Units.Heroes.Components;

    using Attribute = System.Attribute;

    [AttributeUsage(AttributeTargets.Class)]
    public class ExportHeroAttribute : Attribute, IHeroMetadata
    {
        public ExportHeroAttribute(HeroId id)
        {
            this.Id = id;
        }

        public HeroId Id { get; }
    }
}