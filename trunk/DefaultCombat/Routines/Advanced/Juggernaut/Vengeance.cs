// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Vengeance : RotationBase
	{
		public override string Name
		{
			get { return "Juggernaut Vengeance"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Shien Form"),
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
					Spell.Buff("Enraged Defense", ret => Me.HealthPercent <= 70),
					Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 60),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
					Spell.Buff("Endure Pain", ret => Me.HealthPercent <= 30),
					Spell.Buff("Enrage", ret => Me.ActionPoints <= 6)
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
					Spell.Cast("Force Charge",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Rotation
					Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Force Scream", ret => Me.BuffCount("Savagery") == 2),
					Spell.Cast("Vicious Throw", ret => Me.HasBuff("Destroyer") || Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Shatter"),
					Spell.Cast("Impale"),
					Spell.Cast("Sundering Assault", ret => Me.ActionPoints <= 7),
					Spell.Cast("Ravage"),
					Spell.Cast("Vicious Slash", ret => Me.ActionPoints >= 11),
					Spell.Cast("Assault"),
					Spell.Cast("Retaliation")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						Spell.Cast("Vengeful Slam"),
						Spell.Cast("Smash"),
						Spell.Cast("Sweeping Slash")
						));
			}
		}
	}
}