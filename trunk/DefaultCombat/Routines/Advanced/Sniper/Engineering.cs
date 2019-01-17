// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Engineering : RotationBase
    {
        public override string Name
        {
            get { return "Sniper Engineering"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Coordination")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 45),
                    Spell.Buff("Laze Target"),
                    Spell.Buff("Target Acquired"),
                    Spell.Cast("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),


                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Energy
                    new Decorator(ret => Me.EnergyPercent < 30,
                        new PrioritySelector(
                            Spell.Cast("Fragmentation Grenade", ret => Me.HasBuff("Energy Overrides")),
                            Spell.Cast("Rifle Shot")
                            )),

                    //Burn-ish
                    new Decorator(ret => Me.CurrentTarget.HealthPercent < 30,
                        new PrioritySelector(
                            Spell.Cast("Explosive Probe"),
                            Spell.Cast("Series of Shots"),
                            Spell.DoTGround("Plasma Probe", 8500),
                            Spell.DoT("Interrogation Probe", "Interrogation Probe"),
                            Spell.Cast("Fragmentation Grenade", ret => Me.HasBuff("Energy Overrides")),
                            Spell.Cast("EMP Discharge", ret => Me.CurrentTarget.HasDebuff("Interrogation Probe")),
                            Spell.Cast("Corrosive Dart", ret => !Me.CurrentTarget.HasDebuff("Marked [Physical]")),
                            Spell.Cast("Takedown")
                            )),


                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Explosive Probe"),
                    Spell.Cast("Series of Shots"),
                    Spell.DoTGround("Plasma Probe", 8500),
                    Spell.Cast("Fragmentation Grenade", ret => Me.HasBuff("Energy Overrides")),
                    Spell.DoT("Interrogation Probe", "Interrogation Probe"),
                    Spell.Cast("EMP Discharge", ret => Me.CurrentTarget.HasDebuff("Interrogation Probe")),
                    Spell.CastOnGround("Orbital Strike"),
                    Spell.DoT("Corrosive Dart", "Corrosive Dart"),
                    Spell.Cast("Fragmentation Grenade"),
                    Spell.Cast("Rifle Shot"),
                    Spell.Cast("Snipe")
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
                        Spell.CastOnGround("Orbital Strike", ret => Me.IsInCover() && Me.EnergyPercent > 30),
                        Spell.DoTGround("Plasma Probe", 9000),
                        Spell.Cast("Suppressive Fire", ret => Me.IsInCover() && Me.EnergyPercent > 10)
                        ));
            }
        }
    }
}
