// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    internal class Sharpshooter : RotationBase
    {
        public override string Name
        {
            get { return "Gunslinger Sharpshooter"; }
        }


        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Lucky Shots")
                    );
            }
        }


        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                  Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Burst Volley", ret => !Buddy.CommonBot.AbilityManager.CanCast("Penetrating Rounds", Me.CurrentTarget)),
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Dodge", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 50),
                    Spell.Buff("Smuggler's Luck"),
                    Spell.Buff("Illegal Mods"),
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


                    //Low Energy
                    new Decorator(ret => Me.EnergyPercent < 60 && !Buddy.CommonBot.AbilityManager.CanCast("Cool Head", Me),
                        new PrioritySelector(
                            Spell.Cast("Flurry of Bolts")
                            )),


                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),


                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Trickshot"),
                    Spell.Cast("Quickdraw", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Penetrating Rounds", ret => Me.Level >= 26),
                    Spell.Cast("Speed Shot", ret => Me.Level < 26),
                    new Decorator(ret => Me.BuffCount("Charged Aim") == 2,
                        new PrioritySelector(
                            Spell.Cast("Aimed Shot")
                            )),
                    Spell.Cast("Vital Shot", ret => !Me.CurrentTarget.HasMyDebuff("Vital Shot") || Me.CurrentTarget.DebuffTimeLeft("Vital Shot") <= 2),
                    Spell.Cast("Charged Burst"),

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
                        Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
                        Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
                        Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
                        Spell.CastOnGround("XS Freighter Flyby", ret => Me.IsInCover() && Me.EnergyPercent > 30),
                        Spell.Cast("Thermal Grenade"),
                        Spell.CastOnGround("Sweeping Gunfire", ret => Me.IsInCover() && Me.EnergyPercent > 10)
                        ));
            }
        }
    }
}