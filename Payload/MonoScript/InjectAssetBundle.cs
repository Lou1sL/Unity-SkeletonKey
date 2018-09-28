using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class InjectAssetBundle : MonoBehaviour
    {
        private static AssetBundle InjectedScene = null;
        private static AssetBundle InjectedAsset = null;

        void Start()
        {
            this.InjectLog("AssetBundle Injection Will Be Started In 5 Sec!");
            Invoke("InitAsset", 5f);
        }

        private void OnDestroy()
        {  
            InjectedScene.Unload(true);
        }


        private void InitAsset()
        {
            //InjectedAssetBundle = AssetBundle.LoadFromFile(@".\InjectAssetBundle\InjectAssetBundle");
            InjectedAsset = AssetBundle.LoadFromFile(@".\InjectAssetBundle\inject_asset");
            InjectedScene = AssetBundle.LoadFromFile(@".\InjectAssetBundle\inject_scene");
            
            string[] scenePaths = InjectedScene.GetAllScenePaths();

            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
            //this.InjectLog("Load scene:" + sceneName);
            //SceneManager.LoadScene(sceneName);
        }
    }
}
