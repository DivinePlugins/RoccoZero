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

internal sealed class PudgeHook : Base
{
    private readonly PudgeHookMenu PudgeHookMenu;

    private bool finish;

    public PudgeHook(Common common) : base(common)
    {
        PudgeHookMenu = MoreInformationMenu.PudgeHookMenu;
    }

    private Color Color
    {
        get
        {
            return new Color((int)PudgeHookMenu.RedItem, (int)PudgeHookMenu.GreenItem, (int)PudgeHookMenu.BlueItem);
        }
    }

    private Color LineColor
    {
        get
        {
            return new Color((int)PudgeHookMenu.LineRedItem, (int)PudgeHookMenu.LineGreenItem, (int)PudgeHookMenu.LineBlueItem);
        }
    }

    public override bool Particle(Particle particle, string name)
    {
        if (!name.Contains("pudge_meathook"))
        {
            return false;
        }

        if (!PudgeHookMenu.EnableItem)
        {
            return true;
        }

        var hero = EntityManager.GetEntities<Hero>().FirstOrDefault(x => !x.IsAlly(LocalHero) && !x.IsIllusion && x.Id == HeroId.npc_dota_hero_pudge);
        if (hero == null)
        {
            return true;
        }

        UpdateManager.BeginInvoke(1, () =>
        {
            if (name.Contains("pudge_meathook_impact"))
            {
                finish = true;
                return;
            }

            var startPosition = particle.GetControlPoint(0);
            Hook(particle, startPosition);

            if (PudgeHookMenu.WhenIsVisibleItem || !hero.IsVisible)
            {
                var pos = Pos(startPosition, PudgeHookMenu.OnWorldItem);
                var minimapPos = MinimapPos(startPosition, PudgeHookMenu.OnMinimapItem);

                Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_pudge", AbilityId.pudge_meat_hook, 0, PudgeHookMenu.SideMessageItem, PudgeHookMenu.SoundItem);
            }
        });

        return true;
    }

    private async void Hook(Particle particle, Vector3 startPosition)
    {
        var endPosition = particle.GetControlPoint(1);
        var distance = startPosition.Distance(endPosition);

        var color = Color;
        var lineColor = LineColor;

        DrawRange("HookStart", startPosition, 100, lineColor, 180);
        DrawRange("HookEnd", endPosition, 100, lineColor, 180);
        DrawLine("Hook", startPosition, endPosition, 150, 185, lineColor);

        var rawGameTime = GameManager.RawGameTime;
        var isBack = false;
        var backTime = 0.0f;
        finish = false;
        do
        {
            var time = isBack ? backTime - GameManager.RawGameTime : GameManager.RawGameTime - rawGameTime;
            var hookPosition = startPosition.Extend(endPosition, time * 1450);

            if (startPosition.Distance(hookPosition) > distance)
            {
                isBack = true;
                backTime = GameManager.RawGameTime + time;
            }

            DrawRange("HookPosition", hookPosition, 100, color, 180);

            finish = time < 0;
            await Task.Delay(10);
        }
        while (particle.IsValid && !finish);

        DrawRangeRemove("HookStart");
        DrawRangeRemove("HookEnd");
        DrawLineRemove("Hook");

        DrawRangeRemove("HookPosition");
    }
}