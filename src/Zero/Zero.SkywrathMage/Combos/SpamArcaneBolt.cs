using Divine.Core.ComboFactory;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Core.Managers.TargetSelector;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Components;
using Divine.Extensions;
using Divine.Game;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Particle;
using Divine.SkywrathMage.Menus;
using Divine.Update;

namespace Divine.SkywrathMage.Combos
{
    internal sealed class SpamArcaneBolt : BaseTaskHandler
    {
        private readonly SmartArcaneBoltMenu SmartArcaneBoltMenu;

        private readonly Abilities Abilities;

        private readonly TargetSelectorManager TargetSelector;

        private readonly UpdateHandler UpdateHandler;

        public SpamArcaneBolt(Common common)
        {
            SmartArcaneBoltMenu = ((MoreMenu)common.MenuConfig.MoreMenu).SmartArcaneBoltMenu;

            Abilities = (Abilities)common.Abilities;
            TargetSelector = common.TargetSelector;

            UpdateHandler = UpdateManager.CreateIngameUpdate(30, false, OnUpdate);

            SmartArcaneBoltMenu.SpamHotkeyItem.ValueChanged += SpamHotkeyChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            SmartArcaneBoltMenu.SpamHotkeyItem.ValueChanged -= SpamHotkeyChanged;
        }

        private void SpamHotkeyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
        {
            if (e.Value)
            {
                TargetSelector.TargetEffectsManager.DisableTargetDraw = true;

                UpdateHandler.IsEnabled = true;

                RunAsync();
            }
            else
            {
                Cancel();

                UpdateHandler.IsEnabled = false;
                ParticleManager.DestroyParticle("SpamTarget");
                spamTarget = null;

                TargetSelector.TargetEffectsManager.DisableTargetDraw = false;
            }
        }

        private CUnit spamTarget;

        private void OnUpdate()
        {
            CUnit unitTarget = null;
            if (SmartArcaneBoltMenu.SpamUnitsItem)
            {
                unitTarget = UnitManager<CUnit, Enemy, NoIllusion>.Units.Where(x =>
                                                          x.IsVisible &&
                                                          x.IsAlive &&
                                                          x.IsSpawned &&
                                                          (x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral ||
                                                          x.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit ||
                                                          x.ClassId == ClassId.CDOTA_BaseNPC_Warlock_Golem ||
                                                          x.ClassId == ClassId.CDOTA_BaseNPC_Creep ||
                                                          x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane ||
                                                          x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege ||
                                                          x.ClassId == ClassId.CDOTA_Unit_Hero_Beastmaster_Boar ||
                                                          x.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling ||
                                                          x.ClassId == ClassId.CDOTA_Unit_SpiritBear) &&
                                                          x.Distance2D(GameManager.MousePosition) <= 100).OrderBy(x => x.Distance2D(GameManager.MousePosition)).FirstOrDefault();
            }

            if (spamTarget == null || !spamTarget.IsValid || !spamTarget.IsAlive)
            {
                spamTarget = unitTarget ?? TargetSelector.Target; // TODO
            }

            if (spamTarget != null)
            {
                ParticleManager.CreateTargetLineParticle("SpamTarget", Owner.Base, spamTarget.Position, Color.Green);
            }
            else
            {
                ParticleManager.DestroyParticle("SpamTarget");
            }
        }

        protected override CUnit CurrentTarget
        {
            get
            {
                return spamTarget;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (IsStopped)
                {
                    return;
                }

                var target = CurrentTarget;
                if (IsNullTarget(target))
                {
                    Orbwalker.MoveToMousePosition();
                    return;
                }

                if (!target.IsMagicImmune())
                {
                    // Arcane Bolt
                    var arcaneBolt = Abilities.ArcaneBolt;
                    if (arcaneBolt.CanBeCasted && arcaneBolt.CanHit(target))
                    {
                        arcaneBolt.UseAbility(target);
                        var castDelay = arcaneBolt.GetCastDelay(target);
                        var hitTime = arcaneBolt.GetHitTime(target) - (castDelay + 340);
                        ZMultiSleeper<string>.DelaySleep($"IsHitTime_{target.Name}_{arcaneBolt.Name}", castDelay + 40, hitTime);
                        await Task.Delay(castDelay, token);
                    }
                }

                OrbwalkTo(target);
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        private void OrbwalkTo(CUnit target)
        {
            if (target.IsInvulnerable() || target.IsAttackImmune()) //TODO
            {
                Orbwalker.MoveToMousePosition();
                return;
            }

            switch (SmartArcaneBoltMenu.OrbwalkerItem.Value)
            {
                case "Default":
                    {
                        Orbwalker.OrbwalkTo(target);
                    }
                    break;

                case "Distance":
                    {
                        var ownerDis = Math.Min(Owner.Distance2D(GameManager.MousePosition), 230);
                        var ownerPos = Owner.Position.Extend(GameManager.MousePosition, ownerDis);
                        var pos = target.Position.Extend(ownerPos, SmartArcaneBoltMenu.MinDisInOrbwalkItem.Value);

                        Orbwalker.OrbwalkTo(target, pos);
                    }
                    break;

                case "Free":
                    {
                        var attackRange = Owner.AttackRange(target);
                        if (Owner.Distance2D(target) <= attackRange && !SmartArcaneBoltMenu.FullFreeModeItem || target.Distance2D(GameManager.MousePosition) <= attackRange)
                        {
                            Orbwalker.OrbwalkTo(target);
                        }
                        else
                        {
                            Orbwalker.MoveToMousePosition();
                        }
                    }
                    break;

                case "Only Attack":
                    {
                        Orbwalker.AttackTo(target);
                    }
                    break;

                case "No Move":
                    {
                        if (Owner.Distance2D(target) < Owner.AttackRange(target))
                        {
                            Orbwalker.AttackTo(target);
                        }
                    }
                    break;
            }
        }
    }
}
