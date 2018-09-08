using UnityEngine;

namespace Payload.MonoScript
{
    public class InjectMonoManager : MonoBehaviour
    {
        public static new GameObject gameObject { get; private set; }

        private void Awake()
        {
            gameObject = base.gameObject;

            gameObject.AddComponent<Console>();
            gameObject.AddComponent<GameStatistic>();
            gameObject.AddComponent<TransformModifier>();
            //gameObject.AddComponent<InjectAssetBundle>();

            RefreshCamScript();
        }

        private void RefreshCamScript()
        {
            if (!Camera.main.gameObject.GetComponent<FreeMainCamera>())
                Camera.main.gameObject.AddComponent<FreeMainCamera>();


            if (!Camera.main.gameObject.GetComponent<ColliderDrawer>())
                Camera.main.gameObject.AddComponent<ColliderDrawer>();

            Invoke("RefreshCamScript", 3f);
        }
        
    }
}
