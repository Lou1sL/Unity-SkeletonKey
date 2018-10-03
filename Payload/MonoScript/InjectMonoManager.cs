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
            gameObject.AddComponent<FreeCamera>();
            gameObject.AddComponent<InjectAssetBundle>();
            gameObject.AddComponent<Inspector>();
            gameObject.AddComponent<SceneHierarchy>();
            gameObject.AddComponent<Statistic>();
            gameObject.AddComponent<Tetris>();
        }
        
    }
}
