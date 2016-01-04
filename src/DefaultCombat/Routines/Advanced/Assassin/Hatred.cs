// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class Hatred : RotationBase
	{
		public override string Name
		{
			get { return "Assasin Hatred"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Lightning Charge"),
					Spell.Buff("Mark of Power"),
					Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled)
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Unbreakable Will", ret => Me.IsStunned),
					Spell.Buff("Overcharge Saber", ret => Me.HealthPercent <= 85),
					Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
					Spell.Buff("Force Shroud", ret => Me.HealthPercent <= 50),
					Spell.Buff("Recklessness")
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Force Speed",
						ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Low Force
                    Spell.Cast("Saber Strike", ret => Me.ForcePercent <= 40),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.CastOnGround("Death Field"),
                    Spell.Cast("Assassinate", ret => Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("Bloodletting")),
                    Spell.Cast("Demolish", ret => Me.HasBuff("Raze") && Me.Level >= 57),
                    Spell.Cast("Crushing Darkness", ret => Me.HasBuff("Raze") && Me.Level < 57),
                    Spell.DoT("Discharge", "Shocked (Discharge)"),
					Spell.DoT("Creeping Terror", "Creeping Terror"),
                    Spell.Cast("Leeching Strike"),
					Spell.Cast("Thrash"),
					Spell.Buff("Force Speed", ret => Me.CurrentTarget.Distance >= 1.1f && Me.IsMoving && Me.InCombat)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                    Spell.DoT("Discharge", "Discharge"),
					Spell.DoT("Creeping Terror", "Creeping Terror"),
					Spell.CastOnGround("Death Field"),
					Spell.Cast("Lacerate", ret => Me.CurrentTarget.HasDebuff("Discharge") && Me.CurrentTarget.HasDebuff("Creeping Terror") && Me.ForcePercent >= 60 && Targeting.ShouldPbaoe)
					));
			}
		}
	}
}