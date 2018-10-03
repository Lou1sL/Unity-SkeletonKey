using System.Diagnostics.CodeAnalysis;
using UnityEngine;
//SceneManagement feture implemented after Unity 5.3(include)
//using UnityEngine.SceneManagement;

namespace Payload.MonoScript
{
    [SuppressMessage("Microsoft.Design", "CS0618")]
    public class Statistic : MonoBehaviour
    {

        private bool Active = true;
        private KeyCode Switch = KeyCode.PageUp;

        
        private Vector2 ScrollPosition = new Vector2();

        private void OnGUI()
        {
            if (!Active) return;

            AllRect.StatisticRect = GUILayout.Window(WindowID.GAME_STATISTIC, AllRect.StatisticRect, (id) =>
            {
                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
                GUILayout.BeginVertical();

                string str = "Unity Ver: " + Application.unityVersion + "\n";
                str += "FPS: " + Utils.Rnd((1.0f / Time.deltaTime)) + "\n\n";
                //After Unity 5.3(include)
                //str += "Current Scene: " + SceneManager.GetActiveScene().name + "\n";
                //Before Unity 5.3
                str += "Current Scene: (" + Application.loadedLevel + ") " + Application.loadedLevelName + "\n\n";
                str += "Cameras (Total:" + Camera.allCamerasCount + "):\n";
                GUILayout.Label(str, AllGUIStyle.DEFAULT_LABEL);

                
                foreach (Camera cam in Camera.allCameras)
                {

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("□", GUILayout.Width(20)))
                    {
                        Inspector.Activate(cam.transform);
                    }
                    GUILayout.Label((cam == Camera.main ? "(Main)" : "") + Utils.GetGameObjectPath(cam.gameObject), AllGUIStyle.DEFAULT_LABEL);
                    GUILayout.EndHorizontal();



                    GUILayout.BeginHorizontal();
                    if (!cam.GetComponent<FreeCamera>())
                    {
                        if (GUILayout.Button("Free", GUILayout.Width(60)))
                        {
                            foreach (Camera cams in Camera.allCameras)
                            {
                                FreeCamera fc = cams.GetComponent<FreeCamera>();
                                if (fc) DestroyImmediate(fc);
                            }
                            cam.gameObject.AddComponent<FreeCamera>();
                        }
                    }
                    GUILayout.EndHorizontal();

                    string camstr = string.Empty;
                    camstr += "Projection:" +
                        (cam.orthographic ?
                        "Orthographic (Size:" + cam.orthographicSize + ")" :
                        "Perspective (Fov:" + cam.fieldOfView + ")")
                        + "\n";
                    GUILayout.Label(camstr, AllGUIStyle.DEFAULT_LABEL);
                }

                
                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                if (GUILayout.Button("Close")) Active = false;


                GUI.DragWindow(new Rect(0, 0, AllRect.StatisticRect.width, 20));
            }, "Game Statistic", AllGUIStyle.DEFAULT_WINDOW);
        }

        private void Update()
        {
            if (Input.GetKeyDown(Switch)) Active = !Active;
        }


        
    }

}
