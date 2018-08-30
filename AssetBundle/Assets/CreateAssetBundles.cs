#if UNITY_EDITOR

using UnityEditor;


public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("../Build/x86/InjectAssetBundle", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles("../Build/x64/InjectAssetBundle", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}

#endif