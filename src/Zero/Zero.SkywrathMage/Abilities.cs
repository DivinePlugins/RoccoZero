using Divine.Core.ComboFactory.Helpers;
using Divine.Core.Entities.Abilities.Items;
using Divine.Core.Entities.Abilities.Spells.SkywrathMage;
using Divine.Core.Managers;

namespace Divine.SkywrathMage
{
    internal sealed class Abilities : BaseAbilities
    {
        public Abilities()
        {
            ActionManager.ActionAdd<ArcaneBolt>(x => ArcaneBolt = x);
            ActionManager.ActionAdd<ConcussiveShot>(x => ConcussiveShot = x);
            ActionManager.ActionAdd<AncientSeal>(x => AncientSeal = x);
            ActionManager.ActionAdd<MysticFlare>(x => MysticFlare = x);

            ActionManager.ActionAdd<ScytheOfVyse>(x => Hex = x);
            ActionManager.ActionAdd<OrchidMalevolence>(x => Orchid = x);
            ActionManager.ActionAdd<Bloodthorn>(x => Bloodthorn = x);
            ActionManager.ActionAdd<RodOfAtos>(x => Atos = x);
            ActionManager.ActionAdd<Gleipnir>(x => Gleipnir = x);
            ActionManager.ActionAdd<VeilOfDiscord>(x => Veil = x);
            ActionManager.ActionAdd<EtherealBlade>(x => Ethereal = x);
            ActionManager.ActionAdd<Dagon>(x => Dagon = x);
            ActionManager.ActionAdd<ForceStaff>(x => ForceStaff = x);
            ActionManager.ActionAdd<EulsScepterOfDivinity>(x => Eul = x);
            ActionManager.ActionAdd<WindWaker>(x => WindWaker = x);
            ActionManager.ActionAdd<BlinkDagger>(x => Blink = x);
            ActionManager.ActionAdd<ShivasGuard>(x => Shivas = x);
            ActionManager.ActionAdd<Nullifier>(x => Nullifier = x);
            ActionManager.ActionAdd<UrnOfShadows>(x => Urn = x);
            ActionManager.ActionAdd<SpiritVessel>(x => Vessel = x);
        }

        public ArcaneBolt ArcaneBolt { get; private set; }

        public ConcussiveShot ConcussiveShot { get; private set; }

        public AncientSeal AncientSeal { get; private set; }

        public MysticFlare MysticFlare { get; private set; }

        public ScytheOfVyse Hex { get; private set; }

        public OrchidMalevolence Orchid { get; private set; }

        public Bloodthorn Bloodthorn { get; private set; }

        public RodOfAtos Atos { get; private set; }

        public Gleipnir Gleipnir { get; private set; }

        public VeilOfDiscord Veil { get; private set; }

        public EtherealBlade Ethereal { get; private set; }

        public Dagon Dagon { get; private set; }

        public ForceStaff ForceStaff { get; private set; }

        public EulsScepterOfDivinity Eul { get; private set; }

        public WindWaker WindWaker { get; private set; }

        public BlinkDagger Blink { get; private set; }

        public ShivasGuard Shivas { get; private set; }

        public Nullifier Nullifier { get; private set; }

        public UrnOfShadows Urn { get; private set; }

        public SpiritVessel Vessel { get; private set; }
    }
}