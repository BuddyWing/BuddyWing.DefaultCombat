using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;

using DefaultCombat.Dynamics;
using Action = Buddy.BehaviorTree.Action;


namespace DefaultCombat
{
    static class SpecHandler
    {
        public static SkillTreeId GetSpec()
        {
            SkillTreeId tree1 = switchSpec(BuddyTor.Me).Item1;
            SkillTreeId tree2 = switchSpec(BuddyTor.Me).Item2;
            SkillTreeId tree3 = switchSpec(BuddyTor.Me).Item3;

            return calculateSpec(tree1, tree2, tree3);
        }

        public static SkillTreeId GetSpec(TorPlayer Player)
        {
            SkillTreeId tree1 = switchSpec(Player).Item1;
            SkillTreeId tree2 = switchSpec(Player).Item2;
            SkillTreeId tree3 = switchSpec(Player).Item3;

            return calculateSpec(tree1, tree2, tree3);
        }

        private static Tuple<SkillTreeId,SkillTreeId,SkillTreeId> switchSpec(TorPlayer player)
        {
            SkillTreeId tree1 = new SkillTreeId();
            SkillTreeId tree2 = new SkillTreeId();
            SkillTreeId tree3 = new SkillTreeId();

            switch (player.AdvancedClass)
            {
                case AdvancedClass.Assassin:
                    tree1 = SkillTreeId.AssassinDarkness;
                    tree2 = SkillTreeId.AssassinDeception;
                    tree3 = SkillTreeId.AssassinMadness;
                    break;

                case AdvancedClass.Sorcerer:
                    tree1 = SkillTreeId.SorcererCorruption;
                    tree2 = SkillTreeId.SorcererLightning;
                    tree3 = SkillTreeId.SorcererMadness;
                    break;

                case AdvancedClass.Juggernaut:
                    tree1 = SkillTreeId.JuggernautImmortal;
                    tree2 = SkillTreeId.JuggernautRage;
                    tree3 = SkillTreeId.JuggernautVengeance;
                    break;

                case AdvancedClass.Marauder:
                    tree1 = SkillTreeId.MarauderAnnihilation;
                    tree2 = SkillTreeId.MarauderCarnage;
                    tree3 = SkillTreeId.MarauderRage;
                    break;

                case AdvancedClass.Sniper:
                    tree1 = SkillTreeId.SniperEngineering;
                    tree2 = SkillTreeId.SniperLethality;
                    tree3 = SkillTreeId.SniperMarksmanship;
                    break;

                case AdvancedClass.Operative:
                    tree1 = SkillTreeId.OperativeConcealment;
                    tree2 = SkillTreeId.OperativeLethality;
                    tree3 = SkillTreeId.OperativeMedic;
                    break;

                case AdvancedClass.Mercenary:
                    tree1 = SkillTreeId.MercenaryArsenal;
                    tree2 = SkillTreeId.MercenaryBodyguard;
                    tree3 = SkillTreeId.MercenaryFirebug;
                    break;

                case AdvancedClass.Powertech:
                    tree1 = SkillTreeId.PowertechAdvanced;
                    tree2 = SkillTreeId.PowertechFirebug;
                    tree3 = SkillTreeId.PowertechShieldTech;
                    break;

                case AdvancedClass.Shadow:
                    tree1 = SkillTreeId.ShadowBalance;
                    tree2 = SkillTreeId.ShadowCombat;
                    tree3 = SkillTreeId.ShadowInfiltration;
                    break;

                case AdvancedClass.Sage:
                    tree1 = SkillTreeId.SageBalance;
                    tree2 = SkillTreeId.SageSeer;
                    tree3 = SkillTreeId.SageTelekinetics;
                    break;

                case AdvancedClass.Guardian:
                    tree1 = SkillTreeId.GuardianDefense;
                    tree2 = SkillTreeId.GuardianFocus;
                    tree3 = SkillTreeId.GuardianVigilance;
                    break;

                case AdvancedClass.Sentinel:
                    tree1 = SkillTreeId.SentinelCombat;
                    tree2 = SkillTreeId.SentinelFocus;
                    tree3 = SkillTreeId.SentinelWatchman;
                    break;

                case AdvancedClass.Gunslinger:
                    tree1 = SkillTreeId.GunslingerDirtyFighting;
                    tree2 = SkillTreeId.GunslingerSaboteur;
                    tree3 = SkillTreeId.GunslingerSharpshooter;
                    break;

                case AdvancedClass.Scoundrel:
                    tree1 = SkillTreeId.ScoundrelDirtyFighting;
                    tree2 = SkillTreeId.ScoundrelSawbones;
                    tree3 = SkillTreeId.ScoundrelScrapper;
                    break;

                case AdvancedClass.Commando:
                    tree1 = SkillTreeId.CommandoAssaultSpecialist;
                    tree2 = SkillTreeId.CommandoCombatMedic;
                    tree3 = SkillTreeId.CommandoGunnery;
                    break;

                case AdvancedClass.Vanguard:
                    tree1 = SkillTreeId.VanguardAssaultSpecialist;
                    tree2 = SkillTreeId.VanguardShieldSpecialist;
                    tree3 = SkillTreeId.VanguardTactics;
                    break;

                default:
                    break;

                    
            }
            return new Tuple<SkillTreeId, SkillTreeId, SkillTreeId>(tree1, tree2, tree3);
        }

        private static SkillTreeId calculateSpec(SkillTreeId tree1, SkillTreeId tree2, SkillTreeId tree3)
        {
            var points1 = BuddyTor.Me.GetSkillPointsSpentInTree(tree1);
            var points2 = BuddyTor.Me.GetSkillPointsSpentInTree(tree2);
            var points3 = BuddyTor.Me.GetSkillPointsSpentInTree(tree3);

            if (points1 > points2 && points1 > points3)
            {
                return tree1;
            }
            else if (points2 > points1 && points2 > points3)
            {
                return tree2;
            }
            else if (points3 > points1 && points3 > points2)
            {
                return tree3;
            }
            else
                return SkillTreeId.None;
        }
    }
         
}
