// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Gunnery : RotationBase
	{
		public override string Name
		{
			get { return "Commando Gunnery"; }
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
					Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 70),
					Spell.Cast("Echoing Deterrence", ret => Me.HealthPercent <= 30),
					Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
					Spell.Buff("Recharge Cells", ret => Me.ResourceStat <= 40),
					Spell.Buff("Supercharged Cell", ret => Me.BuffCount("Supercharge") == 10),
					Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 60),
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
					Spell.Cast("Hammer Shot", ret => Me.ResourcePercent() < 60),

					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),
					
					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),
					
					//Solo Mode
					Spell.Cast("Med Shot", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 70),
					Spell.Cast("Bacta Infusion", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 60),
					Spell.Cast("Medical Probe", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 50),

					//Rotation
					Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Demolition Round", ret => Me.CurrentTarget.HasDebuff("Gravity Vortex")),
					Spell.Cast("Electro Net"),
					Spell.Cast("High Impact Bolt", ret => Me.BuffCount("Charged Barrel") == 5),
					Spell.Cast("Vortex Bolt"),
					Spell.Cast("Boltstorm", ret => Me.HasBuff("Curtain of Fire")),
					Spell.Cast("Grav Round"),
					Spell.Cast("Hammer Shots")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Mortar Volley", ret => Me.HasBuff("Reserve Powercell")),
						Spell.Cast("Plasma Grenade", ret => Me.ResourceStat >= 90 && Me.HasBuff("Tech Override")),
						Spell.Cast("Sticky Grenade"),
						Spell.CastOnGround("Hail of Bolts", ret => Me.ResourceStat >= 90)
						));
			}
		}
	}
}
