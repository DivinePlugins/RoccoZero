namespace BodyBlocker.Modes.ControllablesBlocker;

using System;
using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

internal class ControllablesBlocker : IBodyBlockMode
{
    private readonly Menu root;

    private Unit hero;

    private Hero blockTarget;

    private readonly ControllablesBlockerSettings settings;

    private UpdateHandler updateHandler;

    public ControllablesBlocker(Menu root)
    {
        this.root = root;
        settings = new ControllablesBlockerSettings(root);
    }

    public void Activate()
    {
        hero = EntityManager.LocalHero;

        settings.Key.ValueChanged += KeyPressed;

        updateHandler = UpdateManager.CreateGameUpdate(50, false, OnUpdate);
    }

    public void Deactivate()
    {
        settings.Key.ValueChanged += KeyPressed;
        UpdateManager.DestroyIngameUpdate(updateHandler);
    }

    private void KeyPressed(MenuHoldKey sender, HoldKeyChangedEventArgs e)
    {
        if (!e.Value)
        {
            return;
        }

        if (updateHandler.IsEnabled)
        {
            updateHandler.IsEnabled = false;
            return;
        }

        blockTarget = EntityManager.GetEntities<Hero>().OrderBy(x => x.Distance2D(GameManager.MousePosition))
            .FirstOrDefault(
                x => x.IsValid && x.IsVisible && !x.IsIllusion && x.IsAlive
                     && x.Distance2D(GameManager.MousePosition) < 1000 && x.IsEnemy(hero)
                     && !x.UnitState.HasFlag(UnitState.NoCollision));

        if (blockTarget != null)
        {
            updateHandler.IsEnabled = true;
        }
    }

    private void OnUpdate()
    {
        if (GameManager.IsPaused)
        {
            return;
        }

        if (!blockTarget.IsAlive || !blockTarget.IsVisible)
        {
            updateHandler.IsEnabled = false;
            return;
        }

        var controllables = EntityManager.GetEntities<Unit>().Where(
                x => x.IsValid && !x.Equals(hero) && x.IsAlive && x.IsAlly(hero) && x.IsControllable
                     && x.AttackCapability != AttackCapability.None && x.MoveCapability == MoveCapability.Ground
                     && !x.UnitState.HasFlag(UnitState.NoCollision) && x.Distance2D(blockTarget) < 1000)
            .ToList();

        var count = Math.Min(controllables.Count, settings.ControllablesCount);
        if (count <= 0)
        {
            updateHandler.IsEnabled = false;
            return;
        }

        for (var i = 0; i < count; i++)
        {
            var targetPosition = blockTarget.Position;
            var definitelynotrandomnumbers = settings.SpreadUnits
                                                 ? (float)Math.Ceiling(i / 2f)
                                                   * (i % 2 == 0 ? -1f + count * 0.06f : 1f - count * 0.06f)
                                                 : 0;
            var blockPosition = targetPosition + blockTarget.Vector3FromPolarAngle(definitelynotrandomnumbers)
                                * settings.BlockSensitivity;

            var controllable = controllables.OrderBy(x => x.Distance2D(blockPosition)).First();
            controllables.Remove(controllable);

            var angle = blockTarget.FindRotationAngle(controllable.Position);

            if (angle > 1.1 + i * 0.5)
            {
                var delta = angle * 0.6f;
                var side1 = targetPosition + blockTarget.Vector3FromPolarAngle(delta)
                            * Math.Max(settings.BlockSensitivity, 150);
                var side2 = targetPosition + blockTarget.Vector3FromPolarAngle(-delta)
                            * Math.Max(settings.BlockSensitivity, 150);

                controllable.Move(
                    side1.Distance(controllable.Position) < side2.Distance(controllable.Position) ? side1 : side2);
            }
            else
            {
                if (blockTarget.IsMoving && angle < 0.3 && controllable.IsMoving)
                {
                    controllable.Stop();
                }
                else
                {
                    controllable.Move(blockPosition);
                }
            }

            //delay between each issued command
            //await Task.Delay(50, token);
        }
    }
}