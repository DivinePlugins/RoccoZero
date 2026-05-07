namespace BeAware.ShowMeMore.MoreInformation;

using System.Threading.Tasks;

using BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Units;
using Divine.Modifier.Modifiers;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Update;

internal sealed class BloodseekerRupture : Base
{
    private readonly BloodseekerRuptureMenu BloodseekerRuptureMenu;

    private readonly BoolEventArgs ColorScreen; // TODO

    public BloodseekerRupture(Common common) : base(common)
    {
        BloodseekerRuptureMenu = MoreInformationMenu.BloodseekerRuptureMenu;

        ColorScreen = new BoolEventArgs();
        ColorScreen.ValueChanged += ColorScreenChanged;
    }

    private void ColorScreenChanged(object sender, bool e)
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
        var color = new Color((int)BloodseekerRuptureMenu.RedItem, (int)BloodseekerRuptureMenu.GreenItem, (int)BloodseekerRuptureMenu.BlueItem, (int)BloodseekerRuptureMenu.AlphaItem);

        var screenSize = RendererManager.ScreenSize;
        RendererManager.DrawFilledRectangle(new RectangleF(0, 0, screenSize.X, screenSize.Y), Color.Zero, color, 0);
    }

    public override bool Modifier(Unit unit, Modifier modifier)
    {
        if (!BloodseekerRuptureMenu.EnableItem || modifier.Name != "modifier_bloodseeker_rupture")
        {
            return false;
        }

        Rupture(unit, modifier);
        return true;
    }

    private async void Rupture(Unit unit, Modifier modifier)
    {
        if (unit.Team != LocalHero.Team)
        {
            return;
        }

        if (LocalHero.Handle != unit.Handle)
        {
            return;
        }

        ColorScreen.IsEnable = true;

        if (BloodseekerRuptureMenu.AutoStopItem)
        {
            LocalHero.Stop();

            UpdateManager.BeginInvoke(300, () => LocalHero.Stop());
        }

        while (modifier.IsValid)
        {
            await Task.Delay(50);
        }

        ColorScreen.IsEnable = false;
    }
}