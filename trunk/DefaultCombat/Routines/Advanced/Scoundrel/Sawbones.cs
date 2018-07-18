// Copyright (C) 2011-2018 Bossland GmbH 
// See the file LICENSE for the source code's detailed license 

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
//using DefaultCombat.Extensions; ((Hold off for now))

namespace DefaultCombat.Routines
{
    public class Sawbones : RotationBase
    {
        public override string Name
        {
            get { return "Scoundrel Sawbones"; }
        }

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
                return new PrioritySelector(
                    Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 20),
                    Spell.Buff("Pugnacity", ret => Me.EnergyPercent <= 70 && Me.BuffCount("Upper Hand") < 3),
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 75),
                    Spell.Buff("Dodge", ret => Me.HealthPercent <= 50),
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15)
                    );
            }
        }

        //DPS 
        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(

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
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Back Blast", ret => Me.IsBehind(Me.CurrentTarget)),
                    Spell.Cast("Blaster Whip"),
                    Spell.Cast("Vital Shot", ret => !Me.CurrentTarget.HasDebuff("Vital Shot")),
                    Spell.Cast("Quick Shot", ret => Me.EnergyPercent >= 70),
                    Spell.Cast("Flurry of Bolts")
                    );
            }
        }

        //Healing 
        public override Composite AreaOfEffect
        {
            get
            {
                return new PrioritySelector(

                    //Cleanse
                    //Spell.Cast("Triage", ret => HealTarget.ShouldDispel()), ((New Code Hold off for now))
                    Spell.Cleanse("Triage"),

                    //Healing
                    Spell.HealGround("Kolto Waves", ret => Targeting.ShouldAoeHeal),
                    Spell.Heal("Kolto Cloud", on => Tank, 80, ret => Tank != null && Targeting.ShouldAoeHeal),
                    Spell.Heal("Emergency Medpac", 90, ret => emergencyMedpac()),
                    Spell.Heal("Slow-release Medpac", on => Tank, 100, ret => Tank != null && tankSlowMedpac()),
                    Spell.Heal("Kolto Pack", 80, ret => Me.BuffCount("Upper Hand") >= 2 && Me.EnergyPercent >= 60 && !HealTarget.HasMyBuff("Kolto Pack")),
                    Spell.Heal("Underworld Medicine", 80),
                    Spell.Heal("Slow-release Medpac", 90, ret => targetSlowMedpac()),
                    Spell.Heal("Diagnostic Scan", 95)
                    );
            }
        }

        public static bool tankSlowMedpac()
        {
            if (Targeting.Tank.BuffCount("Slow-release Medpac") < 2)
                return true;
            if (Targeting.Tank.BuffTimeLeft("Slow-release Medpac") < 3)
                return true;
            return false;
        }

        public static bool targetSlowMedpac()
        {
            if (Targeting.HealTarget.BuffCount("Slow-release Medpac") < 2)
                return true;
            if (Targeting.HealTarget.BuffTimeLeft("Slow-release Medpac") < 3)
                return true;
            return false;
        }

        public static bool emergencyMedpac()
        {
            if (Targeting.HealTarget.BuffTimeLeft("Slow-release Medpac") > 6)
                return false;
            if (Targeting.HealTarget.BuffCount("Slow-release Medpac") < 2)
                return false;
            if (Me.BuffCount("Upper Hand") < 2)
                return false;
            return true;
        }
    }
}
