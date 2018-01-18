﻿// Copyright (C) 2011-2017 Bossland GmbH 
// See the file LICENSE for the source code's detailed license 

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    internal class Seer : RotationBase
    {
        public override string Name
        {
            get { return "Sage Seer"; }
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
                    Spell.Buff("Force Potency", ret => Targeting.ShouldAoeHeal),
                    Spell.Buff("Mental Alacrity", ret => Targeting.ShouldAoeHeal),
                    Spell.Buff("Vindicate", ret => NeedForce()),
                    Spell.Buff("Force Mend", ret => Me.HealthPercent <= 75),
                    Spell.HoT("Force Armor", on => Me, 100, ret => Me.InCombat && !Me.HasDebuff("Force-imbalance")),
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
                    Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Force Stun", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Forcequake", ret => Targeting.ShouldAoe),
                    Spell.DoT("Weaken Mind", "Weaken Mind"),
                    Spell.Cast("Mind Crush"),
                    Spell.Cast("Project"),
                    Spell.Cast("Telekinetic Throw"),
                    Spell.Cast("Disturbance"),

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
                return new PrioritySelector(

                    //Legacy Heroic Moment Ability
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
                    Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode

                    //Cleanse if needed 
                    Spell.Cleanse("Restoration"),

                    //Emergency Heal (Insta-cast) 
                    Spell.Heal("Benevolence", 80, ret => Me.HasBuff("Altruism")),

                    //AoE Healing 
                    new Decorator(ctx => Tank != null,
                        Spell.CastOnGround("Salvation", on => Tank.Position)),

                    //Single Target Healing 
                    Spell.HoT("Force Armor", 90, ret => HealTarget != null && !HealTarget.HasDebuff("Force-imbalance")),
                    Spell.Heal("Healing Trance", 80),

                    //Buff Tank 
                    Spell.HoT("Force Armor", on => Tank, 100, ret => Tank != null && Tank.InCombat && !Tank.HasDebuff("Force-imbalance")),
                    Spell.Heal("Wandering Mend", onUnit => Tank, 100, ret => Tank != null && Tank.InCombat && Me.BuffCount("Wandering Mend Charges") <= 1),

                    //Use Force Bending 
                    new Decorator(ret => Me.HasBuff("Conveyance"),
                        new PrioritySelector(
                            Spell.Heal("Healing Trance", 90),
                            Spell.Heal("Deliverance", 50)
                            )),

                    //Build Force Bending 
                    Spell.HoT("Rejuvenate", 80),
                    Spell.HoT("Rejuvenate", onUnit => Tank, 100, ret => Tank != null && Tank.InCombat),

                    //Single Target Healing                   
                    Spell.Heal("Benevolence", 35),
                    Spell.Heal("Deliverance", 80)
                    );
            }
        }

        private bool NeedForce()
        {
            if (Me.HasBuff("Resplendence") && Me.ForcePercent < 80 && !Me.HasBuff("Amnesty"))
                return true;
            return false;
        }
    }
}