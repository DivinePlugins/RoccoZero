namespace Divine.Zeus;

using Divine.Core.ComboFactory.Combos;
using Divine.Core.ComboFactory.Commons;
using Divine.Core.ComboFactory.Helpers;

using Divine.Zeus.Combos;
using Divine.Zeus.Features;
using Divine.Zeus.Helpers;
using Divine.Zeus.Menus;

internal sealed class Common : BaseCommon
{
    public override BaseAbilities Abilities { get; } = new Abilities();

    public override BaseDamageCalculation DamageCalculation { get; }

    public override BaseLinkenBreaker LinkenBreaker { get; }

    public override BaseKillSteal KillSteal { get; }

    public override BaseCombo Combo { get; }

    public UpdateMode UpdateMode { get; }

    private AbilityBreaker AbilityBreaker { get; }

    private Farm Farm { get; }

    public Common(MenuConfig menuConfig)
        : base(menuConfig)
    {
        DamageCalculation = new DamageCalculation(this);

        LinkenBreaker = new LinkenBreaker(this);
        KillSteal = new KillSteal(this);
        Combo = new Combo(this);

        UpdateMode = new UpdateMode(this);

        AbilityBreaker = new AbilityBreaker(this);
        Farm = new Farm(this);
    }

    public override void Dispose()
    {
        base.Dispose();

        Farm.Dispose();
        AbilityBreaker.Dispose();

        UpdateMode.Dispose();
    }
}
