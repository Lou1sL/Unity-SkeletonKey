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
            gameObject.AddComponent<InjectedMonoManager>();
            Object.DontDestroyOnLoad(gameObject);
        }
        public static void Eject()
        {
            Object.Destroy(gameObject);
        }
    }
}
