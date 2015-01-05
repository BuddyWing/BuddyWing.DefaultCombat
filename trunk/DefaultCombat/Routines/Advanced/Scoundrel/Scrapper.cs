using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Scrapper : RotationBase
    {
        public override string Name { get { return "Scoundrel Scrapper"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Lucky Shots"),
                    Spell.Buff("Stealth", ret => !Rest.KeepResting() && !Rest.NeedRest())
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 45),
                    Spell.Buff("Pugnacity", ret => Me.HasBuff("Upper Hand")),
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 75),
                    Spell.Buff("Dodge", ret => Me.HealthPercent <= 50)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new LockSelector(
                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    Spell.Cast("Backblast", ret => (Me.IsBehind(Me.CurrentTarget))),
                    Spell.DoT("Vital Shot", "Bleeding (Vital Shot)"),
                    Spell.Cast("Sucker Punch", ret => Me.HasBuff("Upper Hand")),
                    Spell.Cast("Blood Boiler"),
                    Spell.Cast("Bludgeon"),
                    Spell.Cast("Shank Shot"),
                    Spell.Cast("Blaster Whip"),
                    Spell.Cast("Dirty Kick"),
                    Spell.Cast("Quick Shot", ret => Me.EnergyPercent >= 87),
                    Spell.Cast("Flurry of Bolts")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {                
                return new Decorator(ret => Targeting.ShouldAOE,
                    new LockSelector(
                        Spell.Cast("Thermal Grenade"),
                        Spell.Cast("Blaster Volley", ret => Me.HasBuff("Upper Hand")))
                );
            }
        }
    }
}
