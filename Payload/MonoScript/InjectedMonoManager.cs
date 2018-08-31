using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class InjectedMonoManager : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.AddComponent<Console>();
            //gameObject.AddComponent<InjectAssetBundle>();
        }

        private void Update()
        {
            if(!Camera.main.gameObject.GetComponent<TriggerDrawer>())
                Camera.main.gameObject.AddComponent<TriggerDrawer>();

            if (!Camera.main.gameObject.GetComponent<FreeMainCamera>())
                Camera.main.gameObject.AddComponent<FreeMainCamera>();
        }

        private void OnDestroy()
        {
            //Destroy(Camera.main.gameObject.GetComponent<TriggerDrawer>());
            Camera.main.gameObject.GetComponent<TriggerDrawer>().enabled = false;
            Camera.main.gameObject.GetComponent<FreeMainCamera>().enabled = false;
        }
    }
}
