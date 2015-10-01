// Copyright (C) 2011-2015 Bossland GmbH
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
				return new LockSelector(
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
				return new LockSelector(
					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),
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
				return new LockSelector(
					//Cleanse if needed
					Spell.Cleanse("Restoration"),

					//Emergency Heal (Insta-cast)
					Spell.Heal("Benevolence", 80, ret => Me.HasBuff("Altruism")),

					//Aoe Heal
					Spell.HealGround("Salvation", ret => Targeting.ShouldAoe),

					//Single Target Healing
					Spell.Heal("Healing Trance", 80),
					Spell.Heal("Force Armor", 90,
						ret => HealTarget != null && !HealTarget.HasDebuff("Force-imbalance") && !HealTarget.HasBuff("Force Armor")),

					//Buff Tank
					Spell.Heal("Force Armor", on => Tank, 100,
						ret => Tank != null && Tank.InCombat && !Tank.HasDebuff("Force-imbalance") && !Tank.HasBuff("Force Armor")),
                    Spell.Heal("Wandering Mend", on => Tank, 100, ret => Tank != null && Tank.InCombat && Me.BuffCount("Wandering Mend Charges") <= 1),

					//Use Force Bending
					new Decorator(ret => Me.HasBuff("Conveyance"),
						new LockSelector(
							Spell.Heal("Healing Trance", 90),
							Spell.Heal("Deliverance", 50)
							)),

					//Build Force Bending
					Spell.HoT("Rejuvenate", 80),
					Spell.HoT("Rejuvenate", on => Tank, 100, ret => Tank != null && Tank.InCombat),

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