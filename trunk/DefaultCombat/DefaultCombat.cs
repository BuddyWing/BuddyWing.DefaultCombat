// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System.Windows;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
using Targeting = DefaultCombat.Core.Targeting;

namespace DefaultCombat
{
	public class DefaultCombat : CombatRoutine
	{
		public static bool IsHealer;
		private static readonly InventoryManagerItem MedPack = new InventoryManagerItem("Medpac", 90);
		private Composite _combat;
		private Composite _ooc;
		private Composite _pull;

		public static bool MovementDisabled
		{
			get { return BotMain.CurrentBot.Name == "Combat Bot"; }
		}

		public static bool Grind
		{
			get { return BotMain.CurrentBot.Name == "Grind Bot"; }
		}

		public override string Name
		{
			get { return "DefaultCombat"; }
		}

		public override Window ConfigWindow
		{
			get { return null; }
		}

		public override CharacterClass Class
		{
			get { return BuddyTor.Me.Class; }
		}

		public override Composite OutOfCombat
		{
			get { return _ooc; }
		}

		public override Composite Pull
		{
			get { return _pull; }
		}

		public override Composite Combat
		{
			get { return _combat; }
		}

		public override void Dispose()
		{
		}

		public override void Initialize()
		{
			Logger.Write("*** Default Combat v74***");
			Logger.Write("Level: " + BuddyTor.Me.Level);
			Logger.Write("Class: " + Class);
			Logger.Write("Advanced Class: " + BuddyTor.Me.AdvancedClass);
			Logger.Write("Discipline: " + BuddyTor.Me.Discipline);

			var f = new RotationFactory();
			var b = f.Build(BuddyTor.Me.Discipline.ToString());

			CombatHotkeys.Initialize();

			if (b == null)
				b = f.Build(BuddyTor.Me.CharacterClass.ToString());

			Logger.Write("Rotation Selected : " + b.Name);

			if (BuddyTor.Me.IsHealer())
			{
				IsHealer = true;
				Logger.Write("Healing Enabled");
			}

			_ooc = new Decorator(ret => !BuddyTor.Me.IsDead && !BuddyTor.Me.IsMounted && !CombatHotkeys.PauseRotation,
				new PrioritySelector(
					Spell.Buff(BuddyTor.Me.SelfBuffName()),
					b.Buffs,
					Rest.HandleRest,
					Scavenge.ScavengeCorpse
					));

			_combat = new Decorator(ret => !CombatHotkeys.PauseRotation,
				new PrioritySelector(
					Spell.WaitForCast(),
					MedPack.UseItem(ret => BuddyTor.Me.HealthPercent <= 30),
					Targeting.ScanTargets,
					b.Cooldowns,
					new Decorator(ret => CombatHotkeys.EnableAoe, b.AreaOfEffect),
					b.SingleTarget));

			_pull = new Decorator(ret => !CombatHotkeys.PauseRotation && !MovementDisabled || IsHealer && !Grind,
				_combat
				);
		}
	}
}