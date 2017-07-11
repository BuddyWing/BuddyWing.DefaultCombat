// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Telekinetics : RotationBase
	{
		public override string Name
		{
			get { return "Sage Telekinetics"; }
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
					Spell.Buff("Force of Will", ret => Me.IsStunned),
					Spell.Buff("Force Potency", ret => Me.CurrentTarget.StrongOrGreater()),
					Spell.Buff("Mental Alacrity", ret => Me.CurrentTarget.StrongOrGreater()),
					Spell.Buff("Force Mend", ret => Me.HealthPercent <= 80),
					Spell.HoT("Force Armor", on => Me, 99, ret => !Me.HasDebuff("Force-imbalance") && !Me.HasBuff("Force Armor")),
					Spell.Buff("Vindicate", ret => Me.ForcePercent < 50 && !Me.HasDebuff("Weary")),
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
					Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),
					
					//Solo Mode
					Spell.Cast("Rejuvenate", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 70),
					Spell.Cast("Benevolence", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 60),
					Spell.Cast("Force Mend", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 50),

					//Rotation
					Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Force Stun", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Weaken Mind", ret => !Me.CurrentTarget.HasDebuff("Weaken Mind")),
					Spell.Cast("Turbulence", ret => Me.CurrentTarget.HasDebuff("Weaken Mind")),
					Spell.Cast("Mind Crush", ret => Me.HasBuff("Force Gust")),
					Spell.Cast("Telekinetic Gust"),
					Spell.Cast("Telekinetic Wave", ret => Me.HasBuff("Tidal Force")),
					Spell.Cast("Telekinetic Burst", ret => Me.Level >= 57),
					Spell.Cast("Disturbance", ret => Me.Level < 57),
					Spell.Cast("Project"),
					Spell.Cast("Telekinetic Throw")
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
						Spell.Cast("Telekinetic Wave", ret => Me.HasBuff("Tidal Force")),
						Spell.CastOnGround("Forcequake")
						));
			}
		}
	}
}