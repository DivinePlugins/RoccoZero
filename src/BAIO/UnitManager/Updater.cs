using System;
using System.Collections.Generic;
using System.Linq;

using BAIO.Core.Extensions;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.EventArgs;
using Divine.Extensions;
using Divine.Update;

namespace BAIO.UnitManager
{
    public class Updater : IDisposable
    {
        private readonly BaseHero _main;

        private readonly Dictionary<uint, UnitManager> allUnits = new Dictionary<uint, UnitManager>();

        private UpdateHandler Handler;

        public IEnumerable<UnitManager> AllUnits
        {
            get
            {
                return allUnits.Values.Where(x =>
                    x.Unit.IsValid && x.Unit.IsAlive && x.Unit.Team == _main.Owner.Team && (!(x.Unit is Hero) || x.Unit.IsIllusion) &&
                    x.Unit.IsControllable && x.Unit.CanMove());
            }
        }

        public Updater(BaseHero main)
        {
            _main = main;

            try
            {
                foreach (var unit in EntityManager.GetEntities<Unit>())
                {
                    if (unit.Team == main.Owner.Team && unit.IsControllable && unit.IsControllableByPlayer(EntityManager.LocalPlayer) &&
                        (!(unit is Hero) || unit.IsIllusion)
                        && unit.NetworkName != "CDOTA_BaseNPC_Clinkz_Skeleton_Army" && unit.NetworkName != "CDOTA_Unit_ZeusCloud"
                        && unit.NetworkName != "CDOTA_Unit_Courier" && unit.Name != "npc_dota_wraith_king_skeleton_warrior"  &&
                        unit.Name != "npc_dota_juggernaut_healing_ward" && !unit.Name.Contains("npc_dota_pugna_nether_ward")
                        && !unit.Name.Contains("npc_dota_shadow_shaman_ward")
                        && unit != EntityManager.LocalHero)
                    {
                        allUnits[unit.Handle] = new UnitManager(_main, unit);
                        //LogManager.Debug($"new unit found");
                    }
                }

                Handler = UpdateManager.CreateIngameUpdate(OnUpdate);
            }
            catch
            {
                //Ignore
            }

            EntityManager.EntityRemoved += OnEntityManagerOnEntityRemoved;
        }

        private void OnUpdate()
        {
            foreach (var unit in EntityManager.GetEntities<Unit>().Where(x =>
                x != null && x.IsValid && x.IsAlly(_main.Owner) && x.IsControllable &&
                x.IsControllableByPlayer(EntityManager.LocalPlayer)
                && (!(x is Hero) || x.IsIllusion)
                && x.NetworkName != "CDOTA_BaseNPC_Clinkz_Skeleton_Army" && x.NetworkName != "CDOTA_Unit_ZeusCloud"
                && x.NetworkName != "CDOTA_Unit_Courier" && x.Name != "npc_dota_wraith_king_skeleton_warrior" &&
                x.Name != "npc_dota_juggernaut_healing_ward"
                && !x.Name.Contains("npc_dota_pugna_nether_ward") && !x.Name.Contains("npc_dota_shadow_shaman_ward")
                && x != EntityManager.LocalHero)
            )
            {
                if (!allUnits.ContainsKey(unit.Handle))
                {
                    allUnits[unit.Handle] = new UnitManager(_main, unit);
                    //LogManager.Debug($"new unit added");
                }
            }
        }

        private void OnEntityManagerOnEntityRemoved(EntityRemovedEventArgs e)
        {
            var entity = e.Entity;
            if (entity is not Unit)
            {
                return;
            }

            var handle = entity.Handle;
            if (!allUnits.ContainsKey(handle))
            {
                return;
            }

            if (allUnits.ContainsKey(handle))
            {
                allUnits.Remove(handle);
            }
        }

        public void Dispose()
        {
            EntityManager.EntityRemoved -= OnEntityManagerOnEntityRemoved;
            UpdateManager.DestroyIngameUpdate(OnUpdate);
            allUnits.Clear();
        }
    }
}