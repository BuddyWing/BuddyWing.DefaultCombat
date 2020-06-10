// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using Buddy.CommonBot;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
using Targeting = DefaultCombat.Core.Targeting;

namespace DefaultCombat.Routines
{
    public class Scrapper : RotationBase
    {
        public override string Name
        {
            get { return "Scoundrel Scrapper"; }
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
                    Spell.Cast("Pugnacity", ret => Me.HasBuff("Upper Hand")),
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 75),
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
                    new Decorator(ret => Me.EnergyPercent < 60,
                        new PrioritySelector(
                            Spell.Cast("Flurry of Bolts")
                            )),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Back Blast", ret => Me.IsBehind(Me.CurrentTarget)),
                    Spell.Cast("Sucker Punch", ret => Me.HasBuff("Upper Hand") && Me.CurrentTarget.HasDebuff("Vital Shot")),
                    Spell.DoT("Vital Shot", "Vital Shot"),
                    Spell.Cast("Blood Boiler"),
                    Spell.Cast("Bludgeon"),
                    Spell.Cast("Quick Shot", ret => Me.EnergyPercent >= 75)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Cast("Thermal Grenade"),
                        Spell.Cast("Bushwhack", ret => !Me.HasBuff("Upper Hand")),
                        Spell.Cast("Lacerating Blast")
                    ));
            }
        }
    }
}
