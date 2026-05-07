using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Divine.Entity;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Game.EventArgs;
using Divine.GameConsole;
using Divine.Helpers;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Service;
using Divine.Update;

namespace BadGuyByHappyAngel
{
    public class Bootstrap : Bootstrapper
    {
        private MenuSwitcher EnableSwitcher;

        private MenuSwitcher AutoPauseOnKillHeroSwitcher;

        private MenuSwitcher ZXC;

        private MenuSwitcher Unpause;

        private MenuSwitcher AutoChatSwitcher;

        private MenuSelector LangSelector;

        private MenuSwitcher AutoFeedSwitcher;

        private MenuSwitcher AutoTauntOnKillHeroSwitcher;

        private MenuSwitcher itstruedeadinside;

        private MenuSwitcher AutoTauntSwitcher;
        
        private MenuSwitcher AutoLolOnKillHeroSwitcher;

        private MenuSwitcher AutoLolSwitcher;

        private Random Random;

        private List<int> SentMsg = new List<int>();

        private int index = 0;

        private readonly string[] EngText =
        {
            "You only need to play supports",
            "LOX",
            "useless",
            "Remove the game mediocre",
            "PRIVET LOX",
            "You lose",
            "Solo one on one",
            "???",
            "1000 - 7?"
        };

        private readonly string[] RusText =
        {
            "???",
            "1000 - 7?",
            "Удали игру мой друг ИВАН и то лучше играет",
            "ПЕТУХ",
            "Слабый",
            "Я лучше тебя ^)",
            "Смотри на мою игру лучше",
            "НЮХАЙ БЕБРУ",
            "Раз на раз цука",
            "Помойка",
            "Я ТОП 1 ",
            "Твой рейт максимум единица чел...",
            "Чел ты...",
            "ROCFALL КОГДА КОМБО НА ИНВОКЕРА ААА???"
        };
        Dictionary<string, int> ZXC7 = new Dictionary<string, int>();
        
        private readonly string[] ZXC1 =
        {
            "а у вас походу умирать это семейное",
            "нахуя пидораса убил?",
            "чао персик дозревай",
            "уважаемый в тюрьме вы будете водолазом",
            "говори буду плохо говорить буду сосать",
            "буду плохо сосать буду пересасывать",
            "долбаеб иди башмачки в сундучок школьный собирай",
            "ботинок ебаный чо слетел",
            "братик маме привет передай",
            "не противник",
            "а ты че клоун???",
            "я обоссал тебя",
            "ты че там отлетел то?",
            "упал хуета ебаная",
            "но в боди забрал да похуй все равно упал",
            "ливай с",
            "до связи башмак",
            "нищета глупейшая играть учись",
            "опущен сын твари",
            "пофикси нищ",
            "оттарабанен армянская королева",
            "сука не позорься и ливни",
            "улетел тапочек ебаный",
            "единицей свалился фуфлыжник",
            "зачем ты играешь тут безмозглый",
            "иди кумыса попей очередняра",
            "откисай сочняра",
            "АХАХА ЕБАТЬ У ТЕБЯ ЧЕРЕПНАЯ КОРОБКА ПРЯМ КАК",
            "на мыло и веревку то деньги есть????",
            "ИЩИ СЕБЯ НА pornoeb.cc/sochniki",
            "свежий кабанчик",
            "до связи на подскоке кабанчик",
            "скажи маме сухарики купить долбаеб",
            "ебать ты красиво на бутылку присел , тебе дать альт ?",
            "прости что без смазки)",
            "алло это скорая? тут такая ситуация пареню который упал нужна скорая)",
            "ало ты мапу лузаешь дура очнись",
            "тяжело с кряком????",
            "ЕБУЧЕСТЬ ВТОРОГО РАЗРЯДА ВЫДВИЖЕНЕЦ ОТКИС",
            "але а противники то где???",
            "ХУЕПРЫГАЛО ТУСОВОЧНОЕ КУДА ПОЛЕТЕЛО",
            "ты куда жертва козьего аборта"
         };

        protected override void OnActivate()
        {
            
            foreach (var item in ZXC1) 
            {
                ZXC7.Add(item, 0);
            } 

            var rootmenu = MenuManager.CreateRootMenu("Bad guy by HappyAngel");
            EnableSwitcher = rootmenu.CreateSwitcher("Enable", false);
            AutoPauseOnKillHeroSwitcher = rootmenu.CreateSwitcher("Auto pause on kill hero", false);
            ZXC = rootmenu.CreateSwitcher("ZXC MODE",false);
            AutoChatSwitcher = rootmenu.CreateSwitcher("Auto chat on kill hero", false);
            LangSelector = rootmenu.CreateSelector("Lang", new[] { "Eng", "Rus" });
            AutoFeedSwitcher = rootmenu.CreateSwitcher("Auto feed", false);
            AutoTauntOnKillHeroSwitcher = rootmenu.CreateSwitcher("Auto taunt on kill hero", false);
            AutoTauntSwitcher = rootmenu.CreateSwitcher("Auto taunt", false);
            AutoLolOnKillHeroSwitcher = rootmenu.CreateSwitcher("Auto lol on kill hero", false);
            AutoLolSwitcher = rootmenu.CreateSwitcher("Auto lol", false);
            itstruedeadinside = rootmenu.CreateSwitcher("its true dead inside", false); 
            Unpause = rootmenu.CreateSwitcher("Unpause", false);

            EnableSwitcher.ValueChanged += OnEnableValueChanged;
        }

        private void OnEnableValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                Random = new Random();
                GameManager.GameEvent += OnGameEvent;
                UpdateManager.CreateIngameUpdate(1000, OnUpdate);
            }
            else
            {
                GameManager.GameEvent -= OnGameEvent;
                UpdateManager.DestroyIngameUpdate(OnUpdate);
            }
        }

        private async void OnGameEvent(GameEventEventArgs e)
        {
            var gameEvent = e.GameEvent;

            if (gameEvent.Name != "entity_killed")
            {
                return;
            }

            if (gameEvent.GetInt32("entindex_attacker") == EntityManager.LocalHero.Index &&
                EntityManager.GetEntityByIndex(gameEvent.GetInt32("entindex_killed")) is Hero hero &&
                !hero.IsIllusion)
            {
                //await Task.Delay(500);

                if (AutoPauseOnKillHeroSwitcher)
                {
                    GameConsoleManager.ExecuteCommand("dota_pause");
                }

                if (AutoTauntOnKillHeroSwitcher && !MultiSleeper<string>.Sleeping("AutoTaunt"))
                {
                    GameConsoleManager.ExecuteCommand("use_item_client current_hero taunt");
                    MultiSleeper<string>.Sleep("AutoTaunt", 7500);
                }

                if (AutoLolOnKillHeroSwitcher && !MultiSleeper<string>.Sleeping("AutoLol"))
                {
                    GameConsoleManager.ExecuteCommand("say lol");
                    MultiSleeper<string>.Sleep("AutoLol", 15000);

                    //await Task.Delay(300);
                }

                if (itstruedeadinside){ 
                for (int i = 1000; i > 2; i -= 7)
                {
                        GameConsoleManager.ExecuteCommand($"say {i}");
                        await Task.Delay(150);
                }
            }



            if (AutoChatSwitcher)
                {
                    var text = LangSelector == "Eng" ? EngText : RusText;
                    index = Random.Next(0, text.Length);

                    GameConsoleManager.ExecuteCommand($"say {text[index]}");
                }

                if (ZXC)
                {
                    if (SentMsg.Count == 42)
                    {
                        foreach (var item in ZXC1)
                        {
                            ZXC7[item] = 0;
                        }
                        SentMsg.Clear();
                        index = Random.Next(0, ZXC1.Length);
                    }

                    do
                    {
                        index = Random.Next(0, ZXC1.Length);
                    } while (SentMsg.Contains(index) && SentMsg.Count != 42 && ZXC7[ZXC1[index]] == 1);

                    SentMsg.Add(index);
                    
                    GameConsoleManager.ExecuteCommand($"say {ZXC1[index]}");
                }
            }

            if (EntityManager.GetEntityByIndex(gameEvent.GetInt32("entindex_killed")) is Hero hero2 && hero2.Position.Distance(EntityManager.LocalHero.Position) < 1000)
            {
                if (AutoTauntOnKillHeroSwitcher && !MultiSleeper<string>.Sleeping("AutoTaunt"))
                {
                    GameConsoleManager.ExecuteCommand("use_item_client current_hero taunt");
                    MultiSleeper<string>.Sleep("AutoTaunt", 7500);
                }
            }
        }

        private void OnUpdate()
        {
            if (Unpause && !MultiSleeper<string>.Sleeping("UNPAUSE") && GameManager.IsPaused ) 
            {
               
                GameConsoleManager.ExecuteCommand("dota_pause");
                MultiSleeper<string>.Sleep("UNPAUSE", 250);
            }

            var localHero = EntityManager.LocalHero;
            if (localHero == null || !localHero.IsValid || GameManager.IsPaused)
            {
                return;
            }



            if (AutoFeedSwitcher && !MultiSleeper<string>.Sleeping("AutoFeed"))
            {
                var fort = EntityManager.GetEntities<Fort>().FirstOrDefault(x => !x.IsAlly(localHero));
                if (fort != null)
                {
                    localHero.Attack(fort.Position);
                }

                MultiSleeper<string>.Sleep("AutoFeed", 5000);
            }

            if (AutoTauntSwitcher && !MultiSleeper<string>.Sleeping("AutoTaunt"))
            {
                GameConsoleManager.ExecuteCommand("use_item_client current_hero taunt");
                MultiSleeper<string>.Sleep("AutoTaunt", 7500);
            }

            if (AutoLolSwitcher && !MultiSleeper<string>.Sleeping("AutoLol"))
            {
                GameConsoleManager.ExecuteCommand("say lol");
                MultiSleeper<string>.Sleep("AutoLol", 15000);
            }
        }
    }
}
