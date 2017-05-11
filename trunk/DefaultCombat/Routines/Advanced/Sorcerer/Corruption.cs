// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Corruption : RotationBase
	{
		public override string Name
		{
			get { return "Sorcerer Corruption"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Mark of Power")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
        
        Spell.Buff("Unbreakable Will", ret => Me.IsStunned),
        Spell.Buff("Recklessness", ret => Targeting.ShouldAoeHeal),
        Spell.Buff("Consuming Darkness", ret => NeedForce()),
        Spell.Buff("Unnatural Preservation", ret => Me.HealthPercent < 50),
        Spell.HoT("Static Barrier", on => Me, 100, ret => Me.InCombat && !Me.HasDebuff("Deionized")),
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
					Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Force Lightning"),
					Spell.Cast("Crushing Darkness"),
					Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction")),
					Spell.Cast("Lightning Strike"),
					Spell.Cast("Shock"));
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new PrioritySelector(
				
					//Legacy Heroic Moment Ability
					Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
					
					//BuffLog.Instance.LogTargetBuffs

					//Cleanse if needed
					Spell.Cleanse("Purge"),

					//Emergency Heal (Insta-cast)
					Spell.Heal("Dark Heal", 80, ret => Me.HasBuff("Dark Concentration")),

					//Buff Tank
					Spell.HoT("Static Barrier", on => Tank, 100, ret => Tank !=null && Tank.InCombat),
					Spell.Heal("Roaming Mend", onUnit => Tank, 100,	ret => Tank != null && Tank.InCombat && Me.BuffCount("Roaming Mend Charges") <= 1),

					//Single Target Healing
					Spell.HoT("Static Barrier", 99, ret => HealTarget != null && !HealTarget.HasDebuff("Deionized")),
          			Spell.Heal("Innervate", 80),

					//Use Force Bending
					new Decorator(ret => Me.HasBuff("Force Bending"),
						new PrioritySelector(
							Spell.Heal("Innervate", 90),
							Spell.Heal("Dark Infusion", 50)
							)),

					//Build Force Bending
					Spell.HoT("Resurgence", 80),
					Spell.HoT("Resurgence", on => Tank, 100, ret => Tank != null && Tank.InCombat),

					//AoE Healing 
					new Decorator(ctx => Tank != null,
						Spell.CastOnGround("Revivification", on => Tank.Position)),

					//Single Target Healing                  
					Spell.Heal("Dark Heal", 35),
					Spell.Heal("Dark Infusion", 80));
			}
		}

		private bool NeedForce()
		{
			if (Me.HasBuff("Force Surge") && Me.ForcePercent < 80 && !Me.HasBuff("Reverse Corruptions"))
				return true;
			return false;
		}
	}
}
