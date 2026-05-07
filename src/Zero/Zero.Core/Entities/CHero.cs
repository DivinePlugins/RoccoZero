namespace Divine.Core.Entities;

using Divine.Core.Entities.Utilities;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Players;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;

public class CHero : CUnit
{
    internal CHero(Hero hero)
        : base(hero)
    {
        Base = hero;

        PrimaryAttribute = hero.PrimaryAttribute;
        IsIllusion = hero.IsIllusion;
        HeroId = hero.HeroId;

        var player = Base.Player;
        if (player != null && player.IsValid)
        {
            Player = player;
            PlayerId = Player.Id;
            TopPanel = new TopPanel(this);  //TODO HudManager
        }

        CachedInfo = new CachedInfo(this);
    }

    public new Hero Base { get; }

    public int PlayerId { get; private set; }

    public TopPanel TopPanel { get; private set; }

    public uint CurrentXP
    {
        get
        {
            return Base.CurrentXP;
        }
    }

    public uint AbilityPoints
    {
        get
        {
            return Base.AbilityPoints;
        }
    }

    public float RespawnTime
    {
        get
        {
            return Base.RespawnTime;
        }
    }

    public float RespawnTimePenalty
    {
        get
        {
            return Base.RespawnTimePenalty;
        }
    }

    public float Strength
    {
        get
        {
            return Base.Strength;
        }
    }

    public float Agility
    {
        get
        {
            return Base.Agility;
        }
    }

    public float Intelligence
    {
        get
        {
            return Base.Intelligence;
        }
    }

    public float TotalStrength
    {
        get
        {
            return Base.TotalStrength;
        }
    }

    public float TotalAgility
    {
        get
        {
            return Base.TotalAgility;
        }
    }

    public float TotalIntelligence
    {
        get
        {
            return Base.TotalIntelligence;
        }
    }

    public int RecentDamage
    {
        get
        {
            return Base.RecentDamage;
        }
    }

    public Player Player { get; private set; }

    public Hero ReplicateFrom
    {
        get
        {
            return Base.ReplicateFrom;
        }
    }

    public bool IsReincarnating
    {
        get
        {
            return Base.IsReincarnating;
        }
    }

    public float SpawnedAt
    {
        get
        {
            return Base.SpawnedAt;
        }
    }

    public Attribute PrimaryAttribute { get; }

    public bool IsBuybackDisabled
    {
        get
        {
            return Base.IsBuybackDisabled;
        }
    }

    public override bool IsIllusion { get; }

    public HeroId HeroId { get; }

    public float LastHurtTime
    {
        get
        {
            return Base.LastHurtTime;
        }
    }

    //public string MinimapIcon { get; }

    //public float MinimapIconSize { get; }

    public static implicit operator Hero(CHero hero)
    {
        return hero.Base;
    }

    public static explicit operator CHero(Entity entity)
    {
        return (CHero)UnitManager.GetUnitByEntity(entity);
    }
}