using Payload.MonoScript;
using System.Collections.Generic;
using UnityEngine;


namespace Payload
{
    public class Main
    {
        static GameObject gameObject;

        public static void Inject()
        {
            gameObject = new GameObject();
            gameObject.name = "InjectedMonoManager";
            gameObject.AddComponent<InjectMonoManager>();
            Object.DontDestroyOnLoad(gameObject);
        }
        public static void Eject()
        {
            //Since this function is easily broken...
            //I'll leave it.
            //Object.Destroy(gameObject);
        }
    }
}
