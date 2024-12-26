using System;
using UnityEngine.SceneManagement;

namespace Source.SaveData
{
    /// <summary>
    /// Represents a chapter or a custcene or something.
    /// </summary>
    public enum GameScene
    {
        /// <summary>
        /// The player has never started the game or hasn't gotten past the opening.
        /// </summary>
        Beginning = 0,
        
        /// <summary>
        /// We meet Cyber, Sludge, and Tiny
        /// </summary>
        Chapter1 = 1,
        
        /// <summary>
        /// One month ago, Cyber gets captured 
        /// </summary>
        ChapterA = 2,
        
        /// <summary>
        /// Cave level
        /// </summary>
        Chapter2 = 3,
        
        /// <summary>
        /// First Drone level as Raven
        /// </summary>
        ChapterB = 4,
        
        /// <summary>
        /// Abandoned outpost level
        /// </summary>
        Chapter3 = 5,
        
        /// <summary>
        /// Raven meets the Vaslings 
        /// </summary>
        ChapterC = 6,
        
        /// <summary>
        /// Garbage disposal 
        /// </summary>
        Chapter4 = 7,
        
        /// <summary>
        /// Raven attacks the vaslings and cyber gets recruited
        /// </summary>
        ChapterD = 8,
        
        /// <summary>
        /// Cyber gets to the nest and is killed!
        /// </summary>
        Chapter5 = 9,
        
        /// <summary>
        /// Cutscene only - Davis is given the plan from Davis.
        /// </summary>
        ChapterE = 10,
        
        /// <summary>
        /// Cyber discovers the inside of the nest
        /// </summary>
        Chapter6 = 11,
        
        /// <summary>
        /// Raven fights the nest with Cyber's ship
        /// </summary>
        Chapter7 = 12,
        
        /// <summary>
        /// Final boss battle, Cyber fights the earth 
        /// </summary>
        Chapter8 = 13,
    }

    public static class GameSceneExtensions
    {
        public static string GetUnitySceneName(this GameScene scene)
        {
            switch (scene)
            {
                case GameScene.Beginning:
                    return "Scenes/30_Opening";
                default:
                    throw new InvalidOperationException("scene doesn't exist, saved data is likely corrupt");
            }
        }
    }



}