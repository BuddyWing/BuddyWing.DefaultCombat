using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class CombatMedic : RotationBase
	{
		public override string Name
		{
			get { return "Commando Combat Medic"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Fortification"),
					Spell.Buff("Combat Support Cell")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Tenacity"),
					Spell.Buff("Supercharged Cell",
						ret =>
							Me.ResourceStat >= 20 && HealTarget != null && HealTarget.HealthPercent <= 80 &&
							Me.BuffCount("Supercharge") == 10),
					Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
					Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 70),
					Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 60),
					Spell.Buff("Recharge Cells", ret => Me.ResourceStat <= 50),
					Spell.Cast("Tech Override", ret => Tank != null && Tank.HealthPercent <= 50)
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
					Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting),
					Spell.Cast("High Impact Bolt"),
					Spell.Cast("Full Auto"),
					Spell.Cast("Charged Bolts", ret => Me.ResourceStat >= 70),
					Spell.Cast("Hammer Shot")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new LockSelector(
					new Decorator(ret => Me.HasBuff("Supercharged Cell"),
						new LockSelector(
							new Decorator(ctx => Tank != null,
								Spell.CastOnGround("Kolto Bomb", on => Tank.Position, ret => !Tank.HasBuff("Kolto Residue"))),
							Spell.Heal("Bacta Infusion", 60),
							Spell.Heal("Advanced Medical Probe", 85)
							)),

					//Dispel
					Spell.Cleanse("Field Aid"),

					//Keep Trauma Probe on Tank
					Spell.Heal("Trauma Probe", on => HealTarget, 100,
						ret => HealTarget != null && HealTarget.BuffCount("Trauma Probe") <= 1),

					//Kolto Bomb to keep up buff
					new Decorator(ctx => Tank != null,
						Spell.CastOnGround("Kolto Bomb", on => Tank.Position, ret => !Tank.HasBuff("Kolto Residue"))),

					//Single Target Healing
					Spell.Heal("Bacta Infusion", 80),
					Spell.Heal("Advanced Medical Probe", 80, ret => Me.BuffCount("Field Triage") == 3),
					Spell.Heal("Medical Probe", 75),

					//To keep Supercharge buff up; filler heal
					Spell.Heal("Med Shot", on => Tank, 100, ret => Tank != null && Me.InCombat)
					);
			}
		}
	}
}