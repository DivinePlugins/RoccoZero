namespace BeAware.ShowMeMore.MoreInformation;

using System.Threading.Tasks;

using BeAware.MenuManager.ShowMeMore;
using BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Numerics;

internal sealed class LeshracSplitEarth : Base
{
    private readonly LeshracSplitEarthMenu LeshracSplitEarthMenu;

    public LeshracSplitEarth(Common common) : base(common)
    {
        LeshracSplitEarthMenu = MoreInformationMenu.LeshracSplitEarthMenu;
    }

    private Color Color
    {
        get
        {
            return new Color((int)LeshracSplitEarthMenu.RedItem.Value, (int)LeshracSplitEarthMenu.GreenItem.Value, (int)LeshracSplitEarthMenu.BlueItem.Value);
        }
    }

    public override bool Entity(Unit unit, Hero hero)
    {
        if (hero.Id != HeroId.npc_dota_hero_leshrac)
        {
            return false;
        }

        if (!LeshracSplitEarthMenu.EnableItem)
        {
            return true;
        }

        if (hero.IsAlly(LocalHero))
        {
            return true;
        }

        var position = unit.Position;
        SplitEarth(unit, hero, position);

        if (LeshracSplitEarthMenu.WhenIsVisibleItem || !hero.IsVisible)
        {
            var pos = Pos(position, LeshracSplitEarthMenu.OnWorldItem);
            var minimapPos = MinimapPos(position, LeshracSplitEarthMenu.OnMinimapItem);
            Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_leshrac", AbilityId.leshrac_split_earth, 0, LeshracSplitEarthMenu.SideMessageItem, LeshracSplitEarthMenu.SoundItem);
        }

        return true;
    }

    private async void SplitEarth(Unit unit, Hero hero, Vector3 position)
    {
        var id = unit.Handle.ToString();
        DrawRange(id, position, hero.GetAbilityById(AbilityId.leshrac_split_earth).GetAbilitySpecialData("radius"), Color, 170); //TODO

        while (unit.IsValid)
        {
            await Task.Delay(150);
        }

        DrawRangeRemove(id);
    }
}