namespace BeAware.ShowMeMore.MoreInformation;

using System.Linq;
using System.Threading.Tasks;

using BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Numerics;
using Divine.Particle.Particles;
using Divine.Update;

internal sealed class WindrunnerPowershot : Base
{
    private readonly WindrunnerPowershotMenu WindrunnerPowershotMenu;

    public WindrunnerPowershot(Common common)
        : base(common)
    {
        WindrunnerPowershotMenu = MoreInformationMenu.WindrunnerPowershotMenu;
    }

    private Color Color
    {
        get
        {
            return new Color((int)WindrunnerPowershotMenu.RedItem.Value, (int)WindrunnerPowershotMenu.GreenItem.Value, (int)WindrunnerPowershotMenu.BlueItem.Value);
        }
    }

    public override bool Particle(Particle particle, string name)
    {
        if (!name.Contains("powershot_channel") && !name.Contains("windrunner_spell_powershot"))
        {
            return false;
        }

        if (!WindrunnerPowershotMenu.EnableItem)
        {
            return true;
        }

        var hero = EntityManager.GetEntities<Hero>().FirstOrDefault(x => !x.IsAlly(LocalHero) && !x.IsIllusion && x.Id == HeroId.npc_dota_hero_windrunner);
        if (hero == null)
        {
            return true;
        }

        var startPosition = hero.Position;

        Powershot(particle, startPosition, hero);

        if (!name.Contains("windrunner_spell_powershot") && (WindrunnerPowershotMenu.WhenIsVisibleItem || !hero.IsVisible))
        {
            var pos = Pos(startPosition, WindrunnerPowershotMenu.OnWorldItem);
            var minimapPos = MinimapPos(startPosition, WindrunnerPowershotMenu.OnMinimapItem);

            Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_windrunner", AbilityId.windrunner_powershot, 0, WindrunnerPowershotMenu.SideMessageItem, WindrunnerPowershotMenu.SoundItem);
        }

        return true;
    }

    private async void Powershot(Particle particle, Vector3 startPosition, Hero hero)
    {
        if (!hero.IsVisible)
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (!particle.IsValid)
                {
                    return;
                }

                var position = particle.GetControlPoint(0);
                var endPosition = position.Extend(position + particle.GetControlPoint(1), 2600);

                DrawRange("PowershotStart", position, 125, Color, 180);
                DrawRange("PowershotEnd", endPosition, 125, Color, 180);

                DrawLine("Powershot", position, endPosition, 150, 210, Color);
            });
        }
        else
        {
            await Task.Delay(200);

            var endPosition = hero.InFront(2600);

            DrawRange("PowershotStart", startPosition, 125, Color, 180);
            DrawRange("PowershotEnd", endPosition, 125, Color, 180);

            DrawLine("Powershot", startPosition, endPosition, 150, 210, Color);
        }

        await Task.Delay(2000);

        DrawRangeRemove("PowershotRange");
        DrawRangeRemove("PowershotStart");
        DrawRangeRemove("PowershotEnd");

        DrawLineRemove("Powershot");
    }
}