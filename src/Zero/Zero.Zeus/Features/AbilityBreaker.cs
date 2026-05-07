namespace Divine.Zeus.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.EventArgs;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Particle;
using Divine.Particle.EventArgs;
using Divine.Update;
using Divine.Common.Log;
using Divine.Zeus.Menus;

internal sealed class AbilityBreaker : BaseTaskHandler
{
    private readonly AbilityBreakerMenu AbilityBreakerMenu;

    private readonly Abilities Abilities;

    public AbilityBreaker(Common common)
    {
        AbilityBreakerMenu = ((MoreMenu)common.MenuConfig.MoreMenu).AbilityBreakerMenu;

        Abilities = (Abilities)common.Abilities;

        AbilityBreakerMenu.EnableItem.ValueChanged += EnableChanged;
    }

    public override void Dispose()
    {
        base.Dispose();

        AbilityBreakerMenu.EnableItem.ValueChanged += EnableChanged;

        if (AbilityBreakerMenu.EnableItem.Value)
        {
            ParticleManager.ParticleAdded -= OnParticleAdded;
            Entity.NetworkPropertyChanged -= OnNetworkPropertyChanged;
        }
    }

    private void EnableChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            RunAsync();

            ParticleManager.ParticleAdded += OnParticleAdded;
            Entity.NetworkPropertyChanged += OnNetworkPropertyChanged;
        }
        else
        {
            ParticleManager.ParticleAdded -= OnParticleAdded;
            Entity.NetworkPropertyChanged -= OnNetworkPropertyChanged;

            Cancel();
        }
    }

    private void OnParticleAdded(ParticleAddedEventArgs e)
    {
        var particle = e.Particle;
        var name = particle.Name;
        var isTeleort = name.Contains("/teleport_start");
        if (!isTeleort && !name.Contains("sandking_epicenter_tell"))
        {
            return;
        }

        UpdateManager.BeginInvoke(20, () =>
        {
            var position = particle.GetControlPoint(0);
            var hero = UnitManager<CHero>.Units.FirstOrDefault(x => x.IsVisible && x.IsAlive && x.Distance2D(position) < 150);
            if (hero != null && hero.IsAlly())
            {
                return;
            }

            if (isTeleort && hero == null && AbilityBreakerMenu.TeleportVisibleItem)
            {
                return;
            }

            var random = new Random().Next(int.MaxValue);

            var distance = Owner.Distance2D(position);
            var pictureStates = AbilityBreakerMenu.AbilitiesSelection;
            var isMenuRange = distance < AbilityBreakerMenu.RangeItem.Value || AbilityBreakerMenu.FullRangeItem;

            if (isTeleort)
            {
                pictureStates = AbilityBreakerMenu.TeleportAbilitiesSelection;
                isMenuRange = distance < AbilityBreakerMenu.TeleportRangeItem.Value || AbilityBreakerMenu.TeleportFullRangeItem;
            }

            AbilityData.Add(new Data(random, position, pictureStates.Values, isMenuRange, AbilityBreakerMenu.TeleportVisibleItem));

            UpdateManager.BeginInvoke(3000, () =>
            {
                AbilityData.RemoveAll(x => x.Id == random);
            });
        });
    }

    private static AbilityId[] IsChannelingAbilities { get; } =
    {
        AbilityId.bane_fiends_grip,
        AbilityId.witch_doctor_death_ward,
        AbilityId.crystal_maiden_freezing_field,
        AbilityId.enigma_black_hole,
        AbilityId.shadow_shaman_shackles
    };

    private void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (e.PropertyName != "m_flChannelStartTime")
        {
            return;
        }

        if (e.NewValue.GetSingle() <= 0)
        {
            return;
        }

        UpdateManager.BeginInvoke(() =>
        {
            var ability = sender as Ability;
            if (ability == null)
            {
                return;
            }

            var abilityId = ability.Id;
            if (!IsChannelingAbilities.Any(x => x == abilityId))
            {
                return;
            }

            var hero = sender.Owner as Hero;
            if (hero == null || hero.IsAlly(Owner.Base))
            {
                return;
            }

            var position = hero.Position;
            var pictureStates = AbilityBreakerMenu.AbilitiesSelection.Values;
            var isMenuRange = Owner.Distance2D(position) < AbilityBreakerMenu.RangeItem.Value || AbilityBreakerMenu.FullRangeItem;
            AbilityData.Add(new Data((int)abilityId, position, pictureStates, isMenuRange, false));

            UpdateManager.BeginInvoke(20, async () =>
            {
                while (hero.IsVisible && ability.IsChanneling)
                {
                    await Task.Delay(50);
                }

                AbilityData.RemoveAll(x => x.Id == (int)abilityId);
            });
        });
    }

    protected async override Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            if (IsStopped)
            {
                return;
            }

            foreach (var data in AbilityData)
            {
                var position = data.Position;
                var pictureStates = data.PictureStates;
                var isMenuRange = data.IsMenuRange;

                // LightningBolt
                var lightningBolt = Abilities.LightningBolt;
                if (pictureStates[lightningBolt.Id]
                    && Owner.Distance2D(position) < lightningBolt.CastRange + 100
                    && lightningBolt.CanBeCasted)
                {
                    lightningBolt.UseAbility(position);
                    await Task.Delay(lightningBolt.GetCastDelay(position), token);
                    return;
                }

                // Nimbus
                var nimbus = Abilities.Nimbus;
                if (pictureStates[nimbus.Id]
                    && Owner.Distance2D(position) > lightningBolt.CastRange + 100
                    && isMenuRange
                    && nimbus.CanBeCasted)
                {
                    nimbus.UseAbility(position);
                    await Task.Delay(nimbus.GetCastDelay(position), token);
                }
            }
        }
        catch (TaskCanceledException)
        {
            // canceled
        }
        catch (Exception e)
        {
            LogManager.Error(e);
        }
    }

    private List<Data> AbilityData { get; } = new List<Data>();

    private class Data
    {
        public int Id { get; }

        public Vector3 Position { get; }

        public IReadOnlyDictionary<AbilityId, bool> PictureStates { get; }

        public bool IsMenuRange { get; }

        public bool IsTeleportVisibleItem { get; }

        public Data(int id, Vector3 position, IReadOnlyDictionary<AbilityId, bool> pictureStates, bool isMenuRange, bool isTeleportVisibleItem)
        {
            Id = id;
            Position = position;
            PictureStates = pictureStates;
            IsMenuRange = isMenuRange;
            IsTeleportVisibleItem = isTeleportVisibleItem;
        }
    }
}
