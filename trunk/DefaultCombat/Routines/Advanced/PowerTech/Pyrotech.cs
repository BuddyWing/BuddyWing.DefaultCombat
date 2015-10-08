// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Pyrotech : RotationBase
	{
		public override string Name
		{
			get { return "PowerTech Pyrotech"; }
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
					Spell.Buff("Determination"),
					Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 65),
					Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 60),
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
					CombatMovement.CloseDistance(Distance.Melee),
					new Decorator(ret => Me.ResourcePercent() > 40,
						new LockSelector(
							Spell.Cast("Flame Burst", ret => Me.HasBuff("Flame Barrage")),
							Spell.Cast("Rapid Shots")
							)),
                    Spell.Cast("Quell", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
                    Spell.Cast("Flame Thrower", ret => Me.BuffCount("Superheated Flame Thrower") == 2),
					Spell.DoT("Incendiary Missile", "Burning (Incendiary Missile)"),
					Spell.DoT("Scorch", "Scorch"),
					Spell.Cast("Rail Shot", ret => Me.HasBuff("Charged Gauntlets")),
					Spell.Cast("Immolate"),
					Spell.Cast("Flaming Fist"),
					Spell.Cast("Flame Burst")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new LockSelector(
					new Decorator(ret => Targeting.ShouldAoe,
						new LockSelector(
							Spell.DoT("Scorch", "Scorch"),
							Spell.CastOnGround("Death from Above"),
							Spell.Cast("Explosive Dart")
							)),
					new Decorator(ret => Targeting.ShouldPbaoe,
						new LockSelector(
							Spell.DoT("Scorch", "Scorch"),
							Spell.Cast("Flame Thrower"),
							Spell.Cast("Flame Sweep")
							)));
			}
		}
	}
}