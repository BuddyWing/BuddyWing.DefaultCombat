// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Watchman : RotationBase
    {
        public override string Name
        {
            get { return "Sentinel Watchman"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force Might")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Resolute", ret => Me.IsStunned),
                    Spell.Buff("Inspiration", ret => CombatHotkeys.EnableRaidBuffs),
                    Spell.Buff("Rebuke", ret => Me.HealthPercent <= 75),
                    Spell.Buff("Force Camouflage"),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 25),
                    Spell.Buff("Guarded by the Force", ret => Me.HealthPercent <= 15),
                    Spell.Buff("Overload Saber", ret => !Me.HasBuff("Overload Saber")),
                    Spell.Buff("Valorous Call", ret => Me.BuffCount("Centering") < 15),
                    Spell.Buff("Zen", ret => !Me.HasBuff("Zen") && Me.CurrentTarget.HasDebuff("Burning (Overload Saber)")),
                    Spell.Cast("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Force Leap", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Twin Saber Throw", ret => Me.HasBuff("Mind Sear")),
                    Spell.Cast("Merciless Slash"),
                    Spell.Cast("Cauterize", ret => !Me.CurrentTarget.HasMyDebuff("Burning (Cauterize)")),
                    Spell.Cast("Force Melt"),
                    Spell.Cast("Dispatch", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Blade Barrage"),
                    Spell.Cast("Slash", ret => Me.ActionPoints >= 9 && Me.CurrentTarget.HasDebuff("Burning (Cauterize)")),
                    Spell.Cast("Zealous Strike", ret => Me.ActionPoints <= 6),
                    Spell.Cast("Force Leap", ret => CombatHotkeys.EnableCharge && Me.ActionPoints <= 8),
                    Spell.Cast("Strike", ret => Me.ActionPoints < 7 ),
                    Spell.Cast("Blade Storm", ret => Me.ActionPoints < 5)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.Cast("Twin Saber Throw", ret => Me.HasBuff("Mind Sear")),
                        Spell.Cast("Force Melt", ret => !Me.CurrentTarget.HasMyDebuff("Force Melt")),
                        Spell.Cast("Cauterize", ret => !Me.CurrentTarget.HasMyDebuff("Burning (Cauterize)")),
                        Spell.Cast("Force Sweep", ret => Me.CurrentTarget.HasDebuff("Burning (Cauterize)") || Me.CurrentTarget.HasMyDebuff("Force Melt")),
                        Spell.Cast("Cyclone Slash")
                        ));
            }
        }
    }
}
