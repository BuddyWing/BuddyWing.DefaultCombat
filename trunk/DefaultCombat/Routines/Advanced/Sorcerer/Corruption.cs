// Copyright (C) 2011-2016 Bossland GmbH
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
					// Spell.Buff("Heroic Moment", ret => Me.CurrentTarget.BossOrGreater()), == commented out due to BossorGreater detection broken in last few releases of bot
          Spell.Buff("Unbreakable Will", ret => Me.IsStunned),
					Spell.Buff("Recklessness", ret => Targeting.ShouldAoeHeal),
					Spell.Buff("Consuming Darkness", ret => NeedForce()),
					Spell.Buff("Unnatural Preservation", ret => Me.HealthPercent < 50)
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
					
					//Legacy Heroic Moment Abilities
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Flamethrower", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.BossOrGreater()),

					//Rotation
					Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
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
					//BuffLog.Instance.LogTargetBuffs,

					//Cleanse if needed
					Spell.Cleanse("Purge"),

					//Emergency Heal (Insta-cast)
					Spell.Heal("Dark Heal", 80, ret => Me.HasBuff("Dark Concentration")),

					//Single Target Healing
					Spell.Heal("Innervate", 80),
					Spell.HoT("Static Barrier", 75, ret => HealTarget != null && !HealTarget.HasDebuff("Deionized")),

					//Buff Tank
					Spell.HoT("Static Barrier", on => Tank, 100, ret => Tank != null && Tank.InCombat && !Tank.HasDebuff("Deionized")),

					//Use Force Bending
					new Decorator(ret => Me.HasBuff("Force Bending"),
						new PrioritySelector(
							Spell.Heal("Innervate", 90),
							Spell.Heal("Dark Infusion", 50)
							)),

					//Build Force Bending
					Spell.HoT("Resurgence", 80),
					Spell.HoT("Resurgence", on => Tank, 100, ret => Tank != null && Tank.InCombat),

					//Aoe Heal
					Spell.HealGround("Revivification"),

					//Single Target Healing                  
					Spell.Heal("Dark Heal", 35),
					Spell.Heal("Dark Infusion", 80));
			}
		}

		private bool NeedForce()
		{
			if (Me.ForcePercent <= 20)
				return true;
			if (Me.HasBuff("Force Surge") && Me.ForcePercent < 80 && !Me.HasBuff("Reverse Corruptions"))
				return true;
			return false;
		}
	}
}