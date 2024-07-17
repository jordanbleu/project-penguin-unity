using UnityEngine;

namespace Source
{
    public class Startup
    {
        /// <summary>
        /// Runs on application startup 
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Main()
        {
            // Seed the random generator
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            
            
        }
    }
}