// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class DirtyFighting : RotationBase
    {
        public override string Name
        {
            get { return "Gunslinger Dirty Fighting"; }
        }

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
                return new PrioritySelector(
                    Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Dodge", ret => Me.HealthPercent <= 30),
                    Spell.Cast("Cool Head", ret => Me.EnergyPercent <= 50),
                    Spell.Cast("Smuggler's Luck", ret => Me.CurrentTarget.BossOrGreater()),
                    Spell.Cast("Illegal Mods", ret => Me.CurrentTarget.BossOrGreater()),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    //Movement
                    CombatMovement.CloseDistance(Distance.Ranged),


                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .6f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .5f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Energy
                    Spell.Cast("Flurry of Bolts", ret => Me.EnergyPercent < 60),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.DoT("Vital Shot", "Vital Shot"),
                    Spell.DoT("Shrap Bomb", "Shrap Bomb"),
                    Spell.Cast("Hemorrhaging Blast", ret => Me.CurrentTarget.HasDebuff("Vital Shot") && Me.CurrentTarget.HasDebuff("Shrap Bomb")),
                    Spell.Cast("Wounding Shots", ret => Me.CurrentTarget.DebuffTimeLeft("Vital Shot") > 3 && Me.CurrentTarget.DebuffTimeLeft("Shrap Bomb") > 3),
                    Spell.Cast("Quickdraw", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Speed Shot"),
                    Spell.Cast("Dirty Blast")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
                        Spell.CastOnGround("XS Freighter Flyby", ret => Me.IsInCover() && Me.EnergyPercent > 30),
                        Spell.Cast("Thermal Grenade"),
                        Spell.Cast("Shrap Bomb", ret => Me.CurrentTarget.HasDebuff("Vital Shot") && !Me.CurrentTarget.HasDebuff("Shrap Bomb")),
                        Spell.CastOnGround("Sweeping Gunfire", ret => Me.IsInCover() && Me.EnergyPercent > 10)
                        ));
            }
        }
    }
}
