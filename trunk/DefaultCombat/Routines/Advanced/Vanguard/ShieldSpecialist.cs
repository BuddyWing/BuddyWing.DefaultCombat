// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class ShieldSpecialist : RotationBase
	{
		public override string Name
		{
			get { return "Vanguard Shield Specialist"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Ion Cell"),
					Spell.Buff("Fortification"),
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
					Spell.Buff("Tenacity", ret => Me.IsStunned),
					Spell.Buff("Recharge Cells", ret => Me.ResourcePercent() >= 50),
					Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 40),
					Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
					Spell.Buff("Shoulder Cannon", ret => !Me.HasBuff("Shoulder Cannon") && Me.CurrentTarget.BossOrGreater())
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Storm", ret => Me.CurrentTarget.Distance >= 1f && !DefaultCombat.MovementDisabled),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					new Decorator(ret => Me.ResourcePercent() < 60,
						new PrioritySelector(
							Spell.Cast("Energy Blast", ret => Me.BuffCount("Power Screen") == 3),
							Spell.Cast("Pulse Cannon", ret => Me.HasBuff("Pulse Engine") && Me.CurrentTarget.Distance <= 1f),
							Spell.Cast("Explosive Surge", ret => Me.HasBuff("Static Surge") && Me.CurrentTarget.Distance <= 0.5f),
							Spell.Cast("Hammer Shot")
							)),
					Spell.CastOnGround("Smoke Grenade", ret => Me.CurrentTarget.BossOrGreater() && Me.CurrentTarget.Distance <= 0.8f),
					Spell.Cast("Shoulder Cannon", ret => Me.HasBuff("Shoulder Cannon") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Riot Strike",
						ret =>
							Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= Distance.Melee && !DefaultCombat.MovementDisabled),
					Spell.Cast("Energy Blast", ret => Me.BuffCount("Power Screen") == 3),
					Spell.Cast("Stockstrike"),
					Spell.Cast("High Impact Bolt"),
					Spell.Cast("Pulse Cannon", ret => Me.HasBuff("Pulse Engine") && Me.CurrentTarget.Distance <= 1f),
					Spell.Cast("Explosive Surge", ret => Me.HasBuff("Static Surge") && Me.CurrentTarget.Distance <= 0.5f),
					Spell.Cast("Ion Pulse")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.CastOnGround("Mortar Volley"),
						Spell.Cast("Sticky Grenade"),
						Spell.Cast("Pulse Cannon", ret => Me.CurrentTarget.Distance <= 1f),
						Spell.Cast("Explosive Surge")
						));
			}
		}
	}
}