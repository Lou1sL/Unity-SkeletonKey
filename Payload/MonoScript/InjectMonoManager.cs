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
            //Camera.main.gameObject.AddComponent<FreeCamera>();
            //gameObject.AddComponent<InjectAssetBundle>();
        }
        
    }
}
