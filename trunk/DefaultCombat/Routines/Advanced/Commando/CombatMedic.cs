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
					Spell.Buff("Tenacity"),
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
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flamethrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),
					
					//Rotation
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
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
					new Decorator(ret => Me.HasBuff("Supercharged Cell"),
						new PrioritySelector(
							Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 4f), //--will only be active when user initiates Heroic Moment--
							Spell.HealGround("Kolto Bomb", ret => !Tank.HasBuff("Kolto Residue")),
							Spell.CastOnGround("Mortar Volley"),
							Spell.Cast("Sticky Grenade"),
							Spell.Heal("Bacta Infusion", 60),
							Spell.Heal("Advanced Medical Probe", 85)
							)),

					//Dispel 
					Spell.Cleanse("Field Aid"),

					//AoE Healing 
					Spell.HealAoe("Successive Treatment", ret => Targeting.ShouldAoeHeal),
					Spell.HealGround("Kolto Bomb", ret => !Tank.HasBuff("Kolto Residue")),

					//Single Target Healing 
					Spell.Heal("Bacta Infusion", 80),
					Spell.Heal("Advanced Medical Probe", 80),
					Spell.Heal("Medical Probe", 75),

					//Keep Trauma Probe on Tank 
					Spell.HoT("Trauma Probe", 100),

					//To keep Supercharge buff up; filler heal 
					Spell.Heal("Med Shot", onUnit => Tank, 100, ret => Tank != null && Me.InCombat)
					);
			}
		}
	}
}
