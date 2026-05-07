namespace BeAware.ShowMeMore.MoreInformation;

using System.Text;

using BeAware.Helpers;
using BeAware.MenuManager;
using BeAware.MenuManager.ShowMeMore;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.GameConsole;
using Divine.Helpers;
using Divine.Modifier.Modifiers;
using Divine.Numerics;
using Divine.Particle;
using Divine.Particle.Components;
using Divine.Particle.Particles;

internal abstract class Base
{
    protected readonly MenuConfig MenuConfig;

    protected readonly MoreInformationMenu MoreInformationMenu;

    protected readonly Verification Verification;

    protected readonly Hero LocalHero = EntityManager.LocalHero;

    public Base(Common common)
    {
        MenuConfig = common.MenuConfig;
        MoreInformationMenu = MenuConfig.ShowMeMoreMenu.MoreInformationMenu;

        Verification = common.Verification;
    }

    public virtual bool Modifier(Unit unit, Modifier modifier)
    {
        return false;
    }

    public virtual bool Modifier(Unit unit, Modifier modifier, bool isHero)
    {
        return false;
    }

    public virtual bool Entity(Unit unit)
    {
        return false;
    }

    public virtual bool Entity(Unit unit, Hero hero)
    {
        return false;
    }

    public virtual bool Particle(Particle particle, string name)
    {
        return false;
    }

    protected Vector3 Pos(Vector3 position, bool menu)
    {
        return menu ? position : Vector3.Zero;
    }

    protected Vector2 MinimapPos(Vector3 position, bool menu)
    {
        return menu ? position.WorldToMinimap() : Vector2.Zero;
    }

    protected void DrawRange(string id, Vector3 position, float radius, Color color, int alpha)
    {
        ParticleManager.CreateParticle(
            $"DrawRange_{id}",
            @"materials\ensage_ui\particles\alert_range.vpcf",
            Attachment.CustomOrigin,
            new ControlPoint(0, position),
            new ControlPoint(1, color),
            new ControlPoint(2, radius, 255, alpha));
    }

    protected void DrawRangeRemove(string id)
    {
        ParticleManager.DestroyParticle($"DrawRange_{id}");
    }

    protected void DrawLine(string id, Vector3 startPosition, Vector3 endPosition, int size, int alpha, Color color)
    {
        ParticleManager.CreateParticle(
            $"DrawLine_{id}",
            @"materials\ensage_ui\particles\rectangle.vpcf",
            Attachment.CustomOrigin,
            new ControlPoint(1, startPosition),
            new ControlPoint(2, endPosition),
            new ControlPoint(3, size, alpha, 0),
            new ControlPoint(4, color));
    }

    protected void DrawLineRemove(string id)
    {
        ParticleManager.DestroyParticle($"DrawLine_{id}");
    }

    protected void DisplayMessage(string message, bool encodingDefault = false)
    {
        if (encodingDefault)
        {
            GameConsoleManager.ExecuteCommand(Encoding.Default.GetString(Encoding.UTF8.GetBytes(message)));
            return;
        }

        GameConsoleManager.ExecuteCommand(message);
    }
}