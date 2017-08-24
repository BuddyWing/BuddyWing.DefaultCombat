// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Immortal : RotationBase
	{
		public override string Name
		{
			get { return "Juggernaut Immortal"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Unnatural Might"),
					Spell.Cast("Guard", on => Me.Companion, ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard"))
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Unleash", ret => Me.IsStunned),
					Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 90),
					Spell.Buff("Endure Pain", ret => Me.HealthPercent <= 80),
					Spell.Buff("Enraged Defense", ret => Me.HealthPercent < 70),
					Spell.Buff("Invincible", ret => Me.HealthPercent <= 50),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 30),
					Spell.Buff("Enrage", ret => Me.ActionPoints <= 6),
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
					Spell.Cast("Saber Throw", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 0.5f && Me.CurrentTarget.Distance <= 3f),
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
					Spell.Cast("Retaliation"),
					Spell.Cast("Crushing Blow"),
					Spell.Cast("Force Scream"),
					Spell.Cast("Aegis Assault", ret => Me.ActionPoints <= 7 || !Me.HasBuff("Aegis")),
					Spell.Cast("Smash", ret => !Me.CurrentTarget.HasDebuff("Unsteady (Force)") && Targeting.ShouldPbaoe),
					Spell.Cast("Backhand", ret => !Me.CurrentTarget.IsStunned),
					Spell.Cast("Ravage"),
					Spell.Cast("Vicious Throw", ret => Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("War Bringer")),
					Spell.Cast("Vicious Slash", ret => Me.ActionPoints >= 9),
					Spell.Cast("Assault"),
					Spell.Cast("Saber Throw", ret => Me.CurrentTarget.Distance >= 0.5f && Me.InCombat),

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
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
						Spell.DoT("Chilling Scream", "Chilling Scream", 0, ret => Me.CurrentTarget.Distance <= 0.8f),
						Spell.Cast("Smash"),
						Spell.Cast("Crushing Blow", ret => Me.HasBuff("Aegis")),
						Spell.Cast("Aegis Assault", ret => !Me.HasBuff("Aegis")),
						Spell.Cast("Retaliation"),
						Spell.Cast("Force Scream"),
						Spell.Cast("Sweeping Slash")
						));
			}
		}
	}
}
