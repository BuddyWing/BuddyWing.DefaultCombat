// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
//using DefaultCombat.Extensions; ((Hold off for now))

namespace DefaultCombat.Routines
{
    public class Bodyguard : RotationBase
    {
        public override string Name
        {
            get { return "Mercenary Bodyguard"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Hunter's Boon")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Determination", ret => Me.IsStunned),
                    Spell.Buff("Supercharged Gas", ret => Me.BuffCount("Supercharge") == 10 && Me.ResourcePercent() <= 80 && HealTarget.HealthPercent <= 80),
                    Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 70),
                    Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 40),
                    Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),
                    Spell.Cast("Responsive Safeguards", ret => Me.HealthPercent <= 20),
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15),
                    Spell.Cast("Sacrifice", ret => Me.HealthPercent <= 5)
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
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Jet Boost", ret => Me.InCombat && Me.CurrentTarget.Distance <= 0.4f && Me.CurrentTarget.IsHostile),
                    Spell.Cast("Rapid Shots", ret => Me.ResourcePercent() > 40),
                    Spell.Cast("Unload", ret => Me.Level < 57),
                    Spell.Cast("Rail Shot"),
                    Spell.Cast("Electro Net"),
                    Spell.Cast("Power Shot", ret => Me.ResourceStat <= 70),
                    Spell.Cast("Missile Blast")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new PrioritySelector(

                        //Legacy Heroic Moment Ability
                        Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode

                        //Solo Mode
                        Spell.Buff("Thermal Sensor Override", ret => CombatHotkeys.EnableSolo),
                        Spell.CastOnGround("Death from Above", ret => Me.HasBuff("Thermal Sensor Override") && CombatHotkeys.EnableSolo && Me.InCombat && Me.CurrentTarget.IsHostile),
                        Spell.Cast("Explosive Dart", ret => CombatHotkeys.EnableSolo),
                        Spell.Cast("Fusion Missile", ret => CombatHotkeys.EnableSolo && Me.CurrentTarget.IsHostile),
                        Spell.CastOnGround("Sweeping Blasters", ret => CombatHotkeys.EnableSolo && Me.ResourcePercent() <= 35 && Me.InCombat && Me.CurrentTarget.IsHostile),

                        //BuffLog.Instance.LogTargetBuffs,

                        //Cleanse
                        //Spell.Cast("Cure", ret => HealTarget.ShouldDispel()), ((New Code Hold off for now))
                        Spell.Cleanse("Cure"),

                        //Buff Party
                        Spell.Heal("Kolto Shell", on => HealTarget, 100, ret => !HealTarget.HasBuff("Kolto Shell")),

                        //AoE Healing 
                        new Decorator(ctx => Tank != null,
                            Spell.CastOnGround("Kolto Missile", on => HealTarget.Position, ret => HealTarget.HealthPercent < 85 && !HealTarget.HasBuff("Kolto Residue") && Me.InCombat)),

                        //Free, so use it!
                        Spell.Heal("Emergency Scan", 80, ret => Me.InCombat),

                        //Important Buffs to take advantage of
                        new Decorator(ctx => Tank != null,
                        Spell.CastOnGround("Kolto Missile", on => HealTarget.Position, ret => Me.HasBuff("Supercharged Gas") && Me.InCombat)),
                        Spell.Heal("Rapid Scan", 80, ret => Me.HasBuff("Critical Efficiency") && Me.InCombat),
                        Spell.Heal("Healing Scan", 80, ret => Me.HasBuff("Supercharged Gas") && Me.InCombat),

                        //Single Target Priority
                        Spell.Heal("Healing Scan", 80, ret => Me.BuffCount("Critical Efficiency") >= 3),
                        Spell.Heal("Rapid Scan", 88, ret => Me.ResourcePercent() <= 50),
                        Spell.Heal("Progressive Scan", 99),
                        Spell.Heal("Kolto Shot", 95),
                        Spell.Heal("Emergency Scan", 85),
                        Spell.Heal("Healing Scan", 65),

                        //Filler
                        Spell.Heal("Kolto Shot", 95, ret => Me.IsMoving),
                        Spell.HoT("Progressive Scan", 85, ret => Me.IsMoving)
                        );
            }
        }
    }
}
