using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Engineering : RotationBase
	{
		public override string Name
		{
			get { return "Sniper Engineering"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Coordination")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Escape"),
					Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 50),
					Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 50),
					Spell.Buff("Laze Target"),
					Spell.Cast("Target Acquired")
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Low Energy
					new Decorator(ret => Me.EnergyPercent < 60,
						new LockSelector(
							Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
							Spell.Cast("Series of Shots", ret => Me.IsInCover()),
							Spell.Cast("Snipe", ret => Me.IsInCover()),
							Spell.Cast("Overload Shot", ret => !Me.IsInCover())
							)),

					//Rotation
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
					Spell.Cast("Shatter Shot", ret => !Me.CurrentTarget.HasDebuff("Armor Reduced")),
					Spell.Cast("Series of Shots", ret => Me.IsInCover()),
					Spell.CastOnGround("Plasma Probe"),
					Spell.DoT("Interrogation Probe", "", 15000),
					Spell.DoT("Corrosive Dart", "", 12000),
					Spell.Cast("Explosive Probe", ret => Me.IsInCover()),
					Spell.Cast("Takedown", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Snipe", ret => Me.IsInCover()),
					Spell.Cast("Overload Shot", ret => !Me.IsInCover())
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.CastOnGround("Orbital Strike"),
						Spell.CastOnGround("Plasma Probe"),
						Spell.Cast("Fragmentation Grenade")
						));
			}
		}
	}
}