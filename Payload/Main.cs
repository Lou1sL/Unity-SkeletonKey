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
            gameObject.AddComponent<Console>();

            gameObject.AddComponent<InjectAssetBundle>();
            
            Camera.main.gameObject.AddComponent<ColliderDrawer>();

            Object.DontDestroyOnLoad(gameObject);
        }
        public static void Eject()
        {
            Object.Destroy(gameObject);
            Camera.main.gameObject.GetComponent<ColliderDrawer>().enabled = false;
        }
    }
}
