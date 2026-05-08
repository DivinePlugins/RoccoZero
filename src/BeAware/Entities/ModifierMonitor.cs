namespace BeAware.Entities;

using System;

using BeAware.Data;
using BeAware.Helpers;
using BeAware.MenuManager.PartialMapHack;
using BeAware.ShowMeMore.MoreInformation;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Helpers;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.Modifier.Modifiers;

internal sealed class ModifierMonitor
{
    private readonly PartialMapHackMenu PartialMapHackMenu;

    private readonly Verification Verification;

    private readonly SpiritBreakerCharge SpiritBreakerCharge;

    private readonly LifeStealerInfest LifeStealerInfest;

    private readonly PhantomAssassinBlur PhantomAssassinBlur;

    private readonly BloodseekerRupture BloodseekerRupture;

    private readonly Hero LocalHero = EntityManager.LocalHero;

    public ModifierMonitor(Common common)
    {
        PartialMapHackMenu = common.MenuConfig.PartialMapHackMenu;

        Verification = common.Verification;

        SpiritBreakerCharge = common.SpiritBreakerCharge;
        LifeStealerInfest = common.LifeStealerInfest;
        PhantomAssassinBlur = common.PhantomAssassinBlur;
        BloodseekerRupture = common.BloodseekerRupture;

        ModifierManager.ModifierAdded += OnModifierAdded;
    }

    public void Dispose()
    {
        ModifierManager.ModifierAdded -= OnModifierAdded;
    }

    private void OnModifierAdded(ModifierAddedEventArgs e)
    {
        try
        {
            var modifier = e.Modifier;
            if (!modifier.IsValid)
            {
                return;
            }

            var entity = modifier.Owner;

            var unit = entity as Unit;

            var hero = unit as Hero;
            var isHero = hero != null;

            if (SpiritBreakerCharge.Modifier(unit, modifier, isHero))
            {
                return;
            }

            if (LifeStealerInfest.Modifier(unit, modifier, isHero))
            {
                return;
            }

            if (PhantomAssassinBlur.Modifier(unit, modifier))
            {
                return;
            }

            if (BloodseekerRupture.Modifier(unit, modifier))
            {
                return;
            }

            if (!PartialMapHackMenu.ModifersItem)
            {
                return;
            }

            if (!isHero)
            {
                return;
            }

            var modifierName = modifier.Name;

            if (ModifierDictionaries.Modifiers.TryGetValue(modifierName, out var sleepTime))
            {
                if (hero.IsAlly(LocalHero))
                {
                    return;
                }

                Modifier(hero, modifier, hero.Position, modifierName, sleepTime);
                return;
            }

            if (ModifierDictionaries.AllyModifiers.TryGetValue(modifierName, out var heroId))
            {
                if (hero.IsEnemy(LocalHero))
                {
                    return;
                }

                Modifier(hero, modifier, hero.Position, modifierName, 5000, heroId);
                return;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    private void Modifier(Hero hero, Modifier modifier, Vector3 position, string modifierName, int sleepTime, HeroId heroId = 0)
    {
        try
        {
            var abilityTextureName = modifier.TextureName;

            if (modifierName.StartsWith("modifier_rune"))
            {
                abilityTextureName = modifierName.Substring("modifier_".Length);
            }

            var heroTextureName = hero.Name;
            if (MultiSleeper<string>.IsSleeping($"Modifers_{abilityTextureName}_{heroTextureName}"))
            {
                return;
            }

            var player = hero.Player;
            if (player == null)
            {
                return;
            }

            Verification.ModifierVerification(position, heroTextureName, abilityTextureName, player.Id + 1, heroId);
            MultiSleeper<string>.Sleep($"Modifers_{abilityTextureName}_{heroTextureName}", sleepTime);
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }
}
