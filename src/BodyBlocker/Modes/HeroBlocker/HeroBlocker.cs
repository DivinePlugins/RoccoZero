namespace BodyBlocker.Modes.HeroBlocker;

using System;
using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.GameConsole;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

internal class HeroBlocker : IBodyBlockMode
{
    private readonly Menu root;

    private Unit hero;

    private readonly HeroBlockerSettings settings;

    private UpdateHandler updateHandler;

    public HeroBlocker(Menu root)
    {
        this.root = root;
        settings = new HeroBlockerSettings(root);
    }

    public void Activate()
    {
        hero = EntityManager.LocalHero;

        settings.Key.ValueChanged += KeyPressed;
        updateHandler = UpdateManager.CreateGameUpdate(50, false, OnUpdate);
    }

    public void Deactivate()
    {
        settings.Key.ValueChanged -= KeyPressed;
        UpdateManager.DestroyIngameUpdate(updateHandler);
    }

    private void KeyPressed(MenuHoldKey sender, HoldKeyChangedEventArgs e)
    {
        updateHandler.IsEnabled = e.Value;

        if (!settings.CenterCamera)
        {
            return;
        }

        const string Command = "dota_camera_center_on_hero";
        GameConsoleManager.ExecuteCommand((updateHandler.IsEnabled ? "+" : "-") + Command);
    }

    private void OnUpdate()
    {
        if (!hero.IsAlive || GameManager.IsPaused)
        {
            return;
        }

        var blockTarget = EntityManager.GetEntities<Hero>().OrderBy(x => x.Distance2D(hero))
            .FirstOrDefault(
                x => x.IsValid && x.IsVisible && x.IsAlive && !x.IsIllusion && x.Distance2D(hero) < 1000
                     && x.IsEnemy(hero) && !x.UnitState.HasFlag(UnitState.NoCollision));

        if (blockTarget == null)
        {
            return;
        }

        var angle = blockTarget.FindRotationAngle(hero.Position);

        if (angle > 1.3)
        {
            var delta = angle * 0.6f;
            var position = blockTarget.Position;
            var side1 = position + blockTarget.Vector3FromPolarAngle(delta)
                        * Math.Max(settings.BlockSensitivity, 150);
            var side2 = position + blockTarget.Vector3FromPolarAngle(-delta)
                        * Math.Max(settings.BlockSensitivity, 150);

            hero.Move(side1.Distance(hero.Position) < side2.Distance(hero.Position) ? side1 : side2);
        }
        else
        {
            if (blockTarget.IsMoving && angle < 0.3 && hero.IsMoving)
            {
                hero.Stop();
                return;
            }

            hero.Move(blockTarget.InFront(settings.BlockSensitivity));
        }
    }
}