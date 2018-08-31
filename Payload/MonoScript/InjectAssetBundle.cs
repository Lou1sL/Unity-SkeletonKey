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
        public static AssetBundle InjectedScene = null;
        public static AssetBundle InjectedAsset = null;

        void Start()
        {
            //InjectedAssetBundle = AssetBundle.LoadFromFile(@".\InjectAssetBundle\InjectAssetBundle");
            //InjectedAsset = AssetBundle.LoadFromFile(@".\InjectAssetBundle\inject_asset");
            InjectedScene = AssetBundle.LoadFromFile(@".\InjectAssetBundle\inject_scene");
            
            string[] scenePaths = InjectedScene.GetAllScenePaths();
            
            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
            SceneManager.LoadScene(sceneName);
        }

        private void OnDestroy()
        {
            InjectedScene.Unload(true);
        }
    }
}
