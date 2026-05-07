namespace CreepsBlocker;

using System;
using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Extensions;
using Divine.Game;
using Divine.GameConsole;
using Divine.Menu.EventArgs;
using Divine.Numerics;
using Divine.Service;
using Divine.Update;

// [ExportPlugin("Creeps Blocker", StartupMode.Auto, "IdcNoob")]
internal class CreepsBlocker : Bootstrapper
{
    private Unit hero;

    private Config config;

    private UpdateHandler updateHandler;

    protected override void OnActivate()
    {
        config = new Config();
        config.MenuKey.ValueChanged += KeyPressed;

        hero = EntityManager.LocalHero;
        updateHandler = UpdateManager.CreateIngameUpdate(50, false, OnUpdate);
    }

    protected override void OnDeactivate()
    {
        config.MenuKey.ValueChanged -= KeyPressed;
        UpdateManager.DestroyGameUpdate(OnUpdate);
        config.Dispose();
    }

    private void KeyPressed(object sender, HoldKeyEventArgs e)
    {
        updateHandler.IsEnabled = e.Value;

        if (!config.CenterCamera)
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

        var creeps = EntityManager.GetEntities<Creep>().Where(x => x.IsValid && x.IsSpawned && x.IsAlive && x.Team == hero.Team && x.Distance2D(hero) < 500).ToList();
        if (!creeps.Any())
        {
            return;
        }

        var creepsMoveDirection = creeps.Aggregate(new Vector3(), (sum, creep) => sum + creep.InFront(1000)) / creeps.Count;

        var tower = EntityManager.GetEntities<Tower>().FirstOrDefault(x => x.IsValid && x.IsAlive && x.Distance2D(hero) < 150 );

        if (tower != null)
        {
            // dont block near retarded dire mid t2 tower
            hero.Move(creepsMoveDirection);
            return;
        }

        foreach (var creep in creeps.OrderByDescending(x => x.IsMoving).ThenBy(x => x.Distance2D(creepsMoveDirection)))
        {
            if (!config.BlockRangedCreep.Value && creep.IsRanged)
            {
                continue;
            }

            var creepDistance = creep.Distance2D(creepsMoveDirection) + 50;
            var heroDistance = hero.Distance2D(creepsMoveDirection);
            var creepAngle = creep.FindRotationAngle(hero.Position);

            if (creepDistance < heroDistance && creepAngle > 2 || creepAngle > 2.5)
            {
                continue;
            }

            var moveDistance = config.BlockSensitivity / hero.MovementSpeed * 100;
            var movePosition = creep.InFront(Math.Max(moveDistance, moveDistance * creepAngle));

            if (movePosition.Distance(creepsMoveDirection) > heroDistance)
            {
                continue;
            }

            if (creepAngle < 0.3f)
            {
                if (hero.MovementSpeed - creep.MovementSpeed > 50 && creeps.Select(x => x.FindRotationAngle(hero.Position)).Average() < 0.4)
                {
                    hero.Stop();
                    return;
                }

                continue;
            }

            hero.Move(movePosition);
            return;
        }

        hero.Stop();
    }
}