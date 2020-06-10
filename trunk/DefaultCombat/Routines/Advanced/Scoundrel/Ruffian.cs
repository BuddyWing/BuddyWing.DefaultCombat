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
					Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled && !Me.IsMounted)
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Stack the Deck", ret => CombatHotkeys.EnableRaidBuffs),
                    Spell.Cast("Cool Head", ret => Me.EnergyPercent <= 45),
                    Spell.Cast("Pugnacityt", ret => !Me.HasBuff("Upper Hand") && Me.InCombat),
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 80),
                    Spell.Buff("Dodge", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Trick Move", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance > .4f),

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
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Energy
                    new Decorator(ret => Me.EnergyPercent < 35,
                        new PrioritySelector(
                            Spell.Cast("Flurry of Bolts")
                            )),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Brutal Shots", ret => Me.CurrentTarget.HasDebuff("Vital Shot") && Me.CurrentTarget.HasDebuff("Shrap Bomb") && Me.HasBuff("Upper Hand")),
                    Spell.Cast("Sanguinary Shot", ret => Me.CurrentTarget.HasDebuff("Vital Shot") && Me.CurrentTarget.HasDebuff("Shrap Bomb")),
                    Spell.Cast("Vital Shot", ret => !Me.CurrentTarget.HasDebuff("Vital Shot") || Me.CurrentTarget.DebuffTimeLeft("Vital Shot") <= 3),
                    Spell.Cast("Shrap Bomb", ret => !Me.CurrentTarget.HasDebuff("Shrap Bomb") || Me.CurrentTarget.DebuffTimeLeft("Shrap Bomb") <= 3),
                    Spell.Cast("Blaster Whip", ret => Me.BuffCount("Upper Hand") < 2 || Me.BuffTimeLeft("Upper Hand") < 6),
                    Spell.Cast("Point Blank Shot"),
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
                        Spell.DoT("Shrap Bomb", "Shrap Bomb"),
                        Spell.Cast("Thermal Grenade"),
                        Spell.Cast("Lacerating Blast"),
                        Spell.Cast("Bushwhack", ret => Me.HasBuff("Upper Hand"))
                        ));
            }
        }
    }
}
