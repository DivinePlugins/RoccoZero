namespace BeAware.ShowMeMore.MoreInformation;

using System.Threading.Tasks;

using BeAware.MenuManager.ShowMeMore;

using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Helpers;
using Divine.Modifier.Modifiers;
using Divine.Numerics;
using Divine.Renderer;

internal sealed class PhantomAssassinBlur : Base
{
    private readonly BoolEventArgs OnMinimap;

    private Unit phantomAssassin;

    public PhantomAssassinBlur(Common common)
        : base(common)
    {
        RendererManager.LoadImage(HeroId.npc_dota_hero_phantom_assassin, UnitImageType.MiniUnit);

        OnMinimap = new BoolEventArgs();
        OnMinimap.ValueChanged += OnMinimapChanged;
    }

    private void OnMinimapChanged(object sender, bool e)
    {
        if (e)
        {
            RendererManager.Draw += OnRendererManagerDraw;
        }
        else
        {
            RendererManager.Draw -= OnRendererManagerDraw;
        }
    }

    private void OnRendererManagerDraw()
    {
        var pos = phantomAssassin.Position.WorldToMinimap();
        RendererManager.DrawImage(@"mini_heroes\npc_dota_hero_phantom_assassin.png", new RectangleF(pos.X - 11, pos.Y - 13, 24, 24));
    }

    public override bool Modifier(Unit unit, Modifier modifier)
    {
        if (modifier.Name != "modifier_phantom_assassin_blur_active")
        {
            return false;
        }

        if (!MoreInformationMenu.PhantomAssassinBlurItem)
        {
            return true;
        }

        Blur(unit, modifier);
        return true;
    }

    private async void Blur(Unit unit, Modifier modifier)
    {
        if (unit.Team == LocalHero.Team || unit.IsIllusion) // TODO Team
        {
            return;
        }

        phantomAssassin = unit;
        OnMinimap.IsEnable = true;

        while (modifier.IsValid)
        {
            await Task.Delay(150);
        }

        OnMinimap.IsEnable = false;
    }
}