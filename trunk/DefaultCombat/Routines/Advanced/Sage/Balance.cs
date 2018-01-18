﻿// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    internal class Balance : RotationBase
    {
        public override string Name
        {
            get { return "Sage Balance"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force Valor")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force of Will", ret => Me.IsStunned),
                    Spell.Buff("Force Barrier", ret => Me.HealthPercent <= 20),
                    Spell.Buff("Force Armor", ret => !Me.HasBuff("Force Armor") && !Me.HasDebuff("Force-imbalance")),
                    Spell.Buff("Force Mend", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Mental Alacrity"),
                    Spell.Buff("Force Potency"),
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

                    //Solo Mode
                    Spell.Cast("Rejuvenate", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 70),
                    Spell.Cast("Benevolence", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 60),
                    Spell.Cast("Force Mend", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 50),

                    //Rotation
                    Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Force Stun", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Vanquish", ret => Me.BuffCount("Presence of Mind") == 4 && Me.Level >= 57),
                    Spell.Cast("Mind Crush", ret => Me.BuffCount("Presence of Mind") == 4 && Me.Level < 57),
                    Spell.DoT("Weaken Mind", "Weaken Mind"),
                    Spell.DoT("Sever Force", "Sever Force"),
                    Spell.CastOnGround("Force in Balance", ret => Me.CurrentTarget.HasDebuff("Weaken Mind") && Me.CurrentTarget.HasDebuff("Sever Force")),
                    Spell.Cast("Force Serenity", ret => Me.CurrentTarget.HasDebuff("Weaken Mind")),
                    Spell.Cast("Disturbance", ret => Me.BuffCount("Presence of Mind") == 4),
                    Spell.Cast("Telekinetic Throw", ret => Me.BuffCount("Presence of Mind") < 4),

                    //HK-55 Mode Rotation
                    Spell.Cast("Charging In", ret => Me.CurrentTarget.Distance >= .4f && Me.InCombat && CombatHotkeys.EnableHK55),
                    Spell.Cast("Blindside", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Assassinate", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Rail Blast", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Rifle Blast", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Execute", ret => Me.CurrentTarget.HealthPercent <= 45 && CombatHotkeys.EnableHK55)
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
                        Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
                        Spell.DoT("Weaken Mind", "Weaken Mind"),
                        Spell.DoT("Sever Force", "Sever Force"),
                        Spell.CastOnGround("Force in Balance", ret => Me.CurrentTarget.HasDebuff("Weaken Mind") && Me.CurrentTarget.HasDebuff("Sever Force")),
                        Spell.CastOnGround("Forcequake", ret => Me.CurrentTarget.HasDebuff("Overwhelmed (Mental)"))
                        ));
            }
        }
    }
}