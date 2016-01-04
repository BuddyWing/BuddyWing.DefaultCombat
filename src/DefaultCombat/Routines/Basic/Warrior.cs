// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Warrior : RotationBase
	{
		public override string Name
		{
			get { return "Basic Warrior"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Unnatural Might"),
					Spell.Buff("Shii-Cho Form")
					);
			}
		}

		public override Composite Cooldowns
		{
			get { return new LockSelector(); }
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					Spell.Cast("Force Charge",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance > 1f && Me.CurrentTarget.Distance <= 3f),
					CombatMovement.CloseDistance(Distance.Melee),
					Spell.Cast("Saber Ward", ret => Me.HealthPercent <= 70),
					Spell.Cast("Ravage"),
					Spell.Cast("Force Scream"),
					Spell.Cast("Retaliation"),
					Spell.Cast("Vicious Slash", ret => Me.ActionPoints >= 7),
					Spell.Cast("Assault")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new LockSelector(
						Spell.Cast("Smash", ret => Me.CurrentTarget.Distance <= Distance.MeleeAoE)
						));
			}
		}
	}
}