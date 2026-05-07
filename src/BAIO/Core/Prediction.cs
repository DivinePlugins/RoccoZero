namespace BAIO.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using BAIO.Core.Extensions;

using Divine.Entity;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Update;

public class Prediction
{
    public static Dictionary<float, double> RotSpeedDictionary = new Dictionary<float, double>();

    public static Dictionary<float, float> RotTimeDictionary = new Dictionary<float, float>();

    public static Dictionary<float, Vector3> SpeedDictionary = new Dictionary<float, Vector3>();

    public static List<Prediction> TrackTable = new List<Prediction>();

    private static Dictionary<float, float> lastRotRDictionary = new Dictionary<float, float>();

    private static bool loaded;

    private static List<Hero> playerList = new List<Hero>();

    public Vector3 LastPosition;

    public float LastRotR;

    public float Lasttick;

    public float RotSpeed;

    public Vector3 Speed;

    public string UnitNetworkName;

    public string UnitName;

    static Prediction()
    {
        playerList = new List<Hero>();
        RotSpeedDictionary = new Dictionary<float, double>();
        RotTimeDictionary = new Dictionary<float, float>();
        SpeedDictionary = new Dictionary<float, Vector3>();
        TrackTable = new List<Prediction>();
        lastRotRDictionary = new Dictionary<float, float>();
        UpdateManager.Update += SpeedTrack;
    }

    public Prediction()
    {
    }

    public Prediction(
        string unitName,
        string unitNetworkName,
        Vector3 speed,
        float rotSpeed,
        Vector3 lastPosition,
        float lastRotR,
        float lasttick)
    {
        this.UnitName = unitName;
        this.UnitNetworkName = unitNetworkName;
        this.Speed = speed;
        this.RotSpeed = rotSpeed;
        this.LastPosition = lastPosition;
        this.LastRotR = lastRotR;
        this.Lasttick = lasttick;
    }

    public static bool AbilityMove(Unit unit)
    {
        return
            unit.HasModifiers(
                new[]
                    {
                            "modifier_spirit_breaker_charge_of_darkness", "modifier_earth_spirit_boulder_smash",
                            "modifier_earth_spirit_rolling_boulder_caster", "modifier_earth_spirit_geomagnetic_grip",
                            "modifier_spirit_breaker_charge_of_darkness", "modifier_huskar_life_break_charge",
                            "modifier_magnataur_skewer_movement", "modifier_storm_spirit_ball_lightning",
                            "modifier_faceless_void_time_walk", "modifier_mirana_leap", "modifier_slark_pounce"
                    },
                false);
    }

    public static float CalculateReachTime(Unit target, float speed, Vector3 dePos)
    {
        Vector3 targetSpeed;
        if ((!SpeedDictionary.TryGetValue(target.Handle, out targetSpeed) || targetSpeed == new Vector3(0, 0, 0))
            && target.NetworkActivity == NetworkActivity.Move)
        {
            var rotSpeed = 0d;
            if (RotSpeedDictionary.ContainsKey(target.Handle))
            {
                rotSpeed = RotSpeedDictionary[target.Handle];
            }

            targetSpeed = target.Vector3FromPolarAngle((float)rotSpeed) * target.MovementSpeed / 1000;
        }

        var a = Math.Pow(targetSpeed.X, 2) + Math.Pow(targetSpeed.Y, 2) - Math.Pow(speed / 1000, 2);
        var b = 2 * (dePos.X * targetSpeed.X + dePos.Y * targetSpeed.Y);
        var c = Math.Pow(dePos.X, 2) + Math.Pow(dePos.Y, 2);
        return (float)((-b - Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a));
    }


    public static Vector3 InFront(Unit unit, float distance)
    {
        var v = unit.Position + unit.Vector3FromPolarAngle() * distance;
        return new Vector3(v.X, v.Y, 0);
    }

    public static bool IsIdle(Unit unit)
    {
        return unit.NetworkActivity != NetworkActivity.Move;
    }

    public static bool IsTurning(Unit unit, double tolerancy = 0)
    {
        double rotSpeed;
        if (!RotSpeedDictionary.TryGetValue(unit.Handle, out rotSpeed))
        {
            return false;
        }

        return Math.Abs(rotSpeed) > tolerancy;
    }

    public static Vector3 PredictedXYZ(Unit unit, float delay)
    {
        if (IsIdle(unit))
        {
            return unit.Position;
        }

        var targetSpeed = new Vector3();
        if (!lastRotRDictionary.ContainsKey(unit.Handle))
        {
            lastRotRDictionary.Add(unit.Handle, unit.RotationRad);
        }

        var straightTime = StraightTime(unit);
        if (straightTime > 180)
        {
            lastRotRDictionary[unit.Handle] = unit.RotationRad;
        }

        lastRotRDictionary[unit.Handle] = unit.RotationRad;
        if ((unit.NetworkName == "CDOTA_Unit_Hero_StormSpirit" || unit.NetworkName == "CDOTA_Unit_Hero_Rubick")
            && unit.HasModifier("modifier_storm_spirit_ball_lightning"))
        {
            var ballLightning = unit.FindSpell("storm_spirit_ball_lightning", true);
            var firstOrDefault =
                ballLightning.AbilitySpecialData.FirstOrDefault(x => x.Name == "ball_lightning_move_speed");
            if (firstOrDefault != null)
            {
                var ballSpeed = firstOrDefault.GetValue(ballLightning.Level - 1);
                var newpredict = unit.Vector3FromPolarAngle() * (ballSpeed / 1000f);
                targetSpeed = newpredict;
            }
        }
        else
        {
            targetSpeed = unit.Vector3FromPolarAngle() * (unit.MovementSpeed / 1000f);
        }

        var v = unit.Position + targetSpeed * delay;
        return new Vector3(v.X, v.Y, 0);
    }

    public static Vector3 SkillShotXYZ(Unit source, Unit target, float delay, float speed, float radius)
    {
        if (IsIdle(target))
        {
            return target.Position;
        }

        if (!(speed < 6000) || speed <= 0)
        {
            return PredictedXYZ(target, delay);
        }

        var predict = PredictedXYZ(target, delay);
        var sourcePos = source.Position;
        var reachTime = CalculateReachTime(target, speed, predict - sourcePos);
        predict = PredictedXYZ(target, delay + reachTime);
        if (!(source.Distance2D(predict) > radius))
        {
            return PredictedXYZ(target, delay + reachTime);
        }

        if (target.MovementSpeed * ((predict.Distance2D(sourcePos) - radius) / speed) < radius)
        {
            sourcePos = (sourcePos - predict) * (sourcePos.Distance2D(predict) - radius)
                        / sourcePos.Distance2D(predict) + predict;
            reachTime = CalculateReachTime(target, speed, predict - sourcePos);
        }
        else
        {
            sourcePos = (sourcePos - predict)
                        * (sourcePos.Distance2D(predict)
                           + target.MovementSpeed * ((predict.Distance2D(sourcePos) - radius) / speed) - radius)
                        / sourcePos.Distance2D(predict) + predict;
            reachTime = CalculateReachTime(target, speed, predict - sourcePos);
        }

        return PredictedXYZ(target, delay + reachTime);
    }

    public static void SpeedTrack()
    {
        if (!GameManager.IsInGame || GameManager.IsPaused)
        {
            return;
        }

        var me = EntityManager.LocalHero;
        if (me == null)
        {
            return;
        }

        if (!Utils.SleepCheck("Prediction.SpeedTrack.Sleep"))
        {
            return;
        }

        if (playerList == null || playerList.Count < 10 && Utils.SleepCheck("Prediction.SpeedTrack"))
        {
            playerList = EntityManager.GetEntities<Hero>().ToList();
            Utils.Sleep(1000, "Prediction.SpeedTrack");
        }

        if (!playerList.Any())
        {
            return;
        }

        Utils.Sleep(70, "Prediction.SpeedTrack.Sleep");
        var tick = Utils.TickCount;
        var tempTable = new List<Prediction>(TrackTable);
        foreach (var unit in playerList)
        {
            if (!unit.IsValid)
            {
                continue;
            }

            var data =
                tempTable.FirstOrDefault(
                    unitData => unitData.UnitName == unit.Name || unitData.UnitNetworkName == unit.NetworkName);
            if (data == null && unit.IsAlive && unit.IsVisible)
            {
                data = new Prediction(
                    unit.Name,
                    unit.NetworkName,
                    new Vector3(0, 0, 0),
                    0,
                    new Vector3(0, 0, 0),
                    0,
                    0);
                TrackTable.Add(data);
            }

            if (data != null && (!unit.IsAlive || !unit.IsVisible))
            {
                data.LastPosition = new Vector3(0, 0, 0);
                data.LastRotR = 0;
                data.Lasttick = 0;
                continue;
            }

            if (data == null || data.LastPosition != new Vector3(0, 0, 0) && !(tick - data.Lasttick > 0))
            {
                continue;
            }

            if (data.LastPosition == new Vector3(0, 0, 0))
            {
                data.LastPosition = unit.Position;
                data.LastRotR = unit.RotationRad;
                data.Lasttick = tick;
            }
            else
            {
                data.RotSpeed = data.LastRotR - unit.RotationRad;
                if (!RotTimeDictionary.ContainsKey(unit.Handle))
                {
                    RotTimeDictionary.Add(unit.Handle, tick);
                }

                if (!lastRotRDictionary.ContainsKey(unit.Handle))
                {
                    lastRotRDictionary.Add(unit.Handle, unit.RotationRad);
                }

                var speed = (unit.Position - data.LastPosition) / (tick - data.Lasttick);
                if (Math.Abs(data.RotSpeed) > 0.18 && data.Speed != new Vector3(0, 0, 0))
                {
                    data.Speed = speed;
                    RotTimeDictionary[unit.Handle] = tick;
                }
                else
                {
                    lastRotRDictionary[unit.Handle] = unit.RotationRad;
                    data.Speed = speed;
                }

                data.LastPosition = unit.Position;
                data.LastRotR = unit.RotationRad;
                data.Lasttick = tick;
                if (!SpeedDictionary.ContainsKey(unit.Handle))
                {
                    SpeedDictionary.Add(unit.Handle, data.Speed);
                }
                else
                {
                    SpeedDictionary[unit.Handle] = data.Speed;
                }

                if (!RotSpeedDictionary.ContainsKey(unit.Handle))
                {
                    RotSpeedDictionary.Add(unit.Handle, data.RotSpeed);
                }
                else
                {
                    RotSpeedDictionary[unit.Handle] = data.RotSpeed;
                }
            }
        }
    }

    public static float StraightTime(Unit unit)
    {
        if (!RotTimeDictionary.ContainsKey(unit.Handle))
        {
            return 0;
        }

        return Utils.TickCount - RotTimeDictionary[unit.Handle] + GameManager.Ping;
    }
}