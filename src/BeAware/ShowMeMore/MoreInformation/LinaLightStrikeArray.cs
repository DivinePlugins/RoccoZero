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

internal sealed class LinaLightStrikeArray : Base
{
    private readonly LinaLightStrikeArrayMenu LinaLightStrikeArrayMenu;

    public LinaLightStrikeArray(Common common) : base(common)
    {
        LinaLightStrikeArrayMenu = MoreInformationMenu.LinaLightStrikeArrayMenu;
    }

    private Color Color
    {
        get
        {
            return new Color((int)LinaLightStrikeArrayMenu.RedItem.Value, (int)LinaLightStrikeArrayMenu.GreenItem.Value, (int)LinaLightStrikeArrayMenu.BlueItem.Value);
        }
    }

    public override bool Entity(Unit unit, Hero hero)
    {
        if (hero.Id != HeroId.npc_dota_hero_lina)
        {
            return false;
        }

        if (!LinaLightStrikeArrayMenu.EnableItem)
        {
            return true;
        }

        if (hero.IsAlly(LocalHero))
        {
            return true;
        }

        var position = unit.Position;
        LightStrikeArray(unit, position);

        if (LinaLightStrikeArrayMenu.WhenIsVisibleItem || !hero.IsVisible)
        {
            var pos = Pos(position, LinaLightStrikeArrayMenu.OnWorldItem);
            var minimapPos = MinimapPos(position, LinaLightStrikeArrayMenu.OnMinimapItem);
            Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_lina", AbilityId.lina_light_strike_array, 0, LinaLightStrikeArrayMenu.SideMessageItem, LinaLightStrikeArrayMenu.SoundItem);
        }

        return true;
    }

    private async void LightStrikeArray(Unit unit, Vector3 position)
    {
        var id = unit.Handle.ToString();
        DrawRange(id, position, 225, Color, 170);

        while (unit.IsValid)
        {
            await Task.Delay(150);
        }

        DrawRangeRemove(id);
    }
}