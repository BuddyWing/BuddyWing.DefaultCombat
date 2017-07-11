// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Carnage : RotationBase
	{
		public override string Name
		{
			get { return "Marauder Carnage"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Unnatural Might")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Unleash", ret => Me.IsStunned),
					Spell.Buff("Cloak of Pain", ret => Me.HealthPercent <= 90),
					Spell.Buff("Force Camouflage", ret => Me.HealthPercent <= 70),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
					Spell.Buff("Undying Rage", ret => Me.HealthPercent <= 20),
					Spell.Buff("Frenzy", ret => Me.BuffCount("Fury") < 5),
					Spell.Buff("Bloodthirst", ret => Me.BuffCount("Fury") > 29 && Me.InCombat),
					Spell.Buff("Berserk", ret => Me.BuffCount("Fury") > 29),
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
					Spell.Cast("Dual Saber Throw", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Force Charge", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
					
					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					
					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

					//Rotation
					Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Battering Assault", ret => Me.ActionPoints <= 6 && !Me.HasBuff("Ferocity")),
					Spell.Cast("Ferocity"),
					Spell.Cast("Devastating Blast", ret => Me.HasBuff("Ferocity")),
					Spell.Cast("Gore", ret => Me.HasBuff("Ferocity")),
					Spell.Cast("Vicious Throw", ret => Me.HasBuff("Ferocity")),
					Spell.Cast("Ravage", ret => Me.HasBuff("Ferocity")),
					Spell.Cast("Massacre", ret => !Me.HasBuff("Massacre")),
					Spell.Cast("Dual Saber Throw", ret => !Me.HasBuff("Ferocity")),
					Spell.Cast("Battering Assault", ret => Me.ActionPoints <= 6 && !Me.HasBuff("Ferocity")),
					//Temp removal as a test for significant DPS loss Spell.Cast("Force Scream", ret => Me.HasBuff("Execute")),
					Spell.Cast("Assault", ret => Me.ActionPoints <= 5 && !Me.HasBuff("Ferocity"))
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
						//Temp removal as a test for significant DPS loss Spell.Cast("Smash"),
						Spell.Cast("Sweeping Slash")
						));
			}
		}
	}
}
