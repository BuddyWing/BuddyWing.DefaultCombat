// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using Buddy.CommonBot;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
using Targeting = DefaultCombat.Core.Targeting;

namespace DefaultCombat.Routines
{
    public class Ruffian : RotationBase
    {
        public override string Name
        {
            get { return "Scoundrel Ruffian"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(

                    Spell.Buff("Lucky Shots"),
                    Spell.Cast("Stealth", ret => !Me.InCombat && !Me.HasBuff("Lucky Shots"))
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 45),
                    Spell.Buff("Pugnacityt", ret => !Me.HasBuff("Upper Hand") && Me.InCombat),
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 80),
                    Spell.Buff("Dodge", ret => Me.HealthPercent <= 50),
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
                    Spell.Cast("Trick Move", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
                    Spell.Cast("Point Blank Shot", ret => (Me.IsStealthed)),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Energy
                    new Decorator(ret => Me.EnergyPercent < 35,
                        new PrioritySelector(
                            Spell.Cast("Flurry of Bolts")
                            )),

                    //Solo Mode
                    Spell.Cast("Kolto Pack", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 90 && Me.HasBuff("Scurry")),
                    Spell.Cast("Diagnostic Scan", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 10),
                    Spell.Cast("Slow-release Medpac", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 80 && Me.BuffTimeLeft("Slow-release Medpac") <= 3),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Brutal Shots", ret => Me.CurrentTarget.HasDebuff("Vital Shot") && Me.CurrentTarget.HasDebuff("Shrap Bomb") && Me.HasBuff("Upper Hand")),
                    Spell.Cast("Sanguinary Shot", ret => Me.CurrentTarget.HasDebuff("Vital Shot") && Me.CurrentTarget.HasDebuff("Shrap Bomb")),
                    Spell.Cast("Vital Shot", ret => !Me.CurrentTarget.HasDebuff("Vital Shot") || Me.CurrentTarget.DebuffTimeLeft("Vital Shot") <= 3),
                    Spell.Cast("Shrap Bomb", ret => !Me.CurrentTarget.HasDebuff("Shrap Bomb") || Me.CurrentTarget.DebuffTimeLeft("Shrap Bomb") <= 3),
                    Spell.Cast("Blaster Whip", ret => Me.BuffCount("Upper Hand") < 2 || Me.BuffTimeLeft("Upper Hand") < 6),
                    Spell.Cast("Point Blank Shot", ret => Me.Level >= 57),
                    Spell.Cast("Back Blast", ret => Me.IsBehind(Me.CurrentTarget) && Me.Level < 57),
                    Spell.Cast("Quick Shot", ret => Me.EnergyPercent > 80 && !Me.HasBuff("Upper Hand")),
                    Spell.Cast("Flurry of Bolts", ret => Me.EnergyPercent < 80 && !Me.HasBuff("Upper Hand") || Me.CurrentTarget.Distance > 1f)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
                        Spell.DoT("Shrap Bomb", "Shrap Bomb"),
                        Spell.Cast("Thermal Grenade"),
                        Spell.Cast("Lacerating Blast"),
                        Spell.Cast("Bushwhack", ret => Me.HasBuff("Upper Hand"))
                        ));
            }
        }
    }
}
