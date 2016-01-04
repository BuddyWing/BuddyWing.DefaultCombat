// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Infiltration : RotationBase
	{
		public override string Name
		{
			get { return "Shadow Infiltration"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Shadow Technique"),
					Spell.Buff("Force Valor"),
					Spell.Cast("Guard", on => Me.Companion,
						ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard")),
					Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled)
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Force of Will"),
					Spell.Buff("Battle Readiness", ret => Me.HealthPercent <= 85),
					Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
					Spell.Buff("Resilience", ret => Me.HealthPercent <= 50),
					Spell.Buff("Force Potency"),
					Spell.Buff("Blackout", ret => Me.ForcePercent <= 40)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Spinning Kick", ret => Me.IsStealthed),
					Spell.Buff("Force Speed",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Interrupts
					Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Force Stun", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Low Slash", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),

					//Rotation
					Spell.Cast("Force Breach", ret => Me.BuffCount("Breaching Shadows") == 3),
					Spell.Cast("Shadow Strike", ret => Me.HasBuff("Stealth") || Me.HasBuff("Infiltration Tactics")),
					Spell.Cast("Project", ret => Me.BuffCount("Circling Shadows") == 2),
					Spell.Cast("Spinning Strike", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Clairvoyant Strike"),
					Spell.Cast("Force Speed", ret => Me.CurrentTarget.Distance >= 1.1f && Me.IsMoving && Me.InCombat)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Whirling Blow", ret => Me.ForcePercent >= 60 && Targeting.ShouldPbaoe)
					);
			}
		}
	}
}