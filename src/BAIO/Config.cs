namespace BAIO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text.Json;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Game;
    using Divine.Input;
    using Divine.Menu;
    using Divine.Menu.Items;
    using Divine.Numerics;
    using Divine.Service;

    public class Config : IDisposable
    {
        private bool disposed;

        public Config(HeroId id, bool supported = true)
        {
            this.RootMenu = MenuManager.CreateRootMenu("BAIO");
            this.General = new GeneralMenu(this.RootMenu);
            this.Hero = new HeroMenu(this.RootMenu, id, supported);
        }

        public RootMenu RootMenu { get; }

        public GeneralMenu General { get; set; }

        public HeroMenu Hero { get; set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (this.disposed)
            //{
            //    return;
            //}

            //if (disposing)
            //{
            //    this.RootMenu.Dispose();
            //}

            //this.disposed = true;
        }

        public class GeneralMenu : IDisposable
        {
            private bool disposed;

            public GeneralMenu(RootMenu rootMenu)
            {
                this.Factory = rootMenu.CreateMenu("General");

                this.Enabled = this.Factory.CreateSwitcher("Enabled");

                this.ComboKey = this.Factory.CreateHoldKey("Combo Key", Key.None);
                this.HarassKey = this.Factory.CreateHoldKey("Harass Key", Key.None);

                this.Killsteal = this.Factory.CreateSwitcher("Killsteal");
                this.Killsteal.SetTooltip("Enable Killsteal");

                this.DrawTargetIndicator = this.Factory.CreateSwitcher("Draw Target Indicator");
                this.DrawTargetIndicator.SetTooltip("Draws Target Indicator on top of the target");

                this.LockTarget = this.Factory.CreateSwitcher("Lock Target");
                this.LockTarget.SetTooltip("Keep your target until it's dead or you release the key");

                this.KiteMode = this.Factory.CreateSwitcher("Kite mode");
                this.KiteMode.SetTooltip("Only try to attack when in attack range");
            }

            public MenuHoldKey ComboKey { get; }

            public MenuHoldKey HarassKey { get; }

            public MenuSwitcher DrawTargetIndicator { get; }

            public Menu Factory { get; }

            public MenuSwitcher Killsteal { get; }

            public MenuSwitcher KiteMode { get; }

            public MenuSwitcher LockTarget { get; }

            public MenuSwitcher Enabled { get; }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                //if (this.disposed)
                //{
                //    return;
                //}

                //if (disposing)
                //{
                //    this.Factory.Dispose();
                //}

                //this.disposed = true;
            }
        }

        public class HeroMenu : IDisposable
        {
            private bool disposed;
            public Dictionary<int, string> Translations { get; set; }

            public HeroMenu(RootMenu rootMenu, HeroId heroId, bool supported)
            {
                if (!supported)
                {
                    this.Factory = rootMenu.CreateMenu($"{GameManager.GetLocalize(heroId.ToString())} - not supported!")
                        .SetHeroImage(heroId)
                        .SetFontColor(Color.Orange);
                    return;
                }

                var assembly = Assembly.GetExecutingAssembly();

                var lang = DivineService.Language;
                if (lang == Language.Cn)
                {
                    var stream = assembly.GetManifestResourceStream("BAIO.Localization.cn.json");
                    Translations = JsonSerializer.Deserialize<Dictionary<int, string>>(new StreamReader(stream).ReadToEnd());;
                }
                else if (lang == Language.Ru)
                {
                    var stream = assembly.GetManifestResourceStream("BAIO.Localization.ru.json");
                    Translations = JsonSerializer.Deserialize<Dictionary<int, string>>(new StreamReader(stream).ReadToEnd());
                }
                else
                {
                    var stream = assembly.GetManifestResourceStream("BAIO.Localization.en.json");
                    Translations = JsonSerializer.Deserialize<Dictionary<int, string>>(new StreamReader(stream).ReadToEnd());
                }

                this.Factory = rootMenu.CreateMenu(GameManager.GetLocalize(heroId.ToString())).SetHeroImage(heroId);
                this.ItemMenu = Factory.CreateMenu("items", "Items");
                this.LinkenBreakerMenu = ItemMenu.CreateMenu("linkenbreakers", "Linken Breakers");
                this.LinkenBreakerTogglerMenu = LinkenBreakerMenu.CreateAbilityToggler("AbilityToggler", "", LinkenAbilityTogglerDic, true);

                this.UnitController = Factory.CreateMenu(Translations[2]).SetImage("npc_dota_neutral_centaur_khan", MenuImageType.Unit);
                this.Bodyblocker = this.UnitController.CreateMenu(Translations[3]).SetAbilityImage(AbilityId.earthshaker_fissure);
                var ucMenu = this.UnitController;
                var bb = this.Bodyblocker;
                this.ControlUnits = ucMenu.CreateSwitcher(Translations[4]);
                this.Enabled = bb.CreateSwitcher(Translations[5]);
                this.UseUnitAbilities = ucMenu.CreateSwitcher(Translations[6]);
                this.BlockSensitivity = bb.CreateSlider(Translations[7], 150, 50, 300);
                this.BlockSensitivity.SetTooltip(Translations[8]);
            }

            public Menu Factory { get; }
            public Menu ItemMenu { get; }
            public Menu LinkenBreakerMenu { get; }
            public Menu UnitController;
            public Menu Bodyblocker;

            public MenuAbilityToggler LinkenBreakerTogglerMenu;

            public MenuSwitcher ControlUnits;
            public MenuSwitcher Enabled;
            public MenuSlider BlockSensitivity;
            public MenuSwitcher UseUnitAbilities;

            public Dictionary<AbilityId, bool> LinkenAbilityTogglerDic = new Dictionary<AbilityId, bool>
            {
                    { AbilityId.item_sheepstick, true},
                    { AbilityId.item_rod_of_atos, true},
                    { AbilityId.item_nullifier, true },
                    { AbilityId.item_bloodthorn, true },
                    { AbilityId.item_orchid, true },
                    { AbilityId.item_cyclone, true },
                    { AbilityId.item_force_staff, true },
                    { AbilityId.item_diffusal_blade, true }
            };

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                //if (this.disposed)
                //{
                //    return;
                //}

                //if (disposing)
                //{
                //    this.Factory.Dispose();
                //}

                //this.disposed = true;
            }
        }
    }
}