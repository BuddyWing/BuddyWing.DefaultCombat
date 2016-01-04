// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Defense : RotationBase
	{
		public override string Name
		{
			get { return "Guardian Defense"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Soresu Form"),
					Spell.Buff("Force Might"),
					Spell.Cast("Guard", on => Me.Companion,
						ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard"))
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Resolute"),
					Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 90),
					Spell.Buff("Enure", ret => Me.HealthPercent <= 80),
					Spell.Buff("Focused Defense", ret => Me.HealthPercent < 70),
					Spell.Buff("Warding Call", ret => Me.HealthPercent <= 50),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 30),
					Spell.Buff("Combat Focus", ret => Me.ActionPoints <= 6)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Saber Throw",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 0.5f && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Force Leap",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Rotation
					Spell.Cast("Riposte"),
					Spell.Cast("Guardian Slash"),
					Spell.Cast("Blade Storm"),
					Spell.Cast("Warding Strike", ret => Me.ActionPoints <= 7 || !Me.HasBuff("Warding Strike")),
					Spell.Cast("Force Sweep", ret => !Me.CurrentTarget.HasDebuff("Unsteady (Force)") && Targeting.ShouldPbaoe),
					Spell.Cast("Hilt Bash", ret => !Me.CurrentTarget.IsStunned),
					Spell.Cast("Blade Dance"),
					Spell.Cast("Dispatch", ret => Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("Ardent Advocate")),
					Spell.Cast("Slash", ret => Me.ActionPoints >= 9),
					Spell.Cast("Strike"),
					Spell.Cast("Saber Throw", ret => Me.CurrentTarget.Distance >= 0.5f && Me.InCombat)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						Spell.Cast("Force Sweep"),
						Spell.Cast("Guardian Slash", ret => Me.HasBuff("Warding Strike")),
						Spell.Cast("Warding Strike", ret => !Me.HasBuff("Warding Strike")),
						Spell.Cast("Riposte"),
						Spell.Cast("Blade Storm"),
						Spell.Cast("Cyclone Slash")
						));
			}
		}
	}
}