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
            gameObject.AddComponent<Statistic>();
            //gameObject.AddComponent<InjectAssetBundle>();

            RefreshCamScript();
        }

        private void RefreshCamScript()
        {
            if (!Camera.main.gameObject.GetComponent<FreeMainCamera>())
                Camera.main.gameObject.AddComponent<FreeMainCamera>();

            foreach (Camera c in Camera.allCameras)
            {
                if (!c.gameObject.GetComponent<TriggerDrawer>())
                    c.gameObject.AddComponent<TriggerDrawer>();
            }

            Invoke("RefreshCamScript", 3f);
        }
    }
}
