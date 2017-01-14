// Copyright (C) 2011-2016 Bossland GmbH
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
					Spell.Buff("Heroic Moment", ret => Me.CurrentTarget.BossOrGreater()),
          Spell.Buff("Unbreakable Will", ret => Me.IsStunned),
          Spell.Buff("Force Barrier", ret => Me.HealthPercent <= 20),
					Spell.Buff("Static Barrier", ret => !Me.HasBuff("Static Barrier") && !Me.HasDebuff("Deionized")),
					Spell.Buff("Unnatural Preservation", ret => Me.HealthPercent <= 50),
					Spell.Buff("Polarity Shift", ret => Me.CurrentTarget.BossOrGreater()),
					Spell.Buff("Recklessness")
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
					Spell.Cast("Electrocute", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
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
						Spell.DoT("Affliction", "Affliction"),
						Spell.DoT("Creeping Terror", "Creeping Terror"),
						Spell.CastOnGround("Death Field",	ret => Me.CurrentTarget.HasDebuff("Affliction") && Me.CurrentTarget.HasDebuff("Creeping Terror")),
						Spell.CastOnGround("Force Storm", ret => Me.CurrentTarget.HasDebuff("Overwhelmed (Mental)"))
						));
			}
		}
	}
}