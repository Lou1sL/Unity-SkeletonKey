using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class SceneHierachy:MonoBehaviour
    {
        public class Analyze
        {
            public enum _Camera
            {
                None,
                MainCamera,
                SubCamera,
            }
            public enum _Render
            {
                None,
                UI,
                Mesh,
                Sprite
            }
            public enum _Physics
            {
                None,
                Rigidbody2D,
                Rigidbody3D,
            }
            public enum _Collider
            {
                None,
                Collider2D,
                Collider3D,
            }

            public _Camera Camera { get; private set; }
            public _Render Render { get; private set; }
            public _Physics Physics { get; private set; }
            public _Collider Collider { get; private set; }
            public bool IsClone { get; private set; }
            public bool IsTrigger { get; private set; }
            public GameObject gameObject;

            public Analyze(GameObject go)
            {
                gameObject = go;

                Camera cam = go.GetComponent<Camera>();
                MeshRenderer mr = go.GetComponent<MeshRenderer>();
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                RectTransform rt = go.GetComponent<RectTransform>();
                Rigidbody rig = go.GetComponent<Rigidbody>();
                Rigidbody2D rig2d = go.GetComponent<Rigidbody2D>();
                Collider co = go.GetComponent<Collider>();
                Collider2D co2d = go.GetComponent<Collider2D>();

                Camera =
                    cam ?
                    (cam == UnityEngine.Camera.main ? _Camera.MainCamera : _Camera.SubCamera)
                    : (Camera = _Camera.None);

                Render = mr ? _Render.Mesh : (sr ? _Render.Sprite : (rt ? _Render.UI : _Render.None));

                Physics = rig ? _Physics.Rigidbody3D : (rig2d ? _Physics.Rigidbody2D : _Physics.None);

                Collider = co ? _Collider.Collider3D : (co2d ? _Collider.Collider2D : _Collider.None);

                IsClone = gameObject.name.EndsWith("(Clone)");

                IsTrigger = co ? co.isTrigger : (co2d ? co2d.isTrigger : false);
            }
        }

        public class GameObjectInfo
        {
            public GameObject gameObject;
            public Analyze analyze;

            public GameObjectInfo(GameObject go)
            {
                gameObject = go;
            }

            public Analyze GetAnalyze()
            {
                if (analyze==null) analyze = new Analyze(gameObject);
                return analyze;
            }
        }
        
        private List<GameObjectInfo> AllGameObject = new List<GameObjectInfo>();

        public void Refresh()
        {
            AllGameObject.Clear();
            foreach(GameObject go in FindObjectsOfType<GameObject>())
            {
                AllGameObject.Add(new GameObjectInfo(go));
            }
        }
        public List<GameObjectInfo> GetAllGameObjects()
        {
            if (AllGameObject.Count == 0) Refresh();
            return AllGameObject;
        }

        private void OnGUI()
        {
            
        }
    }
}
