using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    class DirtyFighting : RotationBase
    {
        public override string Name { get { return "Gunslinger Dirty Fighting"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Lucky Shots")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Escape"),
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Dodge", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 50),
                    Spell.Buff("Smuggler's Luck"),
                    Spell.Buff("Illegal Mods")
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new LockSelector(
                    Spell.Cast("Flurry of Bolts", ret => Me.EnergyPercent < 60),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Ranged),
                   
                    //Rotation
                    Spell.Cast("Charged Burst", ret => Me.HasBuff("Smuggler's Luck")),
                    Spell.Cast("Flurry of Bolts", ret => Me.EnergyPercent < 40),
                    Spell.DoT("Flourish Shot", "Armor Reduced"),
                    Spell.Cast("Sabotage Charge", ret => Me.IsInCover()),
                    Spell.DoT("Shrap Bomb", "Shrapbomb"),
                    Spell.DoT("Vital Shot", "Bleeding (Vital Shot)"),
                    Spell.Cast("Hemorrhaging Blast"),
                    Spell.Cast("Quickdraw", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Wounding Shots"),
                    Spell.Cast("Crouch", ret => !Me.IsInCover()),
                    Spell.Cast("Speed Shot", ret => Me.IsInCover()),
                    Spell.Cast("Aimed Shot", ret => Me.IsInCover()),
                    Spell.Cast("Quick Shot", ret => !Me.IsInCover()),
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
                            Spell.CastOnGround("XS Freighter Flyby"),
                            Spell.Cast("Thermal Grenade"),
                            Spell.DoT("Shrap Bomb", "Shrap Bomb"),
                            Spell.CastOnGround("Sweeping Gunfire")
                            ));
            }
        }
    }
}