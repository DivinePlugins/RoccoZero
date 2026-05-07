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

internal sealed class KunkkaTorrent : Base
{
    private readonly KunkkaTorrentMenu KunkkaTorrentMenu;

    public KunkkaTorrent(Common common) : base(common)
    {
        KunkkaTorrentMenu = MoreInformationMenu.KunkkaTorrentMenu;
    }

    private Color Color
    {
        get
        {
            return new Color((int)KunkkaTorrentMenu.RedItem.Value, (int)KunkkaTorrentMenu.GreenItem.Value, (int)KunkkaTorrentMenu.BlueItem.Value);
        }
    }

    public override bool Entity(Unit unit, Hero hero)
    {
        if (hero.Id != HeroId.npc_dota_hero_kunkka)
        {
            return false;
        }

        if (!KunkkaTorrentMenu.EnableItem)
        {
            return true;
        }

        if (hero.IsAlly(LocalHero))
        {
            return true;
        }

        var position = unit.Position;
        Torrent(unit, hero, position);

        if (KunkkaTorrentMenu.WhenIsVisibleItem || !hero.IsVisible)
        {
            var pos = Pos(position, KunkkaTorrentMenu.OnWorldItem);
            var minimapPos = MinimapPos(position, KunkkaTorrentMenu.OnMinimapItem);
            Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_kunkka", AbilityId.kunkka_torrent, 0, KunkkaTorrentMenu.SideMessageItem, KunkkaTorrentMenu.SoundItem);
        }

        return true;
    }

    private async void Torrent(Unit unit, Hero hero, Vector3 position)
    {
        var id = unit.Handle.ToString();
        DrawRange(id, position, hero.GetAbilityById(AbilityId.kunkka_torrent).GetAbilitySpecialDataWithTalent(hero, "radius"), Color, 170); //TODO

        while (unit.IsValid)
        {
            await Task.Delay(150);
        }

        DrawRangeRemove(id);
    }
}