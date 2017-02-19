// Copyright (C) 2011-2016 Bossland GmbH 
// See the file LICENSE for the source code's detailed license 

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Seer : RotationBase
	{
		public override string Name
		{
			get { return "Sage Seer"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Force Valor")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					// Spell.Buff("Heroic Moment", ret => Me.CurrentTarget.BossOrGreater()), == commented out due to BossorGreater detection broken in last few releases of bot
					Spell.Buff("Force Potency", ret => Targeting.ShouldAoeHeal),
					Spell.Buff("Mental Alacrity", ret => Targeting.ShouldAoeHeal),
					Spell.Buff("Vindicate", ret => NeedForce()),
					Spell.Buff("Force Mend", ret => Me.HealthPercent <= 75)
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
					Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Force Stun", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Forcequake", ret => Targeting.ShouldAoe),
					Spell.Cast("Mind Crush"),
					Spell.DoT("Weaken Mind", "Weaken Mind"),
					Spell.Cast("Project"),
					Spell.Cast("Telekinetic Throw"),
					Spell.Cast("Disturbance")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new PrioritySelector(
					//Cleanse if needed 
					Spell.Cleanse("Restoration"),

					//Emergency Heal (Insta-cast) 
					Spell.Heal("Benevolence", 80, ret => Me.HasBuff("Altruism")),

					//Aoe Heal 
					Spell.HealGround("Salvation", ret => Targeting.ShouldAoeHeal),

					//Single Target Healing 
					Spell.Heal("Healing Trance", 80),
					Spell.HoT("Force Armor", 90, ret => HealTarget != null && !HealTarget.HasDebuff("Force-imbalance")),

					//Buff Tank 
					Spell.HoT("Force Armor", onUnit => Tank, 100,	ret => Tank != null && Tank.InCombat && !Tank.HasDebuff("Force-imbalance")),
					Spell.Heal("Wandering Mend", onUnit => Tank, 100,	ret => Tank != null && Tank.InCombat && Me.BuffCount("Wandering Mend Charges") <= 1),

					//Use Force Bending 
					new Decorator(ret => Me.HasBuff("Conveyance"),
						new PrioritySelector(
							Spell.Heal("Healing Trance", 90),
							Spell.Heal("Deliverance", 50)
							)),

					//Build Force Bending 
					Spell.HoT("Rejuvenate", 80),
					Spell.HoT("Rejuvenate", onUnit => Tank, 100, ret => Tank != null && Tank.InCombat),

					//Single Target Healing                   
					Spell.Heal("Benevolence", 35),
					Spell.Heal("Deliverance", 80)
					);
			}
		}

		private bool NeedForce()
		{
			if (Me.ForcePercent <= 20)
				return true;
			if (Me.HasBuff("Resplendence") && Me.ForcePercent < 80 && !Me.HasBuff("Amnesty"))
				return true;
			return false;
		}
	}
}