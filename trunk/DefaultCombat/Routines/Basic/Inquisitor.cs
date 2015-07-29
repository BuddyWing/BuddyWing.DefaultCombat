using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Inquisitor : RotationBase
	{
		public override string Name
		{
			get { return "Basic Inquisitor"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Mark of Power")
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
					CombatMovement.CloseDistance(Distance.Melee),
					Spell.Cast("Force Lightning"),
					Spell.Cast("Shock", ret => Me.Force > 75),
					Spell.Cast("Thrash", ret => Me.Force > 70),
					Spell.Cast("Saber Strike")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new LockSelector(
						Spell.Cast("Overload", ret => Me.CurrentTarget.Distance <= Distance.MeleeAoE)
						));
			}
		}
	}
}