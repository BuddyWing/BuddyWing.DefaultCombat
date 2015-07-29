using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Consular : RotationBase
	{
		public override string Name
		{
			get { return "Basic Consular"; }
		}

		public override Composite Buffs
		{
			get { return new PrioritySelector(); }
		}

		public override Composite Cooldowns
		{
			get { return new PrioritySelector(); }
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					CombatMovement.CloseDistance(Distance.Melee),
					Spell.Cast("Telekinetic Throw"),
					Spell.Cast("Project", ret => Me.Force > 75),
					Spell.Cast("Double Strike", ret => Me.Force > 70),
					Spell.Cast("Saber Strike")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.Cast("Force Wave", ret => Me.CurrentTarget.Distance <= Distance.MeleeAoE))
					);
			}
		}
	}
}