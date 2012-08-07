using System;
using System.Linq;

using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;

using DefaultCombat.Dynamics;
namespace DefaultCombat.Routines.Advanced.Sniper
{
    public class Sniper
    {
        public static Composite SniperPull()
        {
            return SniperCombat();
        }

        #region Skill/Spell Mirror
        //adds an extra layer of maintenance, but provides a safty check
        public static bool IsAgentSkill(string spell)
        {
            switch (spell)
            {
                //CC abilities
                case "Debilitate":
                case "Diversion":
                case "Flash Bang":
                case "Leg Shot":
                case "Cover Pulse":

                //DPS Abilities
                case "Followthrough":
                case "Series of Shots":
                case "Ambush":
                case "Snipe":
                case "Shatter Shot":
                case "Takedown":
                case "Quick Shot":
                case "Rifle Shot":
                case "Headshot":
                case "Shiv":
                case "Corrosive Dart":
                case "Eviscerate":

                //AoE
                case "Suppressive Fire":
                case "Orbital Strike":
                case "Fragmentation Grenade":

                //buffs
                case "Target Acquired":
                case "Ballistic Shield":
                case "Entrench":
                case "Crack Shot":
                case "Hold Position":
                case "Laze Target":
                case "Rapid Fire":
                case "Explosive Probe":
                case "Adrenaline Probe":
                case "Distraction":
                case "Evasion":
                case "Countermeasures":
                case "Shield Probe":
                    return true;

                //no match
                default: return false;
            }
        }

        //http://www.torhead.com/class/67dQpO7/sniper
        //http://www.torhead.com/class/1tTiRzJ/gunslinger
        //this creates a layer of complexity where things can break, 
        //but it also means I don't need to keep two copis of the code 
        //with diff skill names...
        public static string Mirror(string spell)
        {
            //until there is an ability that cause an issue 
            //early out
            if (AbilityManager.HasAbility(spell)) return spell;

            //if this is not an agent skill and i am an agent try to convert to an agent skill
            if (!IsAgentSkill(spell) && (BuddyTor.Me.Class == CharacterClass.Agent || BuddyTor.Me.Class == CharacterClass.Operative || BuddyTor.Me.Class == CharacterClass.Sniper))
            {
                //expects a gunslinger skill name and returns a sniper skill name
                switch (spell)
                {
                    //CC abilities
                    case "Dirty Kick": return "Debilitate";                                 //
                    case "Diversion": return "Diversion";                                   //
                    case "Flash Grenade": return "Flash Bang";                              //
                    case "Leg Shot": return "Leg Shot";                                     //
                    case "Pulse Detonator": return "Cover Pulse";                           //

                    //DPS Abilities
                    case "Trickshot": return "Followthrough";                               //
                    case "Speed Shot": return "Series of Shots";                            //
                    case "Aimed Shot": return "Ambush";                                     //
                    case "Charged Burst": return "Snipe";                                   //
                    case "Flourish Shot": return "Shatter Shot";                            //
                    case "Quickdraw": return "Takedown";                                    //
                    case "Quick Shot": return "Quick Shot";                                 //
                    case "Flurry of Bolts": return "Rifle Shot";                            //
                    case "Headshot": return "Headshot";                                     //
                    case "Blaster Whip": return "Shiv";                                     //
                    case "Vital Shot": return "Corrosive Dart";                             //
                    case "Cheap Shot": return "Eviscerate";                                 //

                    //AoE
                    case "Sweeping Gunfire": return "Suppressive Fire";                     //
                    case "XS Frienter Flyby": return "Orbital Strike";                      //
                    case "Thermal Grenade": return "Fragmentation Grenade";                 //

                    //buffs
                    case "Illegal Mods": return "Target Acquired";                          //
                    case "Scrambling Field": return "Ballistic Shield";                     //
                    case "Hunker Down": return "Entrench";                                  //
                    case "Advanced Targeting": return "Crack Shot";                         //
                    case "Hold Position": return "Hold Position";                           //
                    case "Smuggler's Luck": return "Laze Target";                           //
                    case "Rapid Fire": return "Rapid Fire";                                 //
                    case "Sabotage Charge": return "Explosive Probe";                       //
                    case "Cool Head": return "Adrenaline Probe";                            //
                    case "Distraction": return "Distraction";                               //
                    case "Dodge": return "Evasion";                                         //
                    case "Surrender": return "Countermeasures";                             //
                    case "Defense Screen": return "Shield Probe";                           //

                    //no match
                    default: return spell;
                }
            }

            //if this is an agent skill and i am a smuggler try to convert
            if (IsAgentSkill(spell) && (BuddyTor.Me.Class == CharacterClass.Smuggler || BuddyTor.Me.Class == CharacterClass.Scoundrel || BuddyTor.Me.Class == CharacterClass.Gunslinger))
            {
                //expects an agent skill and returns a smuggler skill
                switch (spell)
                {
                    //CC abilities
                    case "Debilitate": return "Dirty Kick";                                 //
                    case "Diversion": return "Diversion";                                   //
                    case "Flash Bang": return "Flash Grenade";                              //
                    case "Leg Shot": return "Leg Shot";                                     //
                    case "Cover Pulse": return "Pulse Detonator";                           //

                    //DPS Abilities
                    case "Followthrough": return "Trickshot";                               //
                    case "Series of Shots": return "Speed Shot";                            //
                    case "Ambush": return "Aimed Shot";                                     //
                    case "Snipe": return "Charged Burst";                                   //
                    case "Shatter Shot": return "Flourish Shot";                            //
                    case "Takedown": return "Quickdraw";                                    //
                    case "Quick Shot": return "Quick Shot";                                 //
                    case "Rifle Shot": return "Flurry of Bolts";                            //
                    case "Headshot": return "Headshot";                                     //
                    case "Shiv": return "Blaster Whip";                                     //
                    case "Corrosive Dart": return "Vital Shot";                             //
                    case "Eviscerate": return "Cheap Shot";                                 //

                    //AoE
                    case "Suppressive Fire": return "Sweeping Gunfire";                     //
                    case "Orbital Strike": return "XS Frienter Flyby";                      //
                    case "Fragmentation Grenade": return "Thermal Grenade";                 //

                    //buffs
                    case "Target Acquired": return "Illegal Mods";                          //
                    case "Ballistic Shield": return "Scrambling Field";                     //
                    case "Entrench": return "Hunker Down";                                  //
                    case "Crack Shot": return "Advanced Targeting";                         //
                    case "Hold Position": return "Hold Position";                           //
                    case "Laze Target": return "Smuggler's Luck";                           //
                    case "Rapid Fire": return "Rapid Fire";                                 //
                    case "Explosive Probe": return "Sabotage Charge";                       //
                    case "Adrenaline Probe": return "Cool Head";                            //
                    case "Distraction": return "Distraction";                               //
                    case "Evasion": return "Dodge";                                         //
                    case "Countermeasures": return "Surrender";                             //
                    case "Shield Probe": return "Defense Screen";                           //

                    //no match
                    default: return spell;
                }
            }

            //pass through for unknown and already valid skills
            return spell;
        }
        #endregion

        #region LowLvl ST
        public static PrioritySelector LowLVL_ST = new PrioritySelector(
            Movement.StopInRange(Global.RangeDist),
            Spell.WaitForCast(),
            //http://www.torhead.com/ability/fDYTCm2/escape
            Sniper.Cast("Escape ", castWhen => BuddyTor.Me.IsStunned || AbilityManager.CanCast("Escape", BuddyTor.Me)),
            //melee range cc
            //http://www.torhead.com/ability/avfFa1u/debilitate
            Sniper.Cast("Dibilitate",
                castWhen =>
                    (BuddyTor.Me.CurrentTarget.IsCasting || BuddyTor.Me.HealthPercent <= Global.MidHealth) &&
                    BuddyTor.Me.CurrentTarget.Distance <= Global.MeleeDist),
            //we need to be in cover
            //http://www.torhead.com/ability/cciVyHR/crouch
            Sniper.Cast("Crouch",
                onUnit =>
                    BuddyTor.Me, castWhen => BuddyTor.Me.CurrentTarget.InLineOfSight &&
                    !Global.IsInCover &&
                    BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist),
            // AoE Grenade, High Damage //this may need to be moved down again
            //http://www.torhead.com/ability/f5R2qsF/flash-bang
            Sniper.Cast("Flash Bang",
                castWhen =>
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
            //snipe only when we are not in another FT rotation segment or ready to begin one
            //http://www.torhead.com/ability/SO2A5A/ambush
            //http://www.torhead.com/ability/arpM0Mr/reactive-shot
            Sniper.Cast("Ambush",
                castWhen =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            //level 18
            //finisher, below 30% health
            //http://www.torhead.com/ability/581cEC5/takedown
            Sniper.Cast("Takedown",
                castWhen =>
                    (BuddyTor.Me.Level >= 18 && AbilityManager.HasAbility("Takedown")) &&
                    AbilityManager.CanCast("Takedown", BuddyTor.Me.CurrentTarget) &&
                    !BuddyTor.Me.CurrentTarget.IsStunned),
            // Medium DPS Grenade, place this every time it is available
            //http://www.torhead.com/ability/OEXFI/explosive-probe
            Sniper.Cast("Explosive Probe",
                castWhen => Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin + 20 &&
                    !BuddyTor.Me.CurrentTarget.IsStunned
            ),
            //level 28 armor/healing debuff
            //http://www.torhead.com/ability/10OsXEA/shatter-shot
            Sniper.Cast("Shatter Shot",
                castWhen =>
                    (BuddyTor.Me.Level >= 28 && AbilityManager.HasAbility("Shatter Shot")) &&
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin + 20 &&
                    !BuddyTor.Me.CurrentTarget.IsStunned
            ),
            // attempt to debuff any target that can be debuffed and are a "real" threat
            Sniper.Cast("Diversion",
                castWhen => (
                    AbilityManager.HasAbility("Diversion") &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Weak &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Standard &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.RaidBoss) &&
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            // AoE Grenade, High Damage //this may need to be moved down again
            //http://www.torhead.com/ability/2CUyw6D/fragmentation-grenade
            Sniper.Cast("Fragmentation Grenade",
                castWhen =>
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= Global.MeleeDist &&
                            !o.IsDead &&
                            o.IsStunned &&
                            o.IsHostile &&
                            o.HasDebuff("Blinded(Tech)")
                    ) <= 1 &&
                    ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= Global.MeleeDist &&
                            !o.IsDead &&
                            !o.IsStunned &&
                            o.IsHostile &&
                            !o.HasDebuff("Blinded(Tech)")
                    ) >= 1 && !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded(Tech)")
            ),
            //buff snipe to ensure a crit
            Sniper.Cast("Laze Target",
                ret =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    (AbilityManager.CanCast("Snipe", BuddyTor.Me.CurrentTarget) && !AbilityManager.CanCast("Ambush", BuddyTor.Me.CurrentTarget))
            ),
            //http://www.torhead.com/ability/aZM9JIO/snipe
            Sniper.Cast("Snipe",
                ret =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin
            ),
            //level 16
            //knock back targets
            //http://www.torhead.com/ability/buhxjod/cover-pulse
            Sniper.Cast("Cover Pulse",
                castWhen =>
                    (BuddyTor.Me.Level >= 16 && AbilityManager.HasAbility("Cover Pulse")) &&
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
            //well, everything can't be cced all the time
            //http://www.torhead.com/ability/d0ft5am/headshot
            Sniper.Cast("Headshot",
                castWhen =>
                    AbilityManager.HasAbility("Headshot") &&
                    (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard) &&
                    (BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)"))
            ),
            //http://www.torhead.com/ability/5V44Vl0/rifle-shot
            Sniper.Cast("Rifle Shot",
                castWhen =>
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.RangeDist)
        );

        #endregion

        #region LowLvlAoE
        public static PrioritySelector LowLVL_AoE = new PrioritySelector(
            Movement.StopInRange(Global.RangeDist),
            Spell.WaitForCast(),
            //http://www.torhead.com/ability/fDYTCm2/escape
            Sniper.Cast("Escape ", castWhen => BuddyTor.Me.IsStunned || AbilityManager.CanCast("Escape", BuddyTor.Me)), 
            //melee range cc
            //http://www.torhead.com/ability/avfFa1u/debilitate
            Sniper.Cast("Dibilitate",
                castWhen =>
                    (BuddyTor.Me.CurrentTarget.IsCasting || BuddyTor.Me.HealthPercent <= Global.MidHealth) &&
                    BuddyTor.Me.CurrentTarget.Distance <= Global.MeleeDist),
            //we need to be in cover
            //http://www.torhead.com/ability/cciVyHR/crouch
            Sniper.Cast("Crouch",
                onUnit =>
                    BuddyTor.Me, castWhen => BuddyTor.Me.CurrentTarget.InLineOfSight &&
                    !Global.IsInCover &&
                    BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist),
            // AoE Grenade, High Damage //this may need to be moved down again
            //http://www.torhead.com/ability/2CUyw6D/fragmentation-grenade
            Sniper.Cast("Fragmentation Grenade",
                castWhen =>
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= Global.MeleeDist &&
                            !o.IsDead &&
                            o.IsStunned &&
                            o.IsHostile &&
                            o.HasDebuff("Blinded(Tech)")
                    ) <= 2 &&
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
            //http://www.torhead.com/ability/VQsVbW/suppressive-fire
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
            //level 16
            //knock back targets
            //http://www.torhead.com/ability/buhxjod/cover-pulse
            Sniper.Cast("Cover Pulse",
                castWhen =>
                    (BuddyTor.Me.Level >= 16 && AbilityManager.HasAbility("Cover Pulse")) &&
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                        o =>
                            Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.Position, o.Position) <= 0.5f &&
                            !o.IsDead &&
                            !o.IsStunned &&
                            !o.HasDebuff("Blinded (Tech)") &&
                            o.IsHostile
                    ) >= 2 && !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")
            ),
            //snipe only when we are not in another FT rotation segment or ready to begin one
            //http://www.torhead.com/ability/SO2A5A/ambush
            //http://www.torhead.com/ability/arpM0Mr/reactive-shot
            Sniper.Cast("Ambush",
                castWhen =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            //level 18
            //finisher, below 30% health
            //http://www.torhead.com/ability/581cEC5/takedown
            Sniper.Cast("Takedown",
                castWhen =>
                    (BuddyTor.Me.Level >= 18 && AbilityManager.HasAbility("Takedown")) &&
                    AbilityManager.CanCast("Takedown", BuddyTor.Me.CurrentTarget) &&
                    !BuddyTor.Me.CurrentTarget.IsStunned),
            // Medium DPS Grenade, place this every time it is available
            //http://www.torhead.com/ability/OEXFI/explosive-probe
            Sniper.Cast("Explosive Probe",
                castWhen => Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin + 20 &&
                    !BuddyTor.Me.CurrentTarget.IsStunned
            ),
            //level 28 armor/healing debuff
            //http://www.torhead.com/ability/10OsXEA/shatter-shot
            Sniper.Cast("Shatter Shot",
                castWhen =>
                    (BuddyTor.Me.Level >= 28 && AbilityManager.HasAbility("Shatter Shot")) &&
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin + 20 &&
                    !BuddyTor.Me.CurrentTarget.IsStunned
            ),
            // attempt to debuff any target that can be debuffed and are a "real" threat
            Sniper.Cast("Diversion",
                castWhen => (
                    AbilityManager.HasAbility("Diversion") &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Weak &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Standard &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.RaidBoss) &&
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            //buff snipe to ensure a crit
            Sniper.Cast("Laze Target",
                ret =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    (AbilityManager.CanCast("Snipe", BuddyTor.Me.CurrentTarget) && !AbilityManager.CanCast("Ambush", BuddyTor.Me.CurrentTarget))
            ),
            //http://www.torhead.com/ability/aZM9JIO/snipe
            Sniper.Cast("Snipe",
                ret =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin
            ),
            //level 16
            //knock back targets
            //http://www.torhead.com/ability/buhxjod/cover-pulse
            Sniper.Cast("Cover Pulse",
                castWhen =>
                    (BuddyTor.Me.Level >= 16 && AbilityManager.HasAbility("Cover Pulse")) &&
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
            //well, everything can't be cced all the time
            //http://www.torhead.com/ability/d0ft5am/headshot
            Sniper.Cast("Headshot",
                castWhen =>
                    AbilityManager.HasAbility("Headshot") &&
                    (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard) &&
                    (BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)"))
            ),
            //http://www.torhead.com/ability/5V44Vl0/rifle-shot
            Sniper.Cast("Rifle Shot",
                castWhen =>
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.RangeDist)
        );

        #endregion

        #region Level10to36
        public static PrioritySelector Level10to36 = new PrioritySelector(
            Spell.WaitForCast(),
            Sniper.Cast("Escape ", castWhen => BuddyTor.Me.IsStunned || AbilityManager.CanCast("Escape", BuddyTor.Me)),
            Movement.StopInRange(Global.RangeDist),
            //snipe only when we are not in another FT rotation segment or ready to begin one
            Sniper.Cast("Ambush",
                castWhen =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !(AbilityManager.CanCast("Series of Shots", BuddyTor.Me.CurrentTarget) || AbilityManager.CanCast("Followthrough", BuddyTor.Me.CurrentTarget) || AbilityManager.CanCast("Takedown", BuddyTor.Me.CurrentTarget)) &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            Sniper.Cast("Snipe",
                ret =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    !(AbilityManager.CanCast("Followthrough", BuddyTor.Me.CurrentTarget) || AbilityManager.CanCast("Series of Shots", BuddyTor.Me.CurrentTarget) || AbilityManager.CanCast("Ambush", BuddyTor.Me.CurrentTarget))
                ),
            //filler
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
            //we need to be in cover
            Sniper.Cast("Crouch",
                onUnit =>
                    BuddyTor.Me, castWhen => BuddyTor.Me.CurrentTarget.InLineOfSight &&
                    !Global.IsInCover &&
                    BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist),
            //level 18
            //finisher, below 30% health
            Sniper.Cast("Takedown",
                castWhen =>
                    (BuddyTor.Me.Level >= 18 && AbilityManager.HasAbility("Takedown")) &&
                    AbilityManager.CanCast("Takedown", BuddyTor.Me.CurrentTarget) &&
                    !BuddyTor.Me.CurrentTarget.IsStunned),
            // Medium DPS Grenade, place this every time it is available
            Sniper.Cast("Explosive Probe",
                castWhen => Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin + 20 &&
                    !BuddyTor.Me.CurrentTarget.IsStunned
            ),
            //level 28 armor/healing debuff
            Sniper.Cast("Shatter Shot",
                castWhen =>
                    (BuddyTor.Me.Level >= 28 && AbilityManager.HasAbility("Shatter Shot")) &&
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin + 20 &&
                    !BuddyTor.Me.CurrentTarget.IsStunned
            ),
            // attempt to debuff any target that can be debuffed and are a "real" threat
            Sniper.Cast("Diversion",
                castWhen => (
                    AbilityManager.HasAbility("Diversion") &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Weak &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.Standard &&
                    BuddyTor.Me.CurrentTarget.Toughness != CombatToughness.RaidBoss) &&
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            //buff snipe to ensure a crit
            Sniper.Cast("Laze Target",
                ret =>
                    Global.IsInCover &&
                    BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                    (AbilityManager.CanCast("Snipe", BuddyTor.Me.CurrentTarget) && !AbilityManager.CanCast("Ambush", BuddyTor.Me.CurrentTarget))
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
            Sniper.Cast("Dibilitate",
                castWhen =>
                    (BuddyTor.Me.CurrentTarget.IsCasting || BuddyTor.Me.HealthPercent <= Global.MidHealth) &&
                    BuddyTor.Me.CurrentTarget.Distance <= Global.MeleeDist),
            //level 22
            //AoE spray bullets
            Sniper.CastOnGround("Suppressive Fire",
                castWhen =>
                    (BuddyTor.Me.Level >= 22 && AbilityManager.HasAbility("Suppressive Fire")) &&
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
            //level 16
            //knock back targets
            Sniper.Cast("Cover Pulse",
                castWhen =>
                    (BuddyTor.Me.Level >= 16 && AbilityManager.HasAbility("Cover Pulse")) &&
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
            //well, everything can't be cced all the time
            Sniper.Cast("Headshot",
                castWhen =>
                    AbilityManager.HasAbility("Headshot") &&
                    (BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Weak || BuddyTor.Me.CurrentTarget.Toughness == CombatToughness.Standard) &&
                    (BuddyTor.Me.CurrentTarget.IsStunned || BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)"))
            ),
            Sniper.Cast("Rifle Shot",
                castWhen =>
                    !BuddyTor.Me.CurrentTarget.IsStunned &&
                    !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")),
            Movement.MoveTo(ret => BuddyTor.Me.CurrentTarget.Position, Global.RangeDist)
        );

        #endregion

        #region Sniper.Cast => these functions where implemented as a quick dirty way to get the code to mirror
        public static Composite Cast(string spell)
        {
            return Spell.Cast(Sniper.Mirror(spell));
        }

        public static Composite Cast(string spell, BooleanValueDelegate requirements)
        {
            return Spell.Cast(Sniper.Mirror(spell), requirements);
        }

        public static Composite Cast(string spell, CharacterSelectionDelegate onCharacter, BooleanValueDelegate requirements)
        {
            return Spell.Cast(Sniper.Mirror(spell), onCharacter, requirements);
        }

        public static Composite CastOnGround(string spell, BooleanValueDelegate requirements, CommonBehaviors.Retrieval<Buddy.Common.Math.Vector3> location)
        {
            return Spell.CastOnGround(Sniper.Mirror(spell), requirements, location);
        }
        #endregion

        #region Sniper Check
        public static Composite SniperSelfCheck = new PrioritySelector (
            //knock back targets
            //http://www.torhead.com/ability/buhxjod/cover-pulse
            Sniper.Cast("Cover Pulse",
                castWhen =>
                    (BuddyTor.Me.Level >= 16 && AbilityManager.HasAbility("Cover Pulse")) &&
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
            //enery quick fix
            //http://www.torhead.com/ability/bI3EsHM/adrenaline-probe
            Sniper.Cast("Adrenaline Probe", 
                castWhen =>
                    (BuddyTor.Me.Level >= 14 && AbilityManager.HasAbility("Adrenaline Probe")) &&
                    BuddyTor.Me.ResourceStat <= (Global.EnergyMin - 15)
            ),
            //defense
            //http://www.torhead.com/ability/8zIyFyh/shield-probe
            Sniper.Cast("Shield Probe", 
                castWhen =>
                    (BuddyTor.Me.Level >= 32 && AbilityManager.HasAbility("Shield Probe")) &&
                    BuddyTor.Me.HealthPercent <= Global.LowHealth
            )
        );
        #endregion

        public static Composite SniperCombat()
        {
            return new PrioritySelector(
                Movement.StopInRange(Global.RangeDist),
                Sniper.Cast("Escape ", castWhen => BuddyTor.Me.IsStunned),

                //interrupt
                Sniper.Cast("Distraction",
                    castWhen =>
                        BuddyTor.Me.CurrentTarget.IsCasting &&
                        BuddyTor.Me.CurrentTarget.Distance >= Global.MeleeDist),

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
                        (BuddyTor.Me.CurrentTarget.IsCasting || BuddyTor.Me.HealthPercent <= Global.HealthCritical) &&
                        BuddyTor.Me.CurrentTarget.Distance <= Global.MeleeDist),


                //finisher, below 30% health
                Sniper.Cast("Takedown", 
                    castWhen => 
                        AbilityManager.CanCast("Takedown", BuddyTor.Me.CurrentTarget) && 
                        !BuddyTor.Me.CurrentTarget.IsStunned),

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
                                !o.HasDebuff("Blinded (Tech)")
                        ) >= 2 && !BuddyTor.Me.CurrentTarget.IsStunned &&
                        !BuddyTor.Me.CurrentTarget.HasDebuff("Blinded (Tech)")
                ),

                //we need to be in cover
                Sniper.Cast("Crouch",
                    castWhen => 
                        BuddyTor.Me, ret => BuddyTor.Me.CurrentTarget.InLineOfSight && 
                        !Global.IsInCover && 
                        BuddyTor.Me.CurrentTarget.Distance <= Global.RangeDist),

                // Medium DPS Grenade, place this every time it is available
                Sniper.Cast("Explosive Probe", 
                    castWhen => Global.IsInCover && 
                        BuddyTor.Me.ResourceStat >= Global.EnergyMin + 20 && 
                        !BuddyTor.Me.CurrentTarget.IsStunned
                ),

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
                //Sniper.Cast("Entrench", castWhen => Global.IsInCover), //need some advance logic here for when to use it

                //level 36
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


                //AoE spray bullets
                Sniper.CastOnGround("Suppressive Fire",
                    castWhen =>
                        BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                        ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                            o =>
                                Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.CurrentTarget.Position, o.Position) <= Global.MeleeDist &&
                                !o.IsDead &&
                                !o.IsStunned &&
                                o.IsHostile
                        ) >= 2 && !BuddyTor.Me.CurrentTarget.IsStunned,
                    ret =>
                        BuddyTor.Me.CurrentTarget.Position
                ),

                //knock back targets
                Sniper.Cast("Cover Pulse",
                    castWhen =>
                        BuddyTor.Me.ResourceStat >= Global.EnergyMin &&
                        ObjectManager.GetObjects<Buddy.Swtor.Objects.TorNpc>().Count(
                            o =>
                                Buddy.Common.Math.Vector3.Distance(BuddyTor.Me.Position, o.Position) <= 0.5f &&
                                !o.IsDead &&
                                !o.IsStunned &&
                                o.IsHostile
                        ) >= 1 && !BuddyTor.Me.CurrentTarget.IsStunned
                )

            ); 

        }
    }
}
