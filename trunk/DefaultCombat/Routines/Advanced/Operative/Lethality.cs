// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using Buddy.CommonBot;
using DefaultCombat.Core;
using DefaultCombat.Helpers;
using Targeting = DefaultCombat.Core.Targeting;

namespace DefaultCombat.Routines
{
    public class Lethality : RotationBase
    {
        public override string Name
        {
            get { return "Operative Lethality"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(

                    Spell.Buff("Coordination"),
                    Spell.Cast("Stealth", ret => !Me.InCombat && !Me.HasBuff("Coordination"))
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
                    Spell.Buff("Stim Boost", ret => !Me.HasBuff("Tactical Advantage") && Me.InCombat),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 80),
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
                    Spell.Cast("Holotraverse", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
                    Spell.Cast("Lethal Strike", ret => (Me.IsStealthed)),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Energy
                    new Decorator(ret => Me.EnergyPercent < 35,
                        new PrioritySelector(
                            Spell.Cast("Rifle Shot")
                            )),

                    //Solo Mode
                    Spell.Cast("Kolto Infusion", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 90 && Me.HasBuff("Quickening")),
                    Spell.Cast("Diagnostic Scan", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 10),
                    Spell.Cast("Kolto Probe", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 80 && Me.BuffTimeLeft("Kolto Probe") <= 3),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Toxic Blast", ret => Me.CurrentTarget.HasDebuff("Corrosive Dart") && Me.CurrentTarget.HasDebuff("Corrosive Grenade")),
                    Spell.Cast("Corrosive Assault", ret => Me.CurrentTarget.HasDebuff("Corrosive Dart") && Me.CurrentTarget.HasDebuff("Corrosive Grenade") && Me.HasBuff("Tactical Advantage")),
                    Spell.Cast("Corrosive Dart", ret => !Me.CurrentTarget.HasDebuff("Corrosive Dart") || Me.CurrentTarget.DebuffTimeLeft("Corrosive Dart") <= 3),
                    Spell.Cast("Corrosive Grenade", ret => !Me.CurrentTarget.HasDebuff("Corrosive Grenade") || Me.CurrentTarget.DebuffTimeLeft("Corrosive Grenade") <= 3),
                    Spell.Cast("Shiv", ret => Me.BuffCount("Tactical Advantage") < 2 || Me.BuffTimeLeft("Tactical Advantage") < 6),
                    Spell.Cast("Lethal Strike", ret => Me.Level >= 57),
                    Spell.Cast("Back Stab", ret => Me.IsBehind(Me.CurrentTarget) && Me.Level < 57),
                    Spell.Cast("Overload Shot", ret => Me.EnergyPercent > 85 && !Me.HasBuff("Tactical Advantage")),
                    Spell.Cast("Rifle Shot", ret => Me.EnergyPercent < 65 && !Me.HasBuff("Tactical Advantage") || Me.CurrentTarget.Distance > 1f)
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
                        Spell.DoT("Corrosive Grenade", "Corrosive Grenade"),
                        Spell.Cast("Fragmentation Grenade"),
                        Spell.Cast("Noxious Knives"),
                        Spell.Cast("Toxic Haze", ret => Me.HasBuff("Tactical Advantage"))
                        ));
            }
        }
    }
}
