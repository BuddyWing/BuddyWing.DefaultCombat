// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Madness : RotationBase
	{
		public override string Name
		{
			get { return "Sorcerer Madness"; }
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
					Spell.Buff("Force Barrier", ret => Me.HealthPercent <= 20),
					Spell.Buff("Static Barrier", ret => !Me.HasBuff("Static Barrier") && !Me.HasDebuff("Deionized")),
					Spell.Buff("Unnatural Preservation", ret => Me.HealthPercent <= 50),
					Spell.Buff("Polarity Shift"),
					Spell.Buff("Recklessness"),
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
					
					//Solo Mode
					Spell.Cast("Resurgence", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 70),
					Spell.Cast("Dark Heal", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 60),
					Spell.Cast("Unnatural Preservation", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 50),
					
					//Rotation
					Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Electrocute", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Demolish", ret => Me.BuffCount("Wrath") == 4),
					Spell.DoT("Affliction", "Affliction"),
					Spell.DoT("Creeping Terror", "Creeping Terror"),
					Spell.CastOnGround("Death Field",	ret => Me.CurrentTarget.HasDebuff("Affliction") && Me.CurrentTarget.HasDebuff("Creeping Terror")),
					Spell.Cast("Force Leach", ret => Me.CurrentTarget.HasDebuff("Affliction")),
					Spell.Cast("Lightning Strike", ret => Me.BuffCount("Wrath") == 4),
					Spell.Cast("Crushing Darkness", ret => Me.HasBuff("Force Flash")),
					Spell.Cast("Force Lightning", ret => Me.BuffCount("Wrath") < 4)
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
						Spell.DoT("Affliction", "Affliction"),
						Spell.DoT("Creeping Terror", "Creeping Terror"),
						Spell.CastOnGround("Death Field",	ret => Me.CurrentTarget.HasDebuff("Affliction") && Me.CurrentTarget.HasDebuff("Creeping Terror")),
						Spell.CastOnGround("Force Storm", ret => Me.CurrentTarget.HasDebuff("Overwhelmed (Mental)"))
						));
			}
		}
	}
}