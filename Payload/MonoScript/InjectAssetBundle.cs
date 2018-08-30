using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Payload.MonoScript
{
    public class InjectAssetBundle : MonoBehaviour
    {
        public static AssetBundle InjectedAssetBundle = null;

        void Awake()
        {
            InjectedAssetBundle = AssetBundle.LoadFromFile(@"./InjectAssetBundle/");
            if (InjectedAssetBundle == null)
            {
                Debug.LogError("Injector:Failed to inject AssetBundle!");
            }
            else
            {
                Debug.Log("Injector:Inject AssetBundle Succeed!");
            }

            string[] scenePaths = InjectedAssetBundle.GetAllScenePaths();
            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
            SceneManager.LoadScene(sceneName);
        }

        private void OnDestroy()
        {
            InjectedAssetBundle.Unload(true);
        }
    }
}
