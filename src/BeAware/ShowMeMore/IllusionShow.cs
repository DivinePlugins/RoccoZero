
//using System;
//using System.Linq;

//using BeAware.MenuManager.ShowMeMore;
//using Divine.Menu.EventArgs;
//using Divine.Menu.Items;
//using Divine.SDK.Extensions;

//namespace BeAware.ShowMeMore
//{
//    internal sealed class IllusionShow
//    {
//        private readonly IllusionShowMenu IllusionShowMenu;

//        private readonly Hero LocalHero = EntityManager.LocalHero;

//        public IllusionShow(Common common)
//        {
//            IllusionShowMenu = common.MenuConfig.ShowMeMoreMenu.IllusionShowMenu;

//            IllusionShowMenu.EnableItem.ValueChanged += OnEnableValueChanged;
//        }

//        private void OnEnableValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
//        {
//            if (e.Value)
//            {
//                UpdateIllusion();
//            }
//            else
//            {
//                UpdateIllusion(true);
//            }
//        }

//        public void Dispose()
//        {
//            if (IllusionShowMenu.EnableItem)
//            {
//                UpdateIllusion(true);
//            }

//            IllusionShowMenu.EnableItem.ValueChanged += OnEnableValueChanged;
//        }

//        public bool Entity(Unit unit)
//        {
//            if (!IllusionShowMenu.EnableItem)
//            {
//                return false;
//            }

//            if (unit is not Hero hero)
//            {
//                return false;
//            }

//            if (hero.IsAlly(LocalHero) || !hero.IsAlive)
//            {
//                return false;
//            }

//            if ((hero.Id == HeroId.npc_dota_hero_arc_warden && hero.ModifierStatus.GetBuffsByName("modifier_arc_warden_tempest_double").Any()) || !hero.IsIllusion)
//            {
//                return false;
//            }

//            Illusions(hero);
//            return true;
//        }

//        private void UpdateIllusion(bool remove = false)
//        {
//            foreach (var hero in EntityManager.GetEntities<Hero>())
//            {
//                if (hero.IsAlly(LocalHero) || !hero.IsAlive)
//                {
//                    continue;
//                }

//                if ((hero.Id == HeroId.npc_dota_hero_arc_warden && hero.ModifierStatus.GetBuffsByName("modifier_arc_warden_tempest_double").Any()) || !hero.IsIllusion)
//                {
//                    continue;
//                }

//                if (remove)
//                {
//                    ParticleManager.DestroyParticle($"Illusion_{hero.Handle}");
//                    continue;
//                }

//                Illusions(hero);
//            }
//        }

//        private void Illusions(Unit unit)
//        {
//            ParticleManager.CreateParticle(
//                $"Illusion_{ unit.Handle }",
//                "materials/ensage_ui/particles/illusions_mod_v2.vpcf",
//                unit,
//                Attachment.AbsOrigin,
//                ParticleRestartType.FullRestart,
//                new ControlPoint(1, 0, 100, 0),
//                new ControlPoint(2, 0, 50, 255));
//        }
//    }
//}