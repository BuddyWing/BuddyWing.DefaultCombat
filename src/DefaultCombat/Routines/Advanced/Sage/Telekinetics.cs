// Copyright (C) 2011-2015 Bossland GmbH
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
					Spell.Buff("Force of Will"),
					Spell.Buff("Force Potency", ret => Me.CurrentTarget.StrongOrGreater()),
					Spell.Buff("Mental Alacrity", ret => Me.CurrentTarget.StrongOrGreater()),
					Spell.Buff("Force Mend", ret => Me.HealthPercent <= 80),
					Spell.HoT("Force Armor", on => Me, 99, ret => !Me.HasDebuff("Force-imbalance") && !Me.HasBuff("Force Armor")),
					Spell.Buff("Vindicate", ret => Me.ForcePercent < 50 && !Me.HasDebuff("Weary"))
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

					//Rotation
					Spell.Cast("Weaken Mind", ret => !Me.CurrentTarget.HasDebuff("Weaken Mind")),
					Spell.Cast("Turbulence", ret => Me.CurrentTarget.HasDebuff("Weaken Mind")),
					Spell.Cast("Mind Crush", ret => Me.HasBuff("Force Gust")),
					Spell.Cast("Telekinetic Gust"),
					Spell.Cast("Telekinetic Wave", ret => Me.HasBuff("Tidal Force")),
					Spell.Cast("Telekinetic Burst", ret => Me.Level >= 57),
					Spell.Cast("Disturbance", ret => Me.Level < 57),
					Spell.Cast("Project")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Cast("Telekinetic Wave", ret => Me.HasBuff("Tidal Force")),
						Spell.CastOnGround("Forcequake")
						));
			}
		}
	}
}