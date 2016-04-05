// Copyright (C) 2011-2016 Bossland GmbH
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
					Spell.Buff("Armor-piercing Cell"),
					Spell.Buff("Fortification")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Tenacity"),
					Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 70),
					Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
					Spell.Buff("Recharge Cells", ret => Me.ResourceStat <= 40),
					Spell.Buff("Supercharged Cell", ret => Me.BuffCount("Supercharge") == 10),
					Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 60)
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

					//Rotation
					Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
					Spell.Cast("Boltstorm", ret => Me.HasBuff("Curtain of Fire") && Me.Level >= 57),
					Spell.Cast("Full Auto", ret => Me.HasBuff("Curtain of Fire") && Me.Level < 57),
					Spell.Cast("Demolition Round", ret => Me.CurrentTarget.HasDebuff("Gravity Vortex")),
					Spell.Cast("Electro Net"),
					Spell.Cast("Vortex Bolt"),
					Spell.Cast("High Impact Bolt", ret => Me.BuffCount("Charged Barrel") == 5),
					Spell.Cast("Grav Round")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Cast("Tech Override"),
						Spell.CastOnGround("Mortar Volley"),
						Spell.Cast("Plasma Grenade", ret => Me.ResourceStat >= 90 && Me.HasBuff("Tech Override")),
						Spell.Cast("Pulse Cannon", ret => Me.CurrentTarget.Distance <= 1f),
						Spell.CastOnGround("Hail of Bolts", ret => Me.ResourceStat >= 90)
						));
			}
		}
	}
}