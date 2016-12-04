// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Tactics : RotationBase
	{
		public override string Name
		{
			get { return "Vanguard Tactics"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Fortification")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Tenacity"),
					Spell.Buff("Recharge Cells", ret => Me.ResourcePercent() <= 50),
					Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 60),
					Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
					Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 80),
					Spell.Buff("Battle Focus")
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					//Movement
					Spell.Cast("Storm", ret => Me.CurrentTarget.Distance >= 1f && !DefaultCombat.MovementDisabled),
					CombatMovement.CloseDistance(Distance.Melee),
					new Decorator(ret => Me.ResourcePercent() > 40,
						new PrioritySelector(
							Spell.Cast("High Impact Bolt", ret => Me.HasBuff("Tactical Accelerator")),
							Spell.Cast("Hammer Shot")
							)),
					Spell.Cast("Riot Strike", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Cell Burst", ret => Me.BuffCount("Energy Lode") == 4),
					Spell.Cast("High Impact Bolt",
						ret => Me.CurrentTarget.HasDebuff("Bleeding (Gut)") && Me.HasBuff("Tactical Accelerator")),
					Spell.DoT("Gut", "Bleeding (Gut)"),
					Spell.Cast("Assault Plastique"),
					Spell.Cast("Stock Strike"),
					Spell.Cast("Tactical Surge", ret => Me.Level >= 26),
					Spell.Cast("Ion Pulse", ret => Me.Level < 26)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new PrioritySelector(
					new Decorator(ret => Targeting.ShouldAoe,
						new PrioritySelector(
							Spell.CastOnGround("Artillery Blitz")
							)),
					new Decorator(ret => Targeting.ShouldPbaoe,
						new PrioritySelector(
							Spell.Cast("Ion Wave", ret => Me.CurrentTarget.Distance <= 1f),
						Spell.Cast("Flak Shell", ret => Me.CurrentTarget.Distance <= 1f),
							Spell.Cast("Explosive Surge"))
						));
			}
		}
	}
}
