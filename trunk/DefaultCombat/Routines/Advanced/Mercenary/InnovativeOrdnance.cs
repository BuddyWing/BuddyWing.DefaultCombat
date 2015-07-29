using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class InnovativeOrdnance : RotationBase
	{
		public override string Name
		{
			get { return "Mercenary Innovative Ordnance"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Combustible Gas Cylinder"),
					Spell.Buff("Hunter's Boon")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Determination", ret => Me.IsStunned),
					Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 50),
					Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 40),
					Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(

					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),
					new Decorator(ret => Me.ResourcePercent() > 40,
						new LockSelector(
							Spell.Cast("Rail Shot", ret => Me.HasBuff("Prototype Particle Accelerator")),
							Spell.Cast("Rapid Shots")
							)),

					//Rotation
					Spell.Cast("Disabling Shot",
						ret =>
							Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= Distance.Melee && !DefaultCombat.MovementDisabled),
					Spell.Cast("Rail Shot"),
					Spell.DoT("Incendiary Missile", "", 12000),
					Spell.Cast("Thermal Detonator"),
					Spell.Cast("Electro Net"),
					Spell.Cast("Power Shot")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.CastOnGround("Death from Above", ret => Me.CurrentTarget.Distance > Distance.MeleeAoE),
						Spell.Cast("Fusion Missle", ret => Me.HasBuff("Thermal Sensor Override")),
						Spell.Cast("Explosive Dart"))
					);
			}
		}
	}
}