namespace BeAware.ShowMeMore.MoreInformation;

using System.Linq;
using System.Threading.Tasks;

using BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Helpers;
using Divine.Numerics;

internal sealed class InvokerSunStrike : Base
{
    private readonly InvokerSunStrikeMenu InvokerSunStrikeMenu;

    public InvokerSunStrike(Common common) : base(common)
    {
        InvokerSunStrikeMenu = MoreInformationMenu.InvokerSunStrikeMenu;
    }

    private Color Color
    {
        get
        {
            return new Color((int)InvokerSunStrikeMenu.RedItem.Value, (int)InvokerSunStrikeMenu.GreenItem.Value, (int)InvokerSunStrikeMenu.BlueItem.Value);
        }
    }

    private readonly Sleeper Sleeper = new();

    public override bool Entity(Unit unit, Hero hero)
    {
        if (hero.Id != HeroId.npc_dota_hero_invoker)
        {
            return false;
        }

        if (!InvokerSunStrikeMenu.EnableItem)
        {
            return true;
        }

        if (hero.IsAlly(LocalHero))
        {
            return true;
        }

        var position = unit.Position;
        SunStrike(unit, position);

        var pos = Pos(position, InvokerSunStrikeMenu.OnWorldItem);
        var minimapPos = MinimapPos(position, InvokerSunStrikeMenu.OnMinimapItem);

        var sleeping = !Sleeper.IsSleeping;
        Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_invoker", AbilityId.invoker_sun_strike, 0, InvokerSunStrikeMenu.SideMessageItem && sleeping, InvokerSunStrikeMenu.SoundItem && sleeping);
        Sleeper.Sleep(250);

        if (InvokerSunStrikeMenu.WriteOnChatItem)
        {
            DisplayMessage(pos);
        }

        return true;
    }

    private async void SunStrike(Unit unit, Vector3 position)
    {
        DrawRange(position.ToString(), position, 175, Color, 170);

        while (unit.IsValid)
        {
            await Task.Delay(150);
        }

        DrawRangeRemove(position.ToString());
    }

    private void DisplayMessage(Vector3 position)
    {
        var unit = EntityManager.GetEntities<Hero>().Where(x => !x.IsIllusion && x.IsAlly(LocalHero) && x.Distance2D(position) < 800).OrderBy(x => x.Distance2D(position)).FirstOrDefault();
        if (unit == null)
        {
            return;
        }

        switch (MenuConfig.LanguageItem)
        {
            case "EN":
                {
                    DisplayMessage($"say_team Carefully, Sun strike on {unit.GetDisplayName()}");
                }
                break;

            case "RU":
                {
                    DisplayMessage($"say_team Осторожно, Sun strike на {unit.GetDisplayName()}", true);
                }
                break;
        }
    }
}