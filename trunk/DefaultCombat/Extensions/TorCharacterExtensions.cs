using System.Collections.Generic;
using System.Linq;
using Buddy.Swtor.Objects;

namespace DefaultCombat.Extensions
{
    public static class TorCharacterExtensions
    {
        private static readonly IReadOnlyList<string> _dispellableDebuffs = new List<string>
        {
            								//-=-Universal-=-
            "Weakened (Physical)",
            								
            								
            								//-=-Operations-=-
            //Eternity Vault
            //--
            
            //Explosive Conflicts
            "Mental Anguish",
            "Weakened",
            
            //Gods from the Machine
            "Armor Melt (Any)",
            
            //Karagga's Palace
            "Unstable Energy",
            
            //Scum and Villainy
            
            //Temple of Scarifice
            
            //Terror from Beyond
            
            //The Dread Fortress
            "Corrupted Nanites (Any)",
            
            //The Dread Palace
            "Affliction",
            "Death Mark",

            //The Ravagers
            
            
            								//-=-Flashpoints-=-
            //Assault on Tython
            
            //Athiss
            
            //Battle of Rishi
            
            //Blood Hunt
            "Bleeding (Any)",
            //Boarding Party
            
            //Cademinu
            
            //Collicoid War Games
            
            //Crisis on Umbara
            
            //Czerka Core Meltdown
            
            //Depths of Manaan
            
            //Directive 7
            
            //Hammer Station
            
            //Kaon
            
            //Korriban Incursion
            
            //Kuat Drive Yards
            
            //Legacy of Rakata
            "Hunting Trap (Any)"

            //Lost Island
            
            //Maelstrom Prison
            
            //Mandalorian Raiders
            
            //Red Reaper
            
            //Taral V
            
            //The Battle of Illum
            
            //The Black Talon
            
            //The Esseles
            
            //The False Emperor
            
            //A Traitor Among the Chiss
            
            //The Foundry
            
            
            								//-=-Uprisings-=-
            
            
            //Crimson Fang
            
            //Divided We Fall
            
            //Trench Runner
            
            //Trial and Error
            
            //Landing Party
            
            //Done and Dusted
            
            //Firefrost
            
            //Fractured
            
            //Destroyer of Worlds
            
            //Inferno
            
            
            								//-=-PVP-=-
            //"Trauma (Physical)",
            
            
            								//-=-World and Event Bosses-=-
            								
            
            		//Events
            
            //Xenoanalyst II
            
            //Colossal Monolith
            
            //Golden Fury
            
            //The Eyeless
            
            		//World Bosses
        };

        public static bool ShouldDispel(this TorCharacter target)
        {
            return target != null && _dispellableDebuffs.Any(target.HasDebuff);
        }
    }
}
