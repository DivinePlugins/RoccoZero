using System;
using System.Linq;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;

namespace BAIO.UnitManager
{
    public class UnitManager : IEquatable<UnitManager>
    {
        public readonly BaseHero Main;
        public Unit Unit;
        public uint Handle;

        public Ability Ability { get; set; }
        public Ability Ability2 { get; set; }
        public Ability Ability3 { get; set; }
        public Ability Ability4 { get; set; }

        public UnitManager(BaseHero main, Unit unit)
        {
            Main = main;
            Unit = unit;
            Handle = unit.Handle;
            Ability = unit.Spellbook.Spells.Count(x => x.Name != "lone_druid_spirit_bear_return" && !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive)) > 0 ? unit.Spellbook.Spell1 : null;
            Ability2 = unit.Spellbook.Spells.Count(x => !x.Name.StartsWith("special_") && !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive)) > 1 ? unit.Spellbook.Spell2 : null;
            Ability3 = unit.Spellbook.Spells.Count(x => !x.Name.StartsWith("special_") && !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive)) > 2 ? unit.Spellbook.Spell3 : null;
            Ability4 = unit.Spellbook.Spells.Count(x => !x.Name.StartsWith("special_") && !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive)) > 3 ? unit.Spellbook.Spell6 : null;
        }

        public static bool operator ==(UnitManager left, UnitManager right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UnitManager left, UnitManager right)
        {
            return !Equals(left, right);
        }

        public bool Equals(UnitManager entity)
        {
            if (entity is null)
            {
                return false;
            }

            if (ReferenceEquals(this, entity))
            {
                return true;
            }

            return Handle == entity.Handle;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnitManager);
        }

        public override int GetHashCode()
        {
            return (int)Handle;
        }
    }
}
