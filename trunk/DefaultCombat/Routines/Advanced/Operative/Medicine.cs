// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
//using DefaultCombat.Extensions; ((Hold off for now))

namespace DefaultCombat.Routines
{
    public class Medicine : RotationBase
    {
        public override string Name
        {
            get { return "Operative Medicine"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Coordination")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Tactical Superiority", ret => CombatHotkeys.EnableRaidBuffs),
                    Spell.Cast("Adrenaline Probe", ret => Me.EnergyPercent <= 20),
                    Spell.Cast("Stim Boost", ret => Me.EnergyPercent <= 70 && !Me.HasBuff("Tactical Advantage")),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 75),
                    Spell.Buff("Evasion", ret => Me.HealthPercent <= 50),
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
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .6f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .5f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment"))
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new PrioritySelector(

                    //Cleanse
                    //Spell.Cast("Toxin Scan", ret => HealTarget.ShouldDispel()), ((New Code Hold off for now))
                    Spell.Cleanse("Toxin Scan"),

                    //Healing
                    Spell.Heal("Surgical Probe", 30),
                    Spell.Heal("Recuperative Nanotech", on => Tank, 80, ret => Targeting.ShouldAoeHeal),
                    Spell.Heal("Kolto Probe", on => Tank, 100, ret => Tank != null && Tank.BuffCount("Kolto Probe") < 2 || Tank.BuffTimeLeft("Kolto Probe") < 6),
                    Spell.Heal("Kolto Infusion", 80, ret => Me.BuffCount("Tactical Advantage") >= 2 && Me.EnergyPercent >= 60 && !HealTarget.HasBuff("Kolto Infusion")),
                    Spell.Heal("Surgical Probe", 80, ret => Me.BuffCount("Tactical Advantage") >= 2),
                    Spell.Heal("Kolto Injection", 80),
                    Spell.Cleanse("Toxin Scan"),
                    Spell.Heal("Kolto Probe", 90, ret => HealTarget.BuffCount("Kolto Probe") < 2 || HealTarget.BuffTimeLeft("Kolto Probe") < 6),
                    Spell.Heal("Diagnostic Scan", 90)
                    );
            }
        }
    }
}
