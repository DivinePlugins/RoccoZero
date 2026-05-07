namespace BeAware.Entities;

using System;
using System.Linq;

using BeAware.Data;
using BeAware.Helpers;
using BeAware.MenuManager.PartialMapHack;
using BeAware.ShowMeMore.MoreInformation;

using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Numerics;
using Divine.Particle;
using Divine.Particle.EventArgs;
using Divine.Update;
using Divine.Common.Log;

internal sealed class ParticleMonitor
{
    private readonly PartialMapHackMenu PartialMapHackMenu;

    private readonly Verification Verification;

    private readonly InvokerEMP InvokerEMP;

    private readonly PudgeHook PudgeHook;

    private readonly AncientApparitionIceBlast AncientApparitionIceBlast;

    private readonly WindrunnerPowershot WindrunnerPowershot;

    private readonly MiranaArrow MiranaArrow;

    private readonly Hero LocalHero = EntityManager.LocalHero;

    private readonly static Log Log = LogManager.GetCurrentClassLogger();

    private Hero blinkHeroEnd;

    public ParticleMonitor(Common common)
    {
        PartialMapHackMenu = common.MenuConfig.PartialMapHackMenu;

        Verification = common.Verification;

        InvokerEMP = common.InvokerEMP;
        PudgeHook = common.PudgeHook;
        AncientApparitionIceBlast = common.AncientApparitionIceBlast;
        WindrunnerPowershot = common.WindrunnerPowershot;
        MiranaArrow = common.MiranaArrow;

        ParticleManager.ParticleAdded += OnParticleAdded;
    }

    public void Dispose()
    {
        ParticleManager.ParticleAdded -= OnParticleAdded;
    }

    private static Hero FindHeroWithSpells(AbilityId abilityId)
    {
        return EntityManager.GetEntities<Hero>().FirstOrDefault(x => !x.IsIllusion && x.Spellbook.Spells.Any(v => v.Id == abilityId));
    }

    private static Hero FindHeroWithItems(params AbilityId[] Ids)
    {
        return EntityManager.GetEntities<Hero>().FirstOrDefault(x => !x.IsIllusion && x.Inventory.Items.Any(i => Ids.Contains(i.Id) && i.Cooldown / i.CooldownLength * 100 >= 99));
    }

    private void OnParticleAdded(ParticleAddedEventArgs e)
    {
        try
        {
            var particle = e.Particle;
            if (!particle.IsValid)
            {
                return;
            }

            var name = particle.Name;
            var entity = particle.Owner;

            if (ParticleCorrection.IgnoreParticles.Any(x => name.Contains(x)))
            {
                return;
            }

            if (name.Contains("generic_hit_blood"))
            {
                var isRoshan = entity.NetworkName == "CDOTA_Unit_Roshan";
                if (!isRoshan && entity.NetworkName != "CDOTA_BaseNPC_Creep_Neutral")
                {
                    return;
                }

                UpdateManager.BeginInvoke(() =>
                {
                    if (!particle.IsValid)
                    {
                        return;
                    }

                    JungleScan(entity, isRoshan, particle.GetControlPoint(0));
                });
            }

            if (AncientApparitionIceBlast.Particle(particle, name))
            {
                return;
            }

            if (InvokerEMP.Particle(particle, name))
            {
                return;
            }

            if (PudgeHook.Particle(particle, name))
            {
                return;
            }

            if (WindrunnerPowershot.Particle(particle, name))
            {
                return;
            }

            if (MiranaArrow.Particle(particle, name))
            {
                return;
            }

            if (PartialMapHackMenu.SpellsItem)
            {
                var spellsCP0 = ParticleDictionaries.CP0.FirstOrDefault(x => name.Contains(x.Key));
                if (!string.IsNullOrEmpty(spellsCP0.Key))
                {
                    UpdateManager.BeginInvoke(() =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        Spells(spellsCP0.Value, name, particle.GetControlPoint(0));
                    });

                    return;
                }

                var spellsCP1 = ParticleDictionaries.CP1.FirstOrDefault(x => name.Contains(x.Key));
                if (!string.IsNullOrEmpty(spellsCP1.Key))
                {
                    UpdateManager.BeginInvoke(() =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        Spells(spellsCP1.Value, name, particle.GetControlPoint(1));
                    });

                    return;
                }

                var spellsCP2 = ParticleDictionaries.CP2.FirstOrDefault(x => name.Contains(x.Key));
                if (!string.IsNullOrEmpty(spellsCP2.Key))
                {
                    UpdateManager.BeginInvoke(() =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        Spells(spellsCP2.Value, name, particle.GetControlPoint(2));
                    });

                    return;
                }

                var spellsCP5 = ParticleDictionaries.CP5.FirstOrDefault(x => name.Contains(x.Key));
                if (!string.IsNullOrEmpty(spellsCP5.Key))
                {
                    UpdateManager.BeginInvoke(() =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        Spells(spellsCP5.Value, name, particle.GetControlPoint(5));
                    });

                    return;
                }

                var spellsCP1Plus = ParticleDictionaries.CP1Plus.FirstOrDefault(x => name.Contains(x.Key));
                if (!string.IsNullOrEmpty(spellsCP1Plus.Key))
                {
                    UpdateManager.BeginInvoke(() =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        var position = particle.GetControlPoint(1);
                        if (position.ToVector2() == particle.GetControlPoint(0).ToVector2())
                        {
                            return;
                        }

                        Spells(spellsCP1Plus.Value, name, position);
                    });

                    return;
                }
            }

            if (PartialMapHackMenu.ItemsItem)
            {
                var itemsCP1 = ParticleDictionaries.ItemsCP1.FirstOrDefault(x => name.Contains(x.Key));
                if (!string.IsNullOrEmpty(itemsCP1.Key))
                {
                    UpdateManager.BeginInvoke(1, () =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        Items(entity as Hero, itemsCP1.Value, name, particle.GetControlPoint(1));
                    });

                    return;
                }

                if (name.Contains("blink_dagger_end"))
                {
                    blinkHeroEnd = entity as Hero;
                    return;
                }

                if (name.Contains("blink_dagger_start"))
                {
                    UpdateManager.BeginInvoke(() =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        Items(blinkHeroEnd, AbilityId.item_blink, name, particle.GetControlPoint(0));
                    });

                    return;
                }

                if (name.Contains("refresher"))
                {
                    UpdateManager.BeginInvoke(1, () =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        Items(entity as Hero, AbilityId.item_refresher, name, particle.GetControlPoint(0));
                    });

                    return;
                }

                if (name.Contains("battlefury_cleave"))
                {
                    UpdateManager.BeginInvoke(1, () =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        var position = particle.GetControlPoint(1);
                        if (position.ToVector2() == particle.GetControlPoint(0).ToVector2())
                        {
                            return;
                        }

                        Items(entity as Hero, AbilityId.item_bfury, name, position);
                    });

                    return;
                }

                var itemsSemiNullCP0 = ParticleDictionaries.ItemsSemiNullCP0.FirstOrDefault(x => name.Contains(x.Key));
                if (!string.IsNullOrEmpty(itemsSemiNullCP0.Key))
                {
                    UpdateManager.BeginInvoke(1, () =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        var abilityId = itemsSemiNullCP0.Value;
                        Items(FindHeroWithItems(abilityId), abilityId, name, particle.GetControlPoint(0));
                    });

                    return;
                }

                var itemsSemiNullCP1 = ParticleDictionaries.ItemsSemiNullCP1.FirstOrDefault(x => name.Contains(x.Key));
                if (!string.IsNullOrEmpty(itemsSemiNullCP1.Key))
                {
                    UpdateManager.BeginInvoke(1, () =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        var abilityId = itemsSemiNullCP1.Value;
                        Items(FindHeroWithItems(abilityId), abilityId, name, particle.GetControlPoint(1));
                    });

                    return;
                }

                if (name.Contains("dagon.vpcf"))
                {
                    UpdateManager.BeginInvoke(1, () =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        var abilityId = ParticleCorrection.DagonId;
                        Items(FindHeroWithItems(abilityId), abilityId.First(), name, particle.GetControlPoint(1));
                    });

                    return;
                }
            }

            if (PartialMapHackMenu.TeleportItem)
            {
                if (name.Contains("/teleport_start") || name.Contains("/teleport_end"))
                {
                    UpdateManager.BeginInvoke(() =>
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        var particleColor = particle.GetControlPoint(2);
                        var position = particle.GetControlPoint(0);
                        if (particleColor.IsZero || position.IsZero)
                        {
                            return;
                        }

                        Teleport(AbilityId.item_tpscroll, name, position, particleColor);
                    });
                }
            }

            if (PartialMapHackMenu.SmokeItem)
            {
                if (name.Contains("smoke_of_deceit.vpcf"))
                {
                    UpdateManager.BeginInvoke(() =>
                    {
                        if (EntityManager.GetEntities<Hero>().Any(x => x.IsAlly(LocalHero) && x.HasModifier("modifier_smoke_of_deceit")))
                        {
                            return;
                        }

                        if (!particle.IsValid)
                        {
                            return;
                        }

                        Items(null, AbilityId.item_smoke_of_deceit, name, particle.GetControlPoint(0));
                    });

                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    private void JungleScan(Entity sender, bool isRoshan, Vector3 position)
    {
        try
        {
            if (sender.IsVisible)
            {
                return;
            }

            Verification.JungleVerification(position, isRoshan);
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    private void Spells(AbilityId abilityId, string particleName, Vector3 position)
    {
        try
        {
            if (position.IsZero)
            {
                return;
            }

            var isDangerousSpell = DangerousAbility.DangerousSpells.Contains(abilityId);
            var hero = FindHeroWithSpells(abilityId);
            if (hero == null)
            {
                if (abilityId == AbilityId.viper_poison_attack)
                {
                    return;
                }

                Verification.EntityVerification(position, "npc_dota_hero_default", abilityId, 0, isDangerousSpell, particleName);
                return;
            }

            if (hero.IsAlly(LocalHero))
            {
                return;
            }

            if (!hero.IsVisible || PartialMapHackMenu.WhenIsVisibleItem && isDangerousSpell)
            {
                Verification.EntityVerification(position, hero.Name, abilityId, hero.Player.Id + 1, isDangerousSpell, particleName);
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    private void Items(Hero hero, AbilityId abilityId, string particleName, Vector3 position)
    {
        try
        {
            if (position.IsZero)
            {
                return;
            }

            var isDangerousItem = DangerousAbility.DangerousItems.Contains(abilityId);
            if (hero == null)
            {
                Verification.EntityVerification(position, "npc_dota_hero_default", abilityId, 0, isDangerousItem);
                return;
            }

            if (hero.IsAlly(LocalHero))
            {
                return;
            }

            if (!hero.IsVisible || PartialMapHackMenu.WhenIsVisibleItem && isDangerousItem)
            {
                Verification.EntityVerification(position, hero.Name, abilityId, hero.Player.Id + 1, isDangerousItem);
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    private void Teleport(AbilityId abilityId, string particleName, Vector3 position, Vector3 particleColor)
    {
        try
        {
            var playerId = Colors.HeroColors.FindIndex(x => x == particleColor);
            var hero = EntityManager.GetPlayerById(playerId)?.Hero;
            if (hero == null)
            {
                Verification.EntityVerification(position, "npc_dota_hero_default", abilityId, playerId + 1, true, particleName);
                return;
            }

            if (hero.IsAlly(LocalHero))
            {
                return;
            }

            Verification.EntityVerification(position, hero.Name, abilityId, playerId + 1, true, particleName);
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }
}