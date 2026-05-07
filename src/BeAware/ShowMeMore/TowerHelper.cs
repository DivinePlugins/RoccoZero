//namespace BeAware.ShowMeMore;

//using BeAware.MenuManager.ShowMeMore;

//using Divine.Entity;
//using Divine.Entity.Entities.Units.Buildings;
//using Divine.Entity.Entities.Units.Heroes;
//using Divine.Extensions;
//using Divine.Menu.EventArgs;
//using Divine.Menu.Items;
//using Divine.Numerics;
//using Divine.Particle;
//using Divine.Particle.Components;
//using Divine.Update;

//internal sealed class TowerHelper
//{
//    private readonly TowerHelperMenu TowerHelperMenu;

//    private readonly Hero LocalHero = EntityManager.LocalHero;

//    public TowerHelper(Common common)
//    {
//        TowerHelperMenu = common.MenuConfig.ShowMeMoreMenu.TowerHelperMenu;

//        TowerHelperMenu.AutoAttackRangeItem.ValueChanged += OnAutoAttackRangeValueChanged;
//    }

//    public void Dispose()
//    {
//        TowerHelperMenu.AutoAttackRangeItem.ValueChanged -= OnAutoAttackRangeValueChanged;

//        if (TowerHelperMenu.AutoAttackRangeItem)
//        {
//            UpdateManager.DestroyIngameUpdate(OnUpdate);
//        }
//    }

//    private void OnAutoAttackRangeValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
//    {
//        if (e.Value)
//        {
//            UpdateManager.CreateIngameUpdate(400, OnUpdate);
//        }
//        else
//        {
//            UpdateManager.DestroyIngameUpdate(OnUpdate);

//            foreach (var tower in EntityManager.GetEntities<Tower>())
//            {
//                if (!tower.IsEnemy(LocalHero))
//                {
//                    continue;
//                }

//                var id = $"TowerHelper_{tower.Handle}";
//                ParticleManager.DestroyParticle(id);
//            }
//        }
//    }

//    private void OnUpdate()
//    {
//        foreach (var tower in EntityManager.GetEntities<Tower>())
//        {
//            if (!tower.IsEnemy(LocalHero))
//            {
//                continue;
//            }

//            var id = $"TowerHelper_{tower.Handle}";
//            var position = tower.Position;
//            if (LocalHero.Distance2D(position) < 1100)
//            {
//                ParticleManager.CreateParticle(
//                    id,
//                    @"particles\ui_mouseactions\range_finder_tower_aoe_ring.vpcf",
//                    Attachment.AbsOrigin,
//                    tower,
//                    new ControlPoint(2, position),
//                    new ControlPoint(3, tower.AttackRange(LocalHero), 0, 0),
//                    new ControlPoint(4, 255, 0, 0));
//            }
//            else
//            {
//                ParticleManager.DestroyParticle(id);
//            }
//        }
//    }
//}