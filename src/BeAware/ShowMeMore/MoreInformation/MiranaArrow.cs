namespace BeAware.ShowMeMore.MoreInformation;

using System.Linq;
using System.Threading.Tasks;

using BeAware.Helpers;
using BeAware.MenuManager.ShowMeMore;
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

internal sealed class MiranaArrow : Base
{
    private readonly MiranaArrowMenu MiranaArrowMenu;

    private Color Color
    {
        get
        {
            return new Color((int)MiranaArrowMenu.RedItem.Value, (int)MiranaArrowMenu.GreenItem.Value, (int)MiranaArrowMenu.BlueItem.Value);
        }
    }

    private Color LineColor
    {
        get
        {
            return new Color((int)MiranaArrowMenu.LineRedItem.Value, (int)MiranaArrowMenu.LineGreenItem.Value, (int)MiranaArrowMenu.LineBlueItem.Value);
        }
    }

    public MiranaArrow(Common common)
        : base(common)
    {
        MiranaArrowMenu = MoreInformationMenu.MiranaArrowMenu;
    }

    public override bool Particle(Particle particle, string name)
    {
        if (!name.Contains("mirana") || !name.Contains("spell_arrow"))
        {
            return false;
        }

        if (!MiranaArrowMenu.EnableItem)
        {
            return true;
        }

        var hero = EntityManager.GetEntities<Hero>().FirstOrDefault(x => !x.IsAlly(LocalHero) && !x.IsIllusion && x.Id == HeroId.npc_dota_hero_mirana);
        if (hero == null)
        {
            return true;
        }

        UpdateManager.BeginInvoke(() =>
        {
            var position = particle.GetControlPoint(0);

            Arrow(particle, position, hero.Handle.ToString());

            var pos = Pos(position, MiranaArrowMenu.OnWorldItem);
            var minimapPos = MinimapPos(position, MiranaArrowMenu.OnMinimapItem);
            Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_mirana", AbilityId.mirana_arrow, 0, MiranaArrowMenu.SideMessageItem, MiranaArrowMenu.SoundItem);

            if (MiranaArrowMenu.WriteOnChatItem)
            {
                DisplayMessage();
            }
        });



        return true;
    }

    private async void Arrow(Particle particle, Vector3 position, string id)
    {
        var endPosition = position.Extend(position + particle.GetControlPoint(1), 3000);

        var color = Color;
        var lineColor = LineColor;

        DrawRange($"ArrowStart_{id}", position, 100, lineColor, 170);
        DrawRange($"ArrowEnd_{id}", endPosition, 100, lineColor, 170);
        DrawLine($"Arrow_{id}", position, endPosition, 115, 185, lineColor);

        var rawGameTime = GameManager.RawGameTime;
        do
        {
            var distance = (GameManager.RawGameTime - rawGameTime) * 900;
            if (distance > 3000)
            {
                break;
            }

            DrawRange($"ArrowMove_{id}", position.Extend(endPosition, distance), 100, color, 170);
            await Task.Delay(20);
        }
        while (particle.IsValid);

        DrawRangeRemove($"ArrowStart_{id}");
        DrawRangeRemove($"ArrowEnd_{id}");
        DrawRangeRemove($"ArrowMove_{id}");
        DrawLineRemove($"Arrow_{id}");
    }

    private void DisplayMessage()
    {
        switch (MenuConfig.LanguageItem)
        {
            case "EN":
                {
                    DisplayMessage($"say_team Carefully, Mirana Arrow");
                }
                break;

            case "RU":
                {
                    DisplayMessage($"say_team Осторожно, Mirana Arrow", true);
                }
                break;
        }
    }
}