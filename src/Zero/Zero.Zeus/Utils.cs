using System.Linq;

using Divine.Core.Entities;
using Divine.Core.Entities.Abilities.Spells.Zeus;
using Divine.Core.Managers.Unit;
using Divine.Extensions;
using Divine.Numerics;
using Divine.Prediction;

namespace Divine.Zeus
{
    internal static class Utils
    {
        public static Vector3 LightningBoltPrediction(this LightningBolt lightningBolt, CUnit target, bool isLightningBoltOnPosition)
        {
            if (!target.IsVisible && !isLightningBoltOnPosition)
            {
                return Vector3.Zero;
            }

            var castRange = lightningBolt.CastRange;
            var owner = lightningBolt.Owner;
            var ownerPosition = owner.Position;

            var input = new PredictionInput
            {
                Owner = owner.Base,
                Delay = lightningBolt.CastPoint + lightningBolt.ActivationDelay,
            };

            var position = PredictionManager.GetPrediction(input.WithTarget(target.Base)).CastPosition;
            if (ownerPosition.Distance(position) < castRange + 280)
            {
                if (ownerPosition.Distance(position) > castRange)
                {
                    var distance = ownerPosition.Extend(position, castRange).Distance(position);
                    position = position.Extend(ownerPosition, distance);
                }

                var unit = UnitManager<CUnit, Enemy>.Units.Where(x => x.IsAlive && !(x is CBuilding) && position.Distance(x.Position) < 325)
                                                          .OrderBy(x => position.Distance(x.Position))
                                                          .FirstOrDefault();

                if (unit != target)
                {
                    return Vector3.Zero;
                }

                return position;
            }

            return Vector3.Zero;
        }
    }
}
