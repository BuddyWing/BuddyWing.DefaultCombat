using System;
using System.Linq;

using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using DefaultCombat.Dynamics;
using DefaultCombat.Routines.Advanced.Sniper;
using snipe = DefaultCombat.Routines.Advanced.Sniper.Sniper;
using marks = DefaultCombat.Routines.Advanced.Sniper.Marksmanship;
using lethality = DefaultCombat.Routines.Advanced.Sniper.Lethality;
using eng = DefaultCombat.Routines.Advanced.Sniper.Engineering;

namespace DefaultCombat.Routines.Advanced.Gunslinger
{
    //this simply points to the Sniper which will apply the Sniper.Mirror logic to auto cast Gunslinger abilities
    //so remember don't worry about this file just tune the sniper files
    public class Gunslinger
    {
        public static Composite GunslingerPull()
        {
            return Sniper.Sniper.SniperCombat();
        }

        public static Composite GunslingerCombat()
        {
            return Sniper.Sniper.SniperCombat();
        }

        #region Sharpshooter
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Smuggler, AdvancedClass.Gunslinger, SkillTreeId.GunslingerSharpshooter)]
        public static Composite SharpshooterCombat()
        {
            return Marksmanship.MarksmanshipCombat();
        }

        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Smuggler, AdvancedClass.Gunslinger, SkillTreeId.GunslingerSharpshooter)]
        public static Composite SharpshooterPull()
        {
            return Marksmanship.MarksmanshipPull();
        }
        #endregion

        #region Saboteur
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Smuggler, AdvancedClass.Gunslinger, SkillTreeId.GunslingerSaboteur)]
        public static Composite SaboteurCombat()
        {
            return Engineering.EngineeringCombat();
        }

        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Smuggler, AdvancedClass.Gunslinger, SkillTreeId.GunslingerSaboteur)]
        public static Composite SaboteurPull()
        {
            return Engineering.EngineeringPull();
        }
        #endregion

        #region Dirty Fighting
        [Behavior(BehaviorType.Combat)]
        [Class(CharacterClass.Smuggler, AdvancedClass.Gunslinger, SkillTreeId.GunslingerDirtyFighting)]
        public static Composite DirtyFightingCombat()
        {
            return Lethality.LethalityCombat();
        }

        [Behavior(BehaviorType.Pull)]
        [Class(CharacterClass.Smuggler, AdvancedClass.Gunslinger, SkillTreeId.GunslingerDirtyFighting)]
        public static Composite DirtyFightingPull()
        {
            return Lethality.LethalityPull();
        }
        #endregion

    }
}
