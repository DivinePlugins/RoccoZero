//using System;
//using System.Linq;

//using BeAware.MenuManager.ShowMeMore;
//using Divine.Core.Entities;
//using Divine.Core.Helpers;
//using Divine.Core.Managers;
//using Divine.Core.Managers.Renderer.Particle;
//using Divine.Core.Managers.Unit;
//using Divine.Core.Managers.Update;

//using Ensage;
//using Ensage.SDK.Extensions;
//using Ensage.SDK.Menu.ValueBinding;

//using Divine.Numerics;

//namespace BeAware.ShowMeMore
//{
//    internal sealed class VisibleByEnemy
//    {
//        private readonly CHero Owner = UnitManager.Owner;

//        private readonly VisibleByEnemyMenu VisibleByEnemyMenu;

//        public VisibleByEnemy(Common common)
//        {
//            VisibleByEnemyMenu = common.MenuConfig.ShowMeMoreMenu.VisibleByEnemyMenu;

//            if (VisibleByEnemyMenu.EnableItem)
//            {
//                VisibleByEnemyMenu.EffectTypeItem.ValueChanging += EffectTypeChanging;

//                VisibleByEnemyMenu.RedItem.ValueChanging += ColorChanging;
//                VisibleByEnemyMenu.GreenItem.ValueChanging += ColorChanging;
//                VisibleByEnemyMenu.BlueItem.ValueChanging += ColorChanging;
//                VisibleByEnemyMenu.AlphaItem.ValueChanging += ColorChanging;

//                UpdateManager.Subscribe(250, OnUpdate);

//                if (AppDomain.CurrentDomain.GetAssemblies().Any(x => !x.GlobalAssemblyCache && x.GetName().Name.Contains("VisibleByEnemy")))
//                {
//                    new DivineMessage(true, DivineMessage.DefaultPosition, 15, new Message("BeAware has VisibleByEnemy! Please disable others", Color.Red));
//                }
//            }

//            VisibleByEnemyMenu.EnableItem.Changed += EnableChanged;
//        }

//        public void Dispose()
//        {
//            VisibleByEnemyMenu.EnableItem.Changed -= EnableChanged;

//            if (VisibleByEnemyMenu.EnableItem)
//            {
//                UpdateManager.Unsubscribe(OnUpdate);

//                VisibleByEnemyMenu.EffectTypeItem.ValueChanging -= EffectTypeChanging;

//                VisibleByEnemyMenu.RedItem.ValueChanging -= ColorChanging;
//                VisibleByEnemyMenu.GreenItem.ValueChanging -= ColorChanging;
//                VisibleByEnemyMenu.BlueItem.ValueChanging -= ColorChanging;
//                VisibleByEnemyMenu.AlphaItem.ValueChanging -= ColorChanging;

//                foreach (var unit in UnitManager.Units)
//                {
//                    if (!unit.IsVisibleToEnemies)
//                    {
//                        continue;
//                    }

//                    HandleEffect(unit, false);
//                }
//            }
//        }

//        private void EnableChanged(object sender, ValueChangingEventArgs<bool> e)
//        {
//            if (e.Value)
//            {
//                VisibleByEnemyMenu.EffectTypeItem.ValueChanging += EffectTypeChanging;

//                VisibleByEnemyMenu.RedItem.ValueChanging += ColorChanging;
//                VisibleByEnemyMenu.GreenItem.ValueChanging += ColorChanging;
//                VisibleByEnemyMenu.BlueItem.ValueChanging += ColorChanging;
//                VisibleByEnemyMenu.AlphaItem.ValueChanging += ColorChanging;

//                UpdateManager.Subscribe(250, OnUpdate);

//                if (AppDomain.CurrentDomain.GetAssemblies().Any(x => !x.GlobalAssemblyCache && x.GetName().Name.Contains("VisibleByEnemy")))
//                {
//                    new DivineMessage(true, DivineMessage.DefaultPosition, 15, new Message("BeAware has VisibleByEnemy! Please disable others", Color.Red));
//                }
//            }
//            else
//            {
//                UpdateManager.Unsubscribe(OnUpdate);

//                VisibleByEnemyMenu.EffectTypeItem.ValueChanging -= EffectTypeChanging;

//                VisibleByEnemyMenu.RedItem.ValueChanging -= ColorChanging;
//                VisibleByEnemyMenu.GreenItem.ValueChanging -= ColorChanging;
//                VisibleByEnemyMenu.BlueItem.ValueChanging -= ColorChanging;
//                VisibleByEnemyMenu.AlphaItem.ValueChanging -= ColorChanging;

//                foreach (var unit in UnitManager.Units)
//                {
//                    if (!unit.IsVisibleToEnemies)
//                    {
//                        continue;
//                    }

//                    HandleEffect(unit, false);
//                }
//            }
//        }

//        private void EffectTypeChanging(object sender, ValueChangingEventArgs<string> e)
//        {
//            UpdateManager.BeginInvoke(() =>
//            {
//                Owner.Stop();
//                HandleEffect(Owner, true);
//                AddEffectType = false;
//            });
//        }

//        private void ColorChanging(object sender, ValueChangingEventArgs<int> e)
//        {
//            UpdateManager.BeginInvoke(() =>
//            {
//                Owner.Stop();
//                HandleEffect(Owner, true);
//                AddEffectType = false;
//            });
//        }

//        private void OnUpdate()
//        {
//            if (VisibleByEnemyMenu.AlliedHeroesItem)
//            {
//                foreach (var hero in UnitManager<CHero, Ally>.Units)
//                {
//                    HandleEffect(hero, hero.IsVisibleToEnemies);
//                }
//            }

//            if (VisibleByEnemyMenu.BuildingsItem)
//            {
//                foreach (var building in UnitManager<CBuilding, Ally>.Units)
//                {
//                    HandleEffect(building, building.IsVisibleToEnemies);
//                }
//            }

//            if (VisibleByEnemyMenu.NeutralsItem)
//            {
//                foreach (var neutral in UnitManager<CNeutral>.Units)
//                {
//                    HandleEffect(neutral, neutral.IsVisibleToEnemies);
//                }
//            }

//            var units = UnitManager<CUnit, Ally>.Units;

//            if (VisibleByEnemyMenu.WardsItem)
//            {
//                foreach (var ward in units.Where(x => x.NetworkClassId == NetworkClassId.CDOTA_NPC_Observer_Ward || x.NetworkClassId == NetworkClassId.CDOTA_NPC_Observer_Ward_TrueSight))
//                {
//                    HandleEffect(ward, ward.IsVisibleToEnemies);
//                }
//            }

//            if (VisibleByEnemyMenu.MinesItem)
//            {
//                foreach (var mine in units.Where(x => x.NetworkClassId == NetworkClassId.CDOTA_NPC_TechiesMines))
//                {
//                    HandleEffect(mine, mine.IsVisibleToEnemies);
//                }
//            }

//            if (VisibleByEnemyMenu.ShrinesItem)
//            {
//                foreach (var shrine in units.Where(x => x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Healer))
//                {
//                    HandleEffect(shrine, shrine.IsVisibleToEnemies);
//                }
//            }

//            if (VisibleByEnemyMenu.UnitsItem)
//            {
//                foreach (var unit in units.Where(IsUnit))
//                {
//                    HandleEffect(unit, unit.IsVisibleToEnemies);
//                }
//            }
//        }

//        private bool IsUnit(CUnit sender)
//        {
//            return !(sender is CHero) && !(sender is CBuilding)
//                   && (sender.NetworkClassId != NetworkClassId.CDOTA_BaseNPC_Creep_Lane && sender.NetworkClassId != NetworkClassId.CDOTA_BaseNPC_Creep_Siege || sender.IsControllable)
//                   && sender.NetworkClassId != NetworkClassId.CDOTA_NPC_TechiesMines
//                   && sender.NetworkClassId != NetworkClassId.CDOTA_NPC_Observer_Ward
//                   && sender.NetworkClassId != NetworkClassId.CDOTA_NPC_Observer_Ward_TrueSight
//                   && sender.NetworkClassId != NetworkClassId.CDOTA_BaseNPC_Healer;
//        }

//        private bool AddEffectType;

//        private void HandleEffect(CUnit unit, bool visible)
//        {
//            if (!AddEffectType && Owner.NetworkActivity != NetworkActivity.Idle)
//            {
//                AddEffectType = true;
//            }

//            if (visible && unit.IsAlive && unit.Position.IsOnScreen())
//            {
//                ParticleManager.AddOrUpdate(
//                    $"VisibleByEnemy_{unit.Handle}",
//                    Effects[VisibleByEnemyMenu.EffectTypeItem.SelectedIndex],
//                    unit,
//                    Attachment.AbsOriginFollow,
//                    RestartType.NormalRestart,
//                    new ControlPoint(1, VisibleByEnemyMenu.RedItem.Value, VisibleByEnemyMenu.GreenItem.Value, VisibleByEnemyMenu.BlueItem.Value),
//                    new ControlPoint(2, VisibleByEnemyMenu.AlphaItem.Value));
//            }
//            else if (AddEffectType)
//            {
//                ParticleManager.Remove($"VisibleByEnemy_{unit.Handle}");
//            }
//        }

//        private static readonly string[] Effects =
//        {
//            "particles/items_fx/aura_shivas.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy.vpcf",
//            "materials/ensage_ui/particles/vbe.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_omniknight.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_assault.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_arrow.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_mark.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_glyph.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_coin.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_lightning.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_energy_orb.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_pentagon.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_axis.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_beam_jagged.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_beam_rainbow.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_walnut_statue.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_thin_thick.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_ring_wave.vpcf",
//            "materials/ensage_ui/particles/visiblebyenemy_visible.vpcf"
//        };
//    }
//}