// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Concentration : RotationBase
	{
		public override string Name
		{
			get { return "Sentinel Concentration"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Shii-Cho Form"),
					Spell.Buff("Force Might")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Rebuke", ret => Me.HealthPercent <= 50),
					Spell.Buff("Guarded by the Force", ret => Me.HealthPercent <= 10),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 30)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Rotation
					Spell.Cast("Force Leap"),
					Spell.Cast("Dispatch", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Force Sweep", ret => Me.HasBuff("Singularity") && Me.HasBuff("Felling Blow")),
					Spell.Cast("Force Exhaustion"),
					Spell.Cast("Zealous Leap", ret => Me.HasBuff("Singularity")),
					Spell.Cast("Blade Storm", ret => Me.HasBuff("Battle Cry") || Me.Energy >= 5),
					Spell.Cast("Dual Saber Throw"),
					Spell.Cast("Blade Dance"),
					Spell.Cast("Force Stasis"),
					Spell.Cast("Slash", ret => Me.HasBuff("Zen")),
					Spell.Cast("Zealous Strike"),
					Spell.Cast("Strike")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						));
			}
		}
	}
}