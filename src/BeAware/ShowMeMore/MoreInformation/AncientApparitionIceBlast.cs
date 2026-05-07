namespace BeAware.ShowMeMore.MoreInformation;

using System.Linq;
using System.Threading.Tasks;

using BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Particle.Particles;
using Divine.Update;

internal sealed class AncientApparitionIceBlast : Base
{
    private readonly AncientApparitionIceBlastMenu AncientApparitionIceBlastMenu;

    private bool finish;

    public AncientApparitionIceBlast(Common common) : base(common)
    {
        AncientApparitionIceBlastMenu = MoreInformationMenu.AncientApparitionIceBlastMenu;
    }

    private Color Color
    {
        get
        {
            return new Color((int)AncientApparitionIceBlastMenu.RedItem, (int)AncientApparitionIceBlastMenu.GreenItem, (int)AncientApparitionIceBlastMenu.BlueItem);
        }
    }

    private Color LineColor
    {
        get
        {
            return new Color((int)AncientApparitionIceBlastMenu.LineRedItem, (int)AncientApparitionIceBlastMenu.LineGreenItem, (int)AncientApparitionIceBlastMenu.LineBlueItem);
        }
    }

    public override bool Particle(Particle particle, string name)
    {
        if (name.Contains("ancient_apparition_ice_blast_explode"))
        {
            finish = true;
            return true;
        }

        if (!name.Contains("ancient_apparition_ice_blast_final"))
        {
            return false;
        }

        if (!AncientApparitionIceBlastMenu.EnableItem)
        {
            return true;
        }

        if (EntityManager.GetEntities<Hero>().Any(x => x.IsAlly(LocalHero) && !x.IsIllusion && x.Id == HeroId.npc_dota_hero_ancient_apparition))
        {
            return true;
        }

        UpdateManager.BeginInvoke(() =>
        {
            if (!particle.IsValid)
            {
                return;
            }

            var startPosition = particle.GetControlPoint(0);
            IceBlast(particle, startPosition);

            var pos = Pos(startPosition, AncientApparitionIceBlastMenu.OnWorldItem);
            var minimapPos = MinimapPos(startPosition, AncientApparitionIceBlastMenu.OnMinimapItem);

            Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_ancient_apparition", AbilityId.ancient_apparition_ice_blast, 0, AncientApparitionIceBlastMenu.SideMessageItem, AncientApparitionIceBlastMenu.SoundItem);
        });

        return true;
    }

    private async void IceBlast(Particle particle, Vector3 startPosition)
    {
        var rotation = particle.GetControlPoint(1);
        var endPosition = startPosition + (particle.GetControlPoint(5).X * rotation);
        var radius = startPosition.Distance(endPosition) / 20;

        var color = Color;
        var lineColor = LineColor;

        DrawRange("IceBlastStart", startPosition, 100, lineColor, 180);
        DrawRange("IceBlastEnd", endPosition, 275 + radius, color, 180);
        DrawLine("IceBlast", startPosition, endPosition, 150, 185, lineColor);

        var rawGameTime = GameManager.RawGameTime;
        var speed = startPosition.Distance(startPosition + rotation) * 1.04f;
        finish = false;
        do
        {
            var position = startPosition.Extend(endPosition, distance: (GameManager.RawGameTime - rawGameTime) * speed);
            DrawRange("IceBlastPosition", position, 100, color, 180);

            await Task.Delay(10);
        }
        while (!finish);

        DrawRangeRemove("IceBlastStart");
        DrawRangeRemove("IceBlastEnd");
        DrawLineRemove("IceBlast");
        DrawRangeRemove("IceBlastPosition");
    }
}