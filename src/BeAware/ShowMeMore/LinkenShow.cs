//namespace BeAware.ShowMeMore;

//using BeAware.MenuManager.ShowMeMore;

//using Divine.Entity;
//using Divine.Entity.Entities.Units.Heroes;
//using Divine.Extensions;
//using Divine.Menu.EventArgs;
//using Divine.Menu.Items;
//using Divine.Particle;
//using Divine.Particle.Components;
//using Divine.Update;

//internal sealed class LinkenShow
//{
//    private readonly LinkenShowMenu LinkenShowMenu;

//    private readonly Hero LocalHero = EntityManager.LocalHero;

//    public LinkenShow(Common common)
//    {
//        LinkenShowMenu = common.MenuConfig.ShowMeMoreMenu.LinkenShowMenu;

//        LinkenShowMenu.EnableItem.ValueChanged += OnEnableValueChanged;
//    }

//    public void Dispose()
//    {
//        if (LinkenShowMenu.EnableItem)
//        {
//            UpdateManager.DestroyIngameUpdate(OnUpdate);

//            foreach (var hero in EntityManager.GetEntities<Hero>())
//            {
//                if (hero.IsAlly(LocalHero))
//                {
//                    continue;
//                }

//                RemoveLinkenShow(hero);
//            }
//        }
//    }

//    private void OnEnableValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
//    {
//        if (e.Value)
//        {
//            UpdateManager.CreateIngameUpdate(300, OnUpdate);
//        }
//        else
//        {
//            UpdateManager.DestroyIngameUpdate(OnUpdate);

//            foreach (var hero in EntityManager.GetEntities<Hero>())
//            {
//                if (hero.IsAlly(LocalHero))
//                {
//                    continue;
//                }

//                RemoveLinkenShow(hero);
//            }
//        }
//    }

//    private void OnUpdate()
//    {
//        foreach (var hero in EntityManager.GetEntities<Hero>())
//        {
//            if (hero.IsAlly(LocalHero))
//            {
//                continue;
//            }

//            if (hero.IsLinkensProtected() && hero.IsVisible && hero.IsAlive)
//            {
//                AddLinkenShow(hero);
//            }
//            else
//            {
//                RemoveLinkenShow(hero);
//            }
//        }
//    }

//    private void AddLinkenShow(Hero hero)
//    {
//        ParticleManager.CreateParticle($"LinkenShow_{hero.Handle}", "particles/items_fx/immunity_sphere_buff.vpcf", Attachment.CenterFollow, hero);
//    }

//    private void RemoveLinkenShow(Hero hero)
//    {
//        ParticleManager.DestroyParticle($"LinkenShow_{hero.Handle}");
//    }
//}