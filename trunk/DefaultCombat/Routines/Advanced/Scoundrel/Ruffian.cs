using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    class Ruffian : RotationBase
    {
        public override string Name { get { return "Scoundrel Ruffian"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Lucky Shots"),
                    Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled)
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 45),
                    Spell.Buff("Pugnacity", ret => !Me.HasBuff("Upper Hand") && Me.InCombat),
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
                    Spell.Cast("Shoot First", ret => Me.IsStealthed && Me.IsBehind(Me.CurrentTarget)),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Rotation
                    Spell.Cast("Flurry of Bolts", ret => Me.EnergyPercent < 60),
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= 1f),
                    Spell.Cast("Shoot First", ret => !Me.HasBuff("Upper Hand") && Me.IsBehind(Me.CurrentTarget) && Me.IsStealthed),
                    Spell.DoT("Vital Shot", "Bleeding (Vital Shot)"),
                    Spell.DoT("Shrap Bomb", "", 18000),
                    Spell.Cast("Hemorrhaging Blast"),
                    Spell.Cast("Wounding Shots", ret => Me.HasBuff("Upper Hand")),
                    Spell.Cast("Blaster Whip", ret => Me.BuffCount("Upper Hand") < 2 || Me.BuffTimeLeft("Upper Hand") < 6),
                    Spell.Cast("Back Blast", ret => Me.IsBehind(Me.CurrentTarget)),
                    Spell.Cast("Quick Shot")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAOE,
                        new LockSelector(
                            Spell.DoT("Shrap Bomb", "", 17000),
                            Spell.Cast("Thermal Grenade"),
                            Spell.Cast("Blaster Volley", ret => Me.HasBuff("Upper Hand"))
                ));
            }
        }
    }
}