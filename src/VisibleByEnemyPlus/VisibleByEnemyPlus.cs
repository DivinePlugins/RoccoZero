namespace VisibleByEnemyPlus;

using System;
using System.Collections.Generic;

using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.EventArgs;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Entity.Entities.Units.Creeps.Neutrals;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Wards;
using Divine.Entity.EventArgs;
using Divine.Extensions;
using Divine.Helpers;
using Divine.Numerics;
using Divine.Particle;
using Divine.Particle.Components;
using Divine.Service;
using Divine.Update;

public class VisibleByEnemyPlus : Bootstrapper
{
    private readonly HashSet<Unit> Units = new();

    private Config Config { get; set; }

    private int Red => Config.RedItem;

    private int Green => Config.GreenItem;

    private int Blue => Config.BlueItem;

    private int Alpha => Config.AlphaItem;

    private readonly Sleeper Sleeper = new();

    private Hero LocalHero;

    protected override void OnActivate()
    {
        Config = new Config();

        LocalHero = EntityManager.LocalHero;

        Config.EffectTypeItem.ValueChanged += (selector, e) => { UpdateMenu(e.NewValue, Red, Green, Blue, Alpha); };
        Config.RedItem.ValueChanged += (slider, e) => { UpdateMenu(Config.EffectTypeItem, e.NewValue, Green, Blue, Alpha); };
        Config.GreenItem.ValueChanged += (slider, e) => { UpdateMenu(Config.EffectTypeItem, Red, e.NewValue, Blue, Alpha); };
        Config.BlueItem.ValueChanged += (slider, e) => { UpdateMenu(Config.EffectTypeItem, Red, Green, e.NewValue, Alpha); };
        Config.AlphaItem.ValueChanged += (slider, e) => { UpdateMenu(Config.EffectTypeItem, Red, Green, Blue, e.NewValue); };

        Entity.NetworkPropertyChanged += OnNetworkPropertyChanged;

        Config.HeroesItem.ValueChanged += (_, _) => OnRefresh();
        Config.CouriersItem.ValueChanged += (_, _) => OnRefresh();
        Config.WardsItem.ValueChanged += (_, _) => OnRefresh();
        Config.MinesItem.ValueChanged += (_, _) => OnRefresh();
        Config.TowersItem.ValueChanged += (_, _) => OnRefresh();
        Config.OutpostsItem.ValueChanged += (_, _) => OnRefresh();
        Config.WatchersItem.ValueChanged += (_, _) => OnRefresh();
        Config.TormentorsItem.ValueChanged += (_, _) => OnRefresh();
        Config.BuildingsItem.ValueChanged += (_, _) => OnRefresh();
        Config.NeutralsItem.ValueChanged += (_, _) => OnRefresh();
        Config.CreepsItem.ValueChanged += (_, _) => OnRefresh();
        Config.OthersItem.ValueChanged += (_, _) => OnRefresh();

        OnRefresh();
    }

    private void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (e.PropertyName is not "m_iTaggedAsVisibleByTeam")
        {
            return;
        }

        UpdateManager.BeginInvoke(() =>
        {
            if (sender is not Unit unit || !sender.IsValid)
            {
                return;
            }

            HandleEffect(unit, unit.IsVisibleToEnemies);
        });
    }

    private void OnRefresh()
    {
        if (Sleeper.IsSleeping)
        {
            return;
        }

        Sleeper.Sleep(50);

        UpdateManager.BeginInvoke(() =>
        {
            foreach (var unit in EntityManager.GetEntities<Unit>())
            {
                ParticleManager.DestroyParticle($"VisibleByEnemyPlus.{unit.Handle}");
                HandleEffect(unit, unit.IsVisibleToEnemies);
            }
        });
    }

    protected override void OnDeactivate()
    {
        /*UpdateManager.Unsubscribe(LoopEntities);

        Config.EffectTypeItem.PropertyChanged -= ItemChanged;

        Config.RedItem.PropertyChanged -= ItemChanged;
        Config.GreenItem.PropertyChanged -= ItemChanged;
        Config.BlueItem.PropertyChanged -= ItemChanged;
        Config.AlphaItem.PropertyChanged -= ItemChanged;

        Config?.Dispose();
        ParticleManager.Dispose();*/
    }

    private void UpdateMenu(string selector, int red, int green, int blue, int alpha)
    {
        if (selector == "Default")
        {
            Config.RedItem.SetFontColor(Color.Black);
            Config.GreenItem.SetFontColor(Color.Black);
            Config.BlueItem.SetFontColor(Color.Black);
            Config.AlphaItem.SetFontColor(Color.Black);
        }
        else
        {
            Config.RedItem.SetFontColor(new Color(red, 0, 0, 255));
            Config.GreenItem.SetFontColor(new Color(0, green, 0, 255));
            Config.BlueItem.SetFontColor(new Color(0, 0, blue, 255));
            Config.AlphaItem.SetFontColor(new Color(185, 176, 163, alpha));
        }

        OnRefresh();

        //UpdateManager.BeginInvoke(100, () =>
        //{
        //    HandleEffect(LocalHero, true);
        //});
    }

    private void HandleEffect(Unit unit, bool visible)
    {
        var filter = unit switch
        {
            Hero => Config.HeroesItem && unit.IsAlly(LocalHero),
            Courier => Config.CouriersItem && unit.IsAlly(LocalHero),
            Ward => Config.WardsItem && unit.IsAlly(LocalHero),
            Tower => Config.TowersItem && unit.IsAlly(LocalHero),
            Outpost => Config.OutpostsItem,
            Building { ClassId: ClassId.CDOTA_NPC_Lantern} => Config.WatchersItem,
            Building { ClassId: ClassId.CDOTA_Unit_Miniboss } => Config.TormentorsItem,
            Building => Config.BuildingsItem && unit.IsAlly(LocalHero),
            Neutral => Config.NeutralsItem,
            Creep => Config.CreepsItem && unit.IsAlly(LocalHero),
            Unit { ClassId: ClassId.CDOTA_NPC_TechiesMines } => Config.MinesItem && unit.IsAlly(LocalHero),
            Unit { IsControllable: true } => Config.OthersItem && unit.IsAlly(LocalHero),
            _ => false,
        };

        //Console.WriteLine(unit + "  " + visible + "  " + filter);

        if (!filter)
        {
            return;
        }

        if (visible && unit.IsAlive)
        {
            ParticleManager.CreateParticle(
                $"VisibleByEnemyPlus.{unit.Handle}",
                Config.Effects[Config.EffectTypeItem],
                Attachment.AbsOriginFollow,
                unit,
                new ControlPoint(1, Red, Green, Blue),
                new ControlPoint(2, Alpha));
        }
        else
        {
            ParticleManager.DestroyParticle($"VisibleByEnemyPlus.{unit.Handle}");
        }
    }
}