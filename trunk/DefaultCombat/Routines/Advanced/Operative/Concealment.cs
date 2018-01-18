// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using Buddy.CommonBot;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
using Targeting = DefaultCombat.Core.Targeting;

namespace DefaultCombat.Routines
{
    public class Concealment : RotationBase
    {
        public override string Name
        {
            get { return "Operative Concealment"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Coordination"),
                    Spell.Cast("Stealth", ret => !DefaultCombat.MovementDisabled && !Me.InCombat && !Me.HasBuff("Coordination"))
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 45),
                    Spell.Buff("Stim Boost", ret => Me.BuffCount("Tactical Advantage") < 2),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 75),
                    Spell.Buff("Evasion", ret => Me.HealthPercent <= 50),
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
                    Spell.Cast("Backstab", ret => Me.IsStealthed && Me.IsBehind(Me.CurrentTarget)),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Solo Mode
                    Spell.Cast("Kolto Infusion", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 70),
                    Spell.Cast("Diagnostic Scan", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 60),
                    Spell.Cast("Kolto Probe", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 50),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    new Decorator(ret => Me.ResourcePercent() < 60 && !AbilityManager.CanCast("Adrenaline Probe", Me),
                        new PrioritySelector(
                        Spell.Cast("Rifle Shot")
                            )),
                    new Decorator(
                        ret => (Me.CurrentTarget.HasDebuff("Poisoned (Acid Blade)") || Me.CurrentTarget.HasDebuff("Corrosive Dart")) && !Me.IsStealthed,
                        new PrioritySelector(
                        Spell.Cast("Volatile Substance"))),
                    new Decorator(
                        ret => Me.HasBuff("Tactical Advantage") && !Me.IsStealthed,
                        new PrioritySelector(
                        Spell.Cast("Laceration"))),
                    new Decorator(
                        ret => (!Me.CurrentTarget.HasDebuff("Corrosive Dart") || Me.CurrentTarget.DebuffTimeLeft("Corrosive Dart") <= 2) && !Me.IsStealthed,
                        new PrioritySelector(
                        Spell.Cast("Corrosive Dart"))),
                    new Decorator(
                        ret => !Me.HasBuff("Tactical Advantage") && !Me.IsStealthed,
                        new PrioritySelector(
                        Spell.Cast("Veiled Strike"))),
                    new Decorator(
                        ret => !Me.HasBuff("Tactical Advantage") && !AbilityManager.CanCast("Veiled Strike", Me.CurrentTarget) && Me.IsBehind(Me.CurrentTarget),
                        new PrioritySelector(
                        Spell.Cast("Backstab"))),
                    new Decorator(
                        ret => !Me.HasBuff("Tactical Advantage") && !AbilityManager.CanCast("Veiled Strike", Me.CurrentTarget) && !AbilityManager.CanCast("Backstab", Me.CurrentTarget) && !Me.IsStealthed,
                        new PrioritySelector(
                        Spell.Cast("Crippling Slice"))),
                    new Decorator(
                        ret => !Me.HasBuff("Tactical Advantage") && Me.EnergyPercent >= 87 && !AbilityManager.CanCast("Veiled Strike", Me.CurrentTarget) && !AbilityManager.CanCast("Crippling Slice", Me.CurrentTarget) && !AbilityManager.CanCast("Backstab", Me.CurrentTarget) && !Me.IsStealthed,
                        new PrioritySelector(Spell.Cast("Overload Shot"))),

                    //HK-55 Mode Rotation
                    Spell.Cast("Charging In", ret => Me.CurrentTarget.Distance >= .4f && Me.InCombat && CombatHotkeys.EnableHK55),
                    Spell.Cast("Blindside", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Assassinate", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Rail Blast", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Rifle Blast", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Execute", ret => Me.CurrentTarget.HealthPercent <= 45 && CombatHotkeys.EnableHK55)
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
                        Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
                        Spell.Cast("Fragmentation Grenade"),
                        Spell.Cast("Noxious Knives"),
                        Spell.Cast("Toxic Haze", ret => Me.HasBuff("Tactical Advantage"))
                    ));
            }
        }
    }
}