using System;
using System.Collections.Generic;
using System.Linq;

using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using DefaultCombat.Dynamics;
using Action = Buddy.BehaviorTree.Action;
namespace DefaultCombat.Routines.Advanced.Sniper
{
    /**************************************************************************************************************************************************************
     * http://mmo-mechanics.com/swtor/forums/Thread-Marksmanship-Sniper-Sharpshooter-Gunslinger-Compendium#Quick_Answer_Rotation
     * Snipe – FT
     *  This is the defining sequence for MM snipers. 
     *  Followthrough has such a high DPE (nearly double that of Ambush and SoS) that we should aim to use it every time it comes off cooldown. 
     *  In order to allow that, Snipe (or Ambush when available) must be used at least once every 6s.
     * 
     * SoS – Ambush
     *  Because Series of Shots hits four times over its duration, it will very often proc Reactive Shot, which in turn reduces the activation time of the subsequent Ambush. 
     *  Since the RS effect lasts 10 seconds, a Snipe used during a previous sequence can increase the proc chance of RS even further. 
     *  With a 40% crit chance on Snipe and SoS, this sequence (including the previous Snipe) will proc RS approximately 92% of the time.
     * 
     * TA – SoS – RF – SoS – SoS
     *  The sequence is only available to pure MM snipers. 
     *  The optimal benefit from RF occurs when it is used just after a SoS, finishing its remaining cooldown and allowing two more immediately following. 
     *  This sequence delays FT by 6s beyond its cooldown, but when TA can be used before the initial SoS, the delay is reduced to 3.9s.
     * 
     * 
     * SoS – Ambush – FT – X – X – Snipe – FT – X – X – Snipe – FT
     * 
     * Note that we should use Ambush even when Reactive Shot does not proc. Its damage advantage over Snipe justifies even a 2.5-second activation time.
     * http://mmo-mechanics.com/swtor/forums/Thread-Marksmanship-Sniper-Sharpshooter-Gunslinger-Discussion-1-0-1-2?pid=14175#pid14175
     * 
     **/



    //currently being developed by xsol
    //edited: 5/18/2012
    //testing: lvl 40 sniper
    public class Marksmanship
    {
        private static string aiMode = "10-36"; //10-36, 37-48, 49-50
        private static string dmgMode = "AoE"; //AoE, ST, ?

        //this attempts to use a simple state engine to determine situations/actions/re-actions
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Agent, AdvancedClass.Sniper, SkillTreeId.SniperMarksmanship)]
        public static Composite MarksmanshipCombat()
        {
            return new PrioritySelector(
                //we need energy
                Sniper.Cast("Rifle Shot", onUnit => BuddyTor.Me.CurrentTarget, ret => BuddyTor.Me.ResourceStat <= (Global.EnergyMin - 20)),
                //determine lvl state
                new Decorator(
                    runWhen =>
                        (BuddyTor.Me.Level <= 36 || !AbilityManager.HasAbility("Series of Shots")) && aiMode != "10-36",
                    new Sequence(new Action(atn => aiMode = "10-36"))
                ),
                new Decorator(
                    runWhen =>
                        (BuddyTor.Me.Level >= 36 && AbilityManager.HasAbility("Series of Shots")) && aiMode != "37-48",
                    new Sequence(new Action(atn => aiMode = "37-48"))
                ),
                new Decorator(
                    runWhen =>
                        (BuddyTor.Me.Level >= 48 && AbilityManager.HasAbility("Orbital Strike")) && aiMode != "49-50",
                    new Sequence(new Action(atn => aiMode = "49-50"))
                ),
                //determine targeting mode
                new Decorator(
                    runWhen =>
                        ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= Global.RangeDist &&
                            !o.IsDead &&
                            o.IsHostile) >= 3 && dmgMode != "AoE",
                    new Sequence(new Action(atn => dmgMode = "AoE"))
                ),
                new Decorator(
                    runWhen =>
                        ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= Global.RangeDist &&
                            !o.IsDead &&
                            o.IsHostile) <= 2 && dmgMode != "ST",
                    new Sequence(new Action(atn => dmgMode = "ST"))
                ),
                Sniper.SniperSelfCheck,

                //return the current rotation based on lvl and mode
                new Decorator(
                    ret => 
                        (aiMode == "49-50" && dmgMode == "AoE"),
                    OpsRotation.AoE
                ),
                new Decorator(
                    ret =>
                        (aiMode == "49-50" && dmgMode == "ST"),
                    OpsRotation.Ops
                ),
                new Decorator(
                    ret =>
                        (aiMode == "37-48" && dmgMode == "AoE"),
                    OpsRotation.AoE
                ),
                new Decorator(
                    ret =>
                        (aiMode == "37-48" && dmgMode == "ST"),
                    OpsRotation.Ops
                ),
                new Decorator(
                    ret =>
                        (aiMode == "10-36" && dmgMode == "AoE"),
                    Sniper.LowLVL_AoE
                ),
                new Decorator(
                    ret =>
                        (aiMode == "10-36" && dmgMode == "ST"),
                    Sniper.LowLVL_ST
                )
            );
        }

        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Agent, AdvancedClass.Sniper, SkillTreeId.SniperMarksmanship)]
        public static Composite MarksmanshipPull()
        {
            return Marksmanship.MarksmanshipCombat();
        }

        #region MCV01
        public static PrioritySelector MVC01 = new PrioritySelector(
            Spell.WaitForCast(),
            //cast this every time it is available
            Sniper.Cast("Followthrough",
                castWhen =>
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    Global.IsInCover &&
                    !(BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)"))
            ),
            Sniper.Cast("Escape ", castWhen => BuddyTor.Me.IsStunned || AbilityManager.CanCast("Escape", BuddyTor.Me)),
            Movement.StopInRange(Global.RangeDist),
            //knock back targets
            Sniper.Cast("Cover Pulse",
                castWhen =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.Position, o.Position) <= 0.5f &&
                            !o.IsDead &&
                            !o.IsStunned &&
                            !o.HasDebuff("Blinded (Tech)") &&
                            o.IsHostile
                    ) >= 3 && !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")
            ),
            //interrupt
            Sniper.Cast("Distraction",
                castWhen =>
                    BuddyTor.Me.CurrentTarget.IsCasting &&
                    BuddyTor.Me.CurrentTarget.Distance >= Global.MeleeDist),
            // AoE Grenade, High Damage //this may need to be moved down again
            Sniper.Cast("Fragmentation Grenade",
                castWhen =>
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= Global.MeleeDist &&
                            !o.IsDead &&
                            o.IsHostile &&
                            (o.HasDebuff("Blinded(Tech)") || o.IsStunned)
                    ) <= 2 &&
                    !(BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Blinded(Tech)"))
            ),
            //we need to be in cover
            Sniper.Cast("Crouch",
                onUnit =>
                    BuddyTor.Me,
                castWhen =>
                    (BuddyTor.Me.CurrentTarget.InLineOfSight &&
                    !Global.IsInCover &&
                    BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist)
            ),
            //rotation opener
            Sniper.Cast("Series of Shots",
                ret =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    !(BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)"))
            ),
            //always follow SoS with ambush
            Sniper.Cast("Ambush",
                castWhen =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    !(BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Blinded(Tech)")) &&
                    !AbilityManager.CanCast("Series of Shots", BuddyTor.Me.CurrentTarget) &&
                    !(AbilityManager.CanCast("Followthrough", BuddyTor.Me.CurrentTarget) || AbilityManager.CanCast("Takedown", BuddyTor.Me.CurrentTarget))
            ),
            // Medium DPS Grenade, place this every time it is available
            Sniper.Cast("Explosive Probe",
                castWhen => Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin + 20 &&
                    !BuddyTor.Me.CurrentTarget.IsStunned
            ),
            //finisher, below 30% health
            Sniper.Cast("Takedown",
                castWhen =>
                    AbilityManager.CanCast("Takedown", BuddyTor.Me.CurrentTarget) &&
                    !BuddyTor.Me.CurrentTarget.IsStunned),
            Sniper.Cast("Shatter Shot",
                castWhen => Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin + 20 &&
                    !BuddyTor.Me.CurrentTarget.IsStunned
            ),
            // attempt to debuff any target that can be debuffed and are a "real" threat
            Sniper.Cast("Diversion",
                castWhen => (
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Weak &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Standard &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.RaidBoss) &&
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            //buff snipe to ensure a crit
            Sniper.Cast("Laze Target",
                ret =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin
            ),
            //snipe only when we are not in another FT rotation segment or ready to begin one
            Sniper.Cast("Snipe",
                ret =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    !AbilityManager.CanCast("Followthrough", BuddyTor.Me.CurrentTarget) &&
                    !(AbilityManager.CanCast("Series of Shots", BuddyTor.Me.CurrentTarget) || AbilityManager.CanCast("Ambush", BuddyTor.Me.CurrentTarget))
            ),
            //well, everything can't be cced all the time
            Sniper.Cast("Headshot",
                castWhen =>
                    (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard) &&
                    (BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)"))
            ),
            //CC
            Sniper.Cast("Flash Bang",
                castWhen =>
                    BuddyTor.Me.HealthPercent < Global.HealthCritical ||
                    ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= 0.5 &&
                            !o.IsDead &&
                            !o.IsStunned
                    ) >= 2
            ),
            //melee range cc
            Sniper.Cast("Debilitate",
                castWhen =>
                    (BuddyTor.Me.CurrentTarget.IsCasting || BuddyTor.Me.HealthPercent <= Global.MidHealth) &&
                    BuddyTor.Me.CurrentTarget.Distance <= Global.MeleeDist),
            //enery quick fix
            Sniper.Cast("Adrenaline Probe",
                castWhen =>
                    BuddyTor.Me.ResourceStat <= Global.EnergyMin
            ),
            //defense
            Sniper.Cast("Shield Probe",
                castWhen =>
                    BuddyTor.Me.HealthPercent <= Global.HealthShield
            ),
            //AoE spray bullets
            Sniper.CastOnGround("Suppressive Fire",
                castWhen =>
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= Global.MeleeDist &&
                            !o.IsDead &&
                            !o.IsStunned &&
                            o.IsHostile &&
                            !o.HasDebuff("Blinded (Tech)")
                    ) >= 2 && !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)"),
                ret =>
                    BuddyTor.Me.CurrentTarget.Position
            ),
            //knock back targets
            Sniper.Cast("Cover Pulse",
                castWhen =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.Position, o.Position) <= 0.5f &&
                            !o.IsDead &&
                            !o.IsStunned &&
                            !o.HasDebuff("Blinded (Tech)") &&
                            o.IsHostile
                    ) >= 1 && !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")
            ),
            Sniper.Cast("Rifle Shot",
                castWhen =>
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.RangeDist)
        );
        #endregion

        #region MPGV01
        /// <summary>
        /// Group pull variant.
        /// </summary>
        public static PrioritySelector MPGV01 = new PrioritySelector(
                Sniper.Cast("Flash Bang",
                    castWhen =>
                        BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                        ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                            o =>
                                Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= Global.MeleeDist &&
                                !o.IsDead &&
                                !o.IsStunned &&
                                o.IsHostile &&
                                !o.HasDebuff("Blinded (Tech)")
                        ) >= 2 && !BuddyTor.Me.CurrentTarget.IsStunned &&
                        !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")
                ),
                // AoE Grenade, High Damage //this may need to be moved down again
                Sniper.Cast("Fragmentation Grenade",
                    castWhen =>
                        BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                        ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                            o =>
                                Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= Global.MeleeDist &&
                                !o.IsDead &&
                                !o.IsStunned &&
                                o.IsHostile &&
                                !o.HasDebuff("Blinded(Tech)")
                        ) >= 2 && !BuddyTor.Me.CurrentTarget.IsStunned &&
                        !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded(Tech)")
                ),
                Sniper.Cast("Series of Shots",
                    ret =>
                            Global.IsInCover &&
                            BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                            !BuddyTor.Me.CurrentTarget.IsStunned &&
                        !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")
                ),
                // Medium DPS Shot, this is our main goto dmg and setup skill
                Sniper.Cast("Snipe",
                    ret =>
                        Global.IsInCover &&
                        BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                        !BuddyTor.Me.CurrentTarget.IsStunned &&
                        !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")
                ),
                Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.RangeDist),
                Sniper.Cast("Crouch", onUnit => BuddyTor.Me, castWhen => BuddyTor.Me.CurrentTarget.InLineOfSight && !Global.IsInCover && BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist),
                MarksmanshipCombat(),
                Sniper.SniperPull()
            );
        #endregion

        #region OpsRotation
        /// <summary>
        /// I have been having some trouble locking down the behaviours O wanted to get from the CC, but I think I have taken a step in the right direction.
        /// The idea being tested here is simply getting skills to cast in a fixed order. The next thing to do then becomes checking the success of a skill cast.
        /// </summary>
        public static class OpsRotation
        {
            private static Random ran = new Random();
            private static void log(string s) { Logger.Write(" - MarksmanshipOps: " + s); }
            private static DateTime LastBD = DateTime.Now;

            #region AoE
            public static PrioritySelector AoE = new PrioritySelector(
                Movement.StopInRange(Global.RangeDist),
                Spell.WaitForCast(),
                //we need to crouch
                Sniper.Cast("Crouch", onUnit => BuddyTor.Me, castWhen => BuddyTor.Me.CurrentTarget.InLineOfSight && !Global.IsInCover && BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist),
                //should handle any weird need to reset rotation issues (or cause some)
                new Decorator(
                    runWhen =>
                        DateTime.Now.Subtract(LastAoE_Rotation).TotalSeconds >= 20 && rotationAoE_Skill != string.Empty,
                    new Sequence(
                        new Action(ret => AoEReset())
                    )
                ),
                //attempt to move through the rotation on schedule or pick up the pace
                new Decorator(
                    runWhen =>
                        rotationAoE_Skill == string.Empty ||
                        (DateTime.Now.Subtract(LastAoE_Rotation).TotalSeconds >= rotationAoE_Sleep && BuddyTor.Me.ResourceStat >= Global.EnergyMin && !(BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)"))) ||
                        (rotationAoE_Skill == "Series of Shots" && !AbilityManager.CanCast("Series of Shots", BuddyTor.Me.CurrentTarget)) ||
                        (rotationAoE_Skill == "Snipe" && AbilityManager.CanCast("Followthrough", BuddyTor.Me.CurrentTarget)),
                    new Sequence(
                        new Action(ret => AoENext())
                    )
                ),
                Sniper.Cast("Series of Shots",
                    ret =>
                            Global.IsInCover && rotationAoE_Skill == "Series of Shots"
                ),
                Sniper.Cast("Ambush",
                    ret =>
                            Global.IsInCover && rotationAoE_Skill == "Ambush"
                ),
                Sniper.Cast("Followthrough",
                    ret =>
                            Global.IsInCover && rotationAoE_Skill == "Followthrough"
                ),
                Sniper.Cast("Snipe",
                    ret =>
                            Global.IsInCover && rotationAoE_Skill == "Snipe"
                ),
                Sniper.Cast("Series of Shots",
                    ret =>
                            Global.IsInCover && rotationAoE_Skill == "Series of Shots"
                ),
                Sniper.Cast("Fragmentation Grenade",
                    onUnit => BuddyTor.Me.CurrentTarget,
                    castWhen =>
                        rotationAoE_Skill == "Fragmentation Grenade"
                ),
                Sniper.Cast("Explosive Probe",
                    castWhen =>
                        Global.IsInCover && rotationAoE_Skill == "Explosive Probe"
                ),
                Sniper.Cast("Rifle Shot",
                    castWhen =>
                        rotationAoE_Skill == "Rifle Shot"
                ),
                Sniper.Cast("Shatter Shot",
                    castWhen =>
                        rotationAoE_Skill == "Shatter Shot"
                ),
                Sniper.Cast("Corrosive Dart",
                    castWhen =>
                        rotationAoE_Skill == "Shatter Shot"
                ),
                Sniper.Cast("Suppressive Fire",
                    castWhen =>
                        rotationAoE_Skill == "Suppressive Fire"
                ),
                Movement.MoveTo(where => BuddyTor.Me.CurrentTarget.Position, Global.RangeDist)
            );
            #endregion
            #region Ops
            public static PrioritySelector Ops = new PrioritySelector(
                Movement.StopInRange(Global.RangeDist),
                Spell.WaitForCast(),
                //we need to crouch
                Sniper.Cast("Crouch", onUnit => BuddyTor.Me, castWhen => BuddyTor.Me.CurrentTarget.InLineOfSight && !Global.IsInCover && BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist),
                //should handle any weird need to reset rotation issues (or cause some)
                new Decorator(
                    runWhen =>
                        DateTime.Now.Subtract(LastOpsRotation).TotalSeconds >= 20 && rotationSkill != string.Empty,
                    new Sequence(
                        new Action(ret => OpsReset())
                    )
                ),
                //attempt to move through the rotation on schedule or pick up the pace
                new Decorator(
                    runWhen =>
                        rotationSkill == string.Empty ||
                        (DateTime.Now.Subtract(LastOpsRotation).TotalSeconds >= rotationSleep && BuddyTor.Me.ResourceStat >= Global.EnergyMin && !(BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)"))) ||
                        (rotationSkill == "Series of Shots" && !AbilityManager.CanCast("Series of Shots", BuddyTor.Me.CurrentTarget)) ||
                        (rotationSkill == "Snipe" && AbilityManager.CanCast("Followthrough", BuddyTor.Me.CurrentTarget)),
                    new Sequence(
                        new Action(ret => OpsNext())
                    )
                ),
                Sniper.Cast("Series of Shots",
                    ret =>
                            Global.IsInCover && rotationSkill == "Series of Shots"
                ),
                Sniper.Cast("Ambush",
                    ret =>
                            Global.IsInCover && rotationSkill == "Ambush"
                ),
                Sniper.Cast("Followthrough",
                    ret =>
                            Global.IsInCover && rotationSkill == "Followthrough"
                ),
                new Decorator(
                    useWhen => ((rotationSkill == "Snipe" && !BuddyTor.Me.HasBuff("Snap Shot")) && DateTime.Now.Subtract(LastBD).TotalSeconds >= 6),
                    new Sequence(
                        new Action(atn => LastBD = DateTime.Now),
                        new Action(atn => Buddy.Swtor.Movement.Move(MovementDirection.Forward, TimeSpan.FromMilliseconds(ran.Next(10, 24))))
                    )
                ),
                Sniper.Cast("Snipe",
                    ret =>
                            Global.IsInCover && rotationSkill == "Snipe"
                ),
                Sniper.Cast("Series of Shots",
                    ret =>
                            Global.IsInCover && rotationSkill == "Series of Shots"
                ),
                Sniper.Cast("Fragmentation Grenade",
                    onUnit => BuddyTor.Me.CurrentTarget,
                    castWhen =>
                        rotationSkill == "Fragmentation Grenade"
                ),
                Sniper.Cast("Explosive Probe",
                    castWhen =>
                        Global.IsInCover && rotationSkill == "Explosive Probe"
                ),
                Sniper.Cast("Rifle Shot",
                    castWhen =>
                        rotationSkill == "Rifle Shot"
                ),
                Sniper.Cast("Shatter Shot",
                    castWhen =>
                        rotationSkill == "Shatter Shot"
                ),
                Sniper.Cast("Corrosive Dart",
                    castWhen =>
                        rotationSkill == "Shatter Shot"
                ),
                Sniper.Cast("Suppressive Fire",
                    castWhen =>
                        rotationSkill == "Suppressive Fire"
                ),
                Movement.MoveTo(where => BuddyTor.Me.CurrentTarget.Position, Global.RangeDist)
            );
            #endregion

            //[Behavior(BehaviorType.OutOfCombat)]
            //[Class(CharacterClass.Agent, AdvancedClass.Sniper, SkillTreeId.SniperMarksmanship)]
            public static Composite OutOfDevCombat()
            {
                return new ProbabilitySelector(
                    //reset rotation handler
                    new Decorator(
                        runWhen =>
                            rotationSkill != string.Empty,
                        new Sequence(
                            new Action(ret => OpsReset())
                        )
                    )
                );
            }

            #region Ops junk in trunk
            private static string rotationSkill = string.Empty;
            private static int rotationSleep = 0;
            private static DateTime LastOpsRotation = DateTime.Now;
            private static Stack<string> OpsSkills = new Stack<string>();
            private static Stack<int> OpsIntervals = new Stack<int>();

            private static void OpsReset()
            {
                OpsSkills.Clear();
                OpsIntervals.Clear();
                rotationSkill = string.Empty;
            }

            private static void OpsNext()
            {
                rotationSkill = NextOpsSpell();
                rotationSleep = NextOpsInterval();
                LastOpsRotation = DateTime.Now;
            }

            /// <summary>
            /// Used to help enforce strict rotation.
            /// </summary>
            /// <returns>The next spell in the rotation.</returns>
            public static string NextOpsSpell()
            {
                if (OpsSkills.Count == 0)
                {
                    OpsSkills.Push("Followthrough");
                    OpsSkills.Push("Snipe");
                    OpsSkills.Push("Shatter Shot");
                    OpsSkills.Push("Fragmentation Grenade");
                    OpsSkills.Push("Followthrough");
                    OpsSkills.Push("Snipe");
                    OpsSkills.Push("Rifle Shot");
                    OpsSkills.Push("Fragmentation Grenade");
                    OpsSkills.Push("Followthrough");
                    OpsSkills.Push("Ambush");
                    OpsSkills.Push("Series of Shots");
                    OpsSkills.Push("Followthrough");
                    OpsSkills.Push("Snipe");
                    OpsSkills.Push("Shatter Shot");
                    OpsSkills.Push("Rifle Shot");
                    OpsSkills.Push("Followthrough");
                    OpsSkills.Push("Snipe");
                    OpsSkills.Push("Corrosive Dart");
                    OpsSkills.Push("Rifle Shot");
                    OpsSkills.Push("Followthrough");
                    OpsSkills.Push("Ambush");
                    OpsSkills.Push("Series of Shots");
                    OpsSkills.Push("Followthrough");
                    OpsSkills.Push("Snipe");
                    OpsSkills.Push("Shatter Shot");
                    OpsSkills.Push("Fragmentation Grenade");
                    OpsSkills.Push("Followthrough");
                    OpsSkills.Push("Snipe");
                    OpsSkills.Push("Explosive Probe");
                    OpsSkills.Push("Fragmentation Grenade");
                    OpsSkills.Push("Followthrough");
                    OpsSkills.Push("Ambush");
                    OpsSkills.Push("Series of Shots");
                }

                string skill = OpsSkills.Pop();

                return skill;
            }

            /// <summary>
            /// Used to help enforce strict rotation.
            /// </summary>
            /// <returns> Returns the current wait time between skills.</returns>
            public static int NextOpsInterval()
            {
                if (OpsIntervals.Count == 0)
                {
                    OpsIntervals.Push(2);//("Followthrough");
                    OpsIntervals.Push(2);//("Snipe");
                    OpsIntervals.Push(2);//("Shatter Shot");
                    OpsIntervals.Push(3);//("Fragmentation Grnade");
                    OpsIntervals.Push(2);//("Followthrough");
                    OpsIntervals.Push(2);//("Snipe");
                    OpsIntervals.Push(2);//("Corrosive Dart");
                    OpsIntervals.Push(3);//("Fragmentation Grnade");
                    OpsIntervals.Push(2);//("Followthrough");
                    OpsIntervals.Push(4);//("Ambush");
                    OpsIntervals.Push(7);//("Series of Shots");
                    OpsIntervals.Push(3);//("Followthrough");
                    OpsIntervals.Push(2);//("Snipe");
                    OpsIntervals.Push(2);//("Shatter Shot");
                    OpsIntervals.Push(3);//("Fragmentation Grnade");
                    OpsIntervals.Push(2);//("Followthrough");
                    OpsIntervals.Push(2);//("Snipe");
                    OpsIntervals.Push(2);//("Explosive Probe");
                    OpsIntervals.Push(3);//("Fragmentation Grnade");
                    OpsIntervals.Push(2);//("Followthrough");
                    OpsIntervals.Push(4);//("Ambush");
                    OpsIntervals.Push(7);//("Series of Shots");
                }

                int i = OpsIntervals.Pop();
                return i;
            }

            #endregion
            #region AoE junk in trunk
            private static string rotationAoE_Skill = string.Empty;
            private static int rotationAoE_Sleep = 0;
            private static DateTime LastAoE_Rotation = DateTime.Now;
            private static Stack<string> AoE_Skills = new Stack<string>();
            private static Stack<int> AoE_Intervals = new Stack<int>();

            private static void AoEReset()
            {
                AoE_Skills.Clear();
                AoE_Intervals.Clear();
                rotationAoE_Skill = string.Empty;
            }

            private static void AoENext()
            {
                rotationAoE_Skill = NextAoESpell();
                rotationAoE_Sleep = NextAoEInterval();
                LastAoE_Rotation = DateTime.Now;
            }


            /// <summary>
            /// Used to help enforce strict rotation.
            /// </summary>
            /// <returns>The next spell in the rotation.</returns>
            public static string NextAoESpell()
            {
                if (AoE_Skills.Count == 0)
                {

                    AoE_Skills.Push("Followthrough");
                    AoE_Skills.Push("Snipe");
                    AoE_Skills.Push("Shatter Shot");
                    AoE_Skills.Push("Fragmentation Grenade");
                    AoE_Skills.Push("Followthrough");
                    AoE_Skills.Push("Snipe");
                    AoE_Skills.Push("Explosive Probe");
                    AoE_Skills.Push("Fragmentation Grenade");
                    AoE_Skills.Push("Followthrough");
                    AoE_Skills.Push("Ambush");
                    AoE_Skills.Push("Series of Shots");
                    AoE_Skills.Push("Suppressive Fire");
                    AoE_Skills.Push("Fragmentation Grenade");
                }

                string skill = AoE_Skills.Pop();

                return skill;
            }

            /// <summary>
            /// Used to help enforce strict rotation.
            /// </summary>
            /// <returns> Returns the current wait time between skills.</returns>
            public static int NextAoEInterval()
            {
                if (AoE_Intervals.Count == 0)
                {
                    AoE_Intervals.Push(2);//("Followthrough");
                    AoE_Intervals.Push(2);//("Snipe");
                    AoE_Intervals.Push(2);//("Shatter Shot");
                    AoE_Intervals.Push(2);//("Fragmentation Grnade");
                    AoE_Intervals.Push(2);//("Followthrough");
                    AoE_Intervals.Push(2);//("Snipe");
                    AoE_Intervals.Push(2);//("Explosive Probe");
                    AoE_Intervals.Push(2);//("Fragmentation Grnade");
                    AoE_Intervals.Push(2);//("Followthrough");
                    AoE_Intervals.Push(4);//("Ambush");
                    AoE_Intervals.Push(5);//("Series of Shots");
                    AoE_Intervals.Push(6);//("Suppressive Fire");
                    AoE_Intervals.Push(2);//("Fragmentation Grnade");
                }

                int i = AoE_Intervals.Pop();

                return i;
            }

            #endregion

        }  

        #endregion
    }
}