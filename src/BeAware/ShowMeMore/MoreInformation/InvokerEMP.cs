namespace BeAware.ShowMeMore.MoreInformation;

using System.Linq;

using BeAware.MenuManager.ShowMeMore;
using BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Numerics;
using Divine.Particle.Particles;
using Divine.Update;

internal sealed class InvokerEMP : Base
{
    private readonly InvokerEMPMenu InvokerEMPMenu;

    public InvokerEMP(Common common) : base(common)
    {
        InvokerEMPMenu = MoreInformationMenu.InvokerEMPMenu;
    }

    private Color Color
    {
        get
        {
            return new Color((int)InvokerEMPMenu.RedItem.Value, (int)InvokerEMPMenu.GreenItem.Value, (int)InvokerEMPMenu.BlueItem.Value);
        }
    }

    public override bool Particle(Particle particle, string name)
    {
        if (!name.Contains("invoker_emp"))
        {
            return false;
        }

        if (!InvokerEMPMenu.EnableItem)
        {
            return true;
        }

        if (!EntityManager.GetEntities<Hero>().Any(x => !x.IsAlly(LocalHero) && !x.IsIllusion && x.Id == HeroId.npc_dota_hero_invoker))
        {
            return true;
        }

        var handle = particle.GetHashCode().ToString();
        UpdateManager.BeginInvoke(1, () =>
        {
            var position = particle.GetControlPoint(0);
            DrawRange(handle, position, 675, Color, 70);

            var pos = Pos(position, InvokerEMPMenu.OnWorldItem);
            var minimapPos = MinimapPos(position, InvokerEMPMenu.OnMinimapItem);
            Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_invoker", AbilityId.invoker_emp, 0, InvokerEMPMenu.SideMessageItem, InvokerEMPMenu.SoundItem);
        });

        UpdateManager.BeginInvoke(2900, () =>
        {
            DrawRangeRemove(handle);
        });

        return true;
    }
}