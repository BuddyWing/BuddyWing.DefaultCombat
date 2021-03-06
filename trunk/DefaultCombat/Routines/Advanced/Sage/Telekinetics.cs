﻿// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Telekinetics : RotationBase
    {
        public override string Name
        {
            get { return "Sage Telekinetics"; }
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
					Spell.Cast("Telekinetic Blitz", ret => Me.CurrentTarget.BossOrGreater()),   // maybe this could be implemented better in the rotation
                    Spell.Buff("Force Empowerment", ret => CombatHotkeys.EnableRaidBuffs),
                    Spell.Cast("Force Potency"),
                    Spell.Cast("Mental Alacrity"),
                    Spell.Cast("Vindicate", ret => Me.ForcePercent < 50 && !Me.HasDebuff("Weary")),
                    Spell.HoT("Force Armor", on => Me, 60, ret => !Me.HasDebuff("Force-imbalance") && !Me.HasBuff("Force Armor")),
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

                    //Rotation
                    Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Weaken Mind", ret => !Me.CurrentTarget.HasDebuff("Weaken Mind")),
                    Spell.Cast("Turbulence"),
                    Spell.Cast("Telekinetic Gust"),
                    Spell.Cast("Mind Crush", ret => Me.HasBuff("Force Gust")),
                    Spell.Cast("Project", ret => Me.CurrentTarget.HasDebuff("Crushed (Mind Crush)")),
                    Spell.Cast("Telekinetic Wave", ret => Me.HasBuff("Tidal Force")),
                    Spell.Cast("Telekinetic Burst")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Cast("Telekinetic Wave", ret => Me.HasBuff("Tidal Force")),
                        Spell.CastOnGround("Forcequake")
                        ));
            }
        }
    }
}
