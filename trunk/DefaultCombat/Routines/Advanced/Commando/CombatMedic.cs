// Copyright (C) 2011-2017 Bossland GmbH 
// See the file LICENSE for the source code's detailed license 

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
					Spell.Buff("Fortification")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Tenacity", ret => Me.IsStunned),
					Spell.Buff("Supercharged Cell",	ret => Me.ResourceStat >= 20 && HealTarget != null && HealTarget.HealthPercent <= 80 &&	Me.BuffCount("Supercharge") == 10),
					Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
					Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 70),
					Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 60),
					Spell.Buff("Recharge Cells", ret => Me.ResourceStat <= 50),
					Spell.Cast("Tech Override", ret => Tank != null && Tank.HealthPercent <= 50),
					Spell.Cast("Echoing Deterrence", ret => Me.HealthPercent <= 30)
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
					Spell.Cast("Legacy Flamethrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),
					
					//Rotation
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
				return new PrioritySelector(
				
					//AoE Healing 
					new Decorator(ctx => Tank != null,
						Spell.CastOnGround("Kolto Bomb", on => Tank.Position, ret => !Tank.HasBuff("Invigorated"))),
					
					//Legacy Heroic Moment Ability
					Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
					
					//Dispel 
					Spell.Cleanse("Field Aid"),
					
					//Single Target Healing 
					Spell.Heal("Bacta Infusion", 80),
					Spell.Heal("Successive Treatment", 90),
					Spell.Heal("Advanced Medical Probe", 80),
					Spell.Heal("Medical Probe", 75),

					//Keep Trauma Probe on Tank 
					Spell.Heal("Trauma Probe", on => HealTarget, 100, ret => HealTarget != null && HealTarget.BuffCount("Trauma Probe") <= 1),

					//To keep Supercharge buff up; filler heal 
					Spell.Heal("Med Shot", onUnit => Tank, 100, ret => Tank != null && Me.InCombat)
					);
			}
		}
	}
}
