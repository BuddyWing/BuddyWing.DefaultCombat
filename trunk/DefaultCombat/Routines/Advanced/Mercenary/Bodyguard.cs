// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Bodyguard : RotationBase
	{
		public override string Name
		{
			get { return "Mercenary Bodyguard"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Hunter's Boon")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Determination", ret => Me.IsStunned),
					Spell.Buff("Supercharged Gas", ret => Me.BuffCount("Supercharge") == 30 && Me.ResourcePercent() <= 60 && HealTarget != null && HealTarget.HealthPercent <= 80),
					Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 70),
					Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 40),
					Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),
					Spell.Cast("Responsive Safeguards", ret => Me.HealthPercent <= 20),
					Spell.Cast("Unity", ret => Me.HealthPercent <= 15),
					Spell.Cast("Sacrifice", ret => Me.HealthPercent <= 5)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),
					
					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),
					
					//Rotation
					Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Unload"),
					Spell.Cast("Power Shot", ret => Me.ResourceStat >= 70),
					Spell.Cast("Rapid Shots")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
					
						//Legacy Heroic Moment Ability
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
					
						//BuffLog.Instance.LogTargetBuffs,
						Spell.Cleanse("Cure"),

						//Buff Kolto Residue
						new Decorator(ctx => HealTarget != null,
							Spell.CastOnGround("Kolto Missile", on => HealTarget.Position, ret => HealTarget.HealthPercent < 60 && !HealTarget.HasBuff("Kolto Residue"))),

						//Free, so use it!
						Spell.Heal("Emergency Scan", 80),

						//Important Buffs to take advantage of
						new Decorator(ctx => Tank != null,
						Spell.CastOnGround("Kolto Missile", on => Tank.Position, ret => Me.HasBuff("Supercharged Gas") && Me.InCombat && !Tank.HasBuff("Charge Screen"))),
						Spell.Heal("Rapid Scan", 80, ret => Me.HasBuff("Critical Efficiency")),
						Spell.HealGround("Kolto Missile", ret => Me.HasBuff("Supercharged Gas")),
						Spell.Heal("Healing Scan", 80, ret => Me.HasBuff("Supercharged Gas")),

						//Buff Tank
						Spell.Heal("Kolto Shell", on => HealTarget, 100, ret => HealTarget != null && Me.InCombat && !HealTarget.HasBuff("Kolto Shell")),

						//Single Target Priority
						Spell.Heal("Healing Scan", 75),
						Spell.Heal("Rapid Scan", 75),

						//Filler
						Spell.Heal("Rapid Shots", 95, ret => HealTarget != null && HealTarget.Name != Me.Name),
						Spell.Heal("Rapid Shots", on => Tank, 100, ret => Tank != null && Me.InCombat)
						));
			}
		}
	}
}
